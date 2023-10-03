using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystemPolar
    {
        public DiscreteFunction2DComplex WaveFunction { get; private set; }
        public DiscreteFunction2DComplex WaveFunctionMomentumSpace { get; private set; }
        public DiscreteFunction2D PositionSpaceProbabilityDensity { get; private set; }
        public DiscreteFunction2D MomentumSpaceProbabilityDensity { get; private set; }

        private double[,] PositionSpaceDistributionParameters;
        private double[] MomentumSpaceDistributionParameters;

        public double Energy { get; private set; }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[,] PositionDomain;
        private double[,] MomentumDomain;

        private Random Random;

        public QuantumSystemPolar(CancellationToken token, int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[,] positionDomain, double[,] momentumDomain)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            PositionDomain = positionDomain;
            MomentumDomain = momentumDomain;
            Precision = precision;
            EnergyLevel = energyLevel;
            AzimuthalLevel = azimuthalLevel;

            token.ThrowIfCancellationRequested();

            var T = -1 / (2 * mass);
            var V = potential;

            var boundaryConditions = new BoundaryConditionPDE[]
            {
                new BoundaryConditionPDE(0, 0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 0, positionDomain[0, 1].ToString(), "0")
            };

            token.ThrowIfCancellationRequested();
            var schrodingerEquation = new string[] { T.ToString(), T + "/(x^2 + 0,0001)", T + "/(x + 0,0001)", "0", V };

            var solution = DESolver.SolveEigenvaluePDE(token, DifferenceScheme.CENTRAL, schrodingerEquation, boundaryConditions, positionDomain, precision);
            token.ThrowIfCancellationRequested();

            var t = 0;
            var done = false;

            for (int n = 1; n <= 10; ++n)
            {
                if (done)
                    break;

                for (int l = 0; l <= 10; ++l)
                {
                    if (done)
                        break;

                    if (n == EnergyLevel && l == AzimuthalLevel - 1)
                    {
                        done = true;
                        break;
                    }

                    ++t;
                }
            }

            Energy = solution.Keys.ElementAt(t).Real;
            var u_vector = solution.Values.ElementAt(t);

            var dx = (positionDomain[0, 1] - positionDomain[0, 0]) / (precision - 1);
            var dy = (positionDomain[1, 1] - positionDomain[1, 0]) / (precision - 1);
            var x = CreateVector.Sparse<double>(precision);
            var y = CreateVector.Sparse<double>(precision);
            var u = CreateMatrix.Sparse<Complex>(Precision, Precision);

            token.ThrowIfCancellationRequested();

            for (int i = 0; i < precision; ++i)
            {
                token.ThrowIfCancellationRequested();

                for (int j = 0; j < precision; ++j)
                {
                    x[i] = positionDomain[0, 0] + i * dx;
                    y[j] = positionDomain[1, 0] + j * dy;
                    u[i, j] = u_vector[precision * i + j];
                    token.ThrowIfCancellationRequested();
                }
            }

            WaveFunction = Interpolator.Bicubic(x, y, u);
            var density = WaveFunction.GetMagnitudeSquared();

            var N = Math.Sqrt(1d / density.Integrate(positionDomain[0, 0], positionDomain[0, 1], positionDomain[1, 0], positionDomain[1, 1], true));

            token.ThrowIfCancellationRequested();

            WaveFunction = Interpolator.Bicubic(x, y, N * u);
            WaveFunctionMomentumSpace = WaveFunction.FourierTransform(positionDomain);
            density = WaveFunctionMomentumSpace.GetMagnitudeSquared();

            token.ThrowIfCancellationRequested();

            var Np = Math.Sqrt(1d / density.Integrate(momentumDomain[0, 0], momentumDomain[0, 1], momentumDomain[1, 0], momentumDomain[1, 1], true));
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (Precision - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (Precision - 1);
            var kx = CreateVector.Sparse<double>(Precision);
            var ky = CreateVector.Sparse<double>(Precision);
            var k = CreateMatrix.Sparse<Complex>(Precision, Precision);

            token.ThrowIfCancellationRequested();

            for (int i = 0; i < Precision; ++i)
            {
                token.ThrowIfCancellationRequested();

                for (int j = 0; j < Precision; ++j)
                {
                    kx[i] = MomentumDomain[0, 0] + i * dkx;
                    ky[j] = MomentumDomain[1, 0] + j * dky;

                    k[i, j] = Np * WaveFunctionMomentumSpace.Evaluate(kx[i], ky[j]);
                    token.ThrowIfCancellationRequested();
                }
            }

            WaveFunctionMomentumSpace = Interpolator.Bicubic(kx, ky, k);
            PositionSpaceProbabilityDensity = WaveFunction.GetMagnitudeSquared();
            MomentumSpaceProbabilityDensity = WaveFunctionMomentumSpace.GetMagnitudeSquared();
            PositionSpaceDistributionParameters = GetPositionSpaceDistributionParameters();
            MomentumSpaceDistributionParameters = GetMomentumSpaceDistributionParameters();
            token.ThrowIfCancellationRequested();
        }

        public void PlotPositionSpace(FormsPlot plot)
        {
            var domain = new double[,] { { -PositionDomain[0, 1], PositionDomain[0, 1] }, { -PositionDomain[0, 1], PositionDomain[0, 1] } };

            var f = new DiscreteFunction2DComplex((x, y) =>
            {
                var r = Math.Sqrt(x * x + y * y);
                var fi = Math.Atan2(y, x);

                if (fi < 0)
                    fi += 2 * Math.PI;

                return WaveFunction.Evaluate(r, fi);
            });

            f.Plot(plot, domain, Precision);
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            var n = Precision * Precision;
            var dk = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (n - 1);
            var k = CreateVector.Sparse<double>(n);
            var p = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                k[i] = MomentumDomain[0, 0] + i * dk;
                p[i] = MomentumSpaceProbabilityDensity.Integrate(k[i] - dk, k[i] + dk, MomentumDomain[1, 0], MomentumDomain[1, 1], true);
            }

            var f = Interpolator.Cubic(k, p);
            f.Plot(plot, CreateMatrix.SparseOfArray(MomentumDomain).Row(0).ToArray(), n);
        }

        private int RandomSign()
        {
            var sign = Random.Next(0, 2);

            if (sign == 0)
                return -1;

            return 1;
        }

        public Tuple<Tuple<double, double>, double> MeasurePositionMomentum(int seed)
        {
            Random = new Random(seed);
            var r = MeasurePosition(seed);
            var x = r.Item1;
            var y = r.Item1;
            var p = MeasureMomentum(seed);

            var R = Tuple.Create(x + RandomSign() * Random.NextDouble() * 0.5, y + RandomSign() * Random.NextDouble());
            return Tuple.Create(R, p + RandomSign() * Random.NextDouble() * 0.5);
        }

        public double MeasureAngularMomentum()
        {
            return AzimuthalLevel * (AzimuthalLevel + 1);
        }

        #region Position Space

        public double[,] GetPositionDomain()
        {
            return PositionDomain;
        }

        private double[,] GetPositionSpaceDistributionParameters()
        {
            var mean_x = new DiscreteFunction2D((x, y) => x * Math.Cos(y) * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1], true);
            var mean_y = new DiscreteFunction2D((x, y) => x * Math.Sin(y) * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1], true);
            var var_x = new DiscreteFunction2D((x, y) => (x * Math.Cos(y) - mean_x) * (x * Math.Cos(y) - mean_x) * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1], true);
            var var_y = new DiscreteFunction2D((x, y) => (x * Math.Sin(y) - mean_y) * (x * Math.Sin(y) - mean_y) * PositionSpaceProbabilityDensity.Evaluate(x, y)).Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1], true);

            return new double[,] { { mean_x, Math.Sqrt(var_x) }, { mean_y, Math.Sqrt(var_y) } };
        }

        public double GetProbabilityPositionSpace(double x, double y)
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (Precision - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(x - dx, x + dx, y - dy, y + dy, true);
        }

        public Tuple<double, double> ExpectedPosition()
        {
            var fx = new DiscreteFunction2D(new Func<double, double, double>((x, y) => x * Math.Cos(y) * PositionSpaceProbabilityDensity.Evaluate(x, y)));
            var fy = new DiscreteFunction2D(new Func<double, double, double>((x, y) => x * Math.Sin(y) * PositionSpaceProbabilityDensity.Evaluate(x, y)));

            var exp_x = fx.Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1], true);
            var exp_y = fy.Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1], true);

            return Tuple.Create(exp_x, exp_y);
        }

        public Tuple<double, double> MeasurePosition(int seed)
        {
            Random = new Random(seed);
            var parameters = PositionSpaceDistributionParameters;

            var x = Normal.Sample(Random, parameters[0, 0], parameters[0, 1]);
            var y = Normal.Sample(Random, parameters[1, 0], parameters[1, 1]);
            var r = Math.Sqrt(x * x + y * y);

            while (r <= PositionDomain[0, 0] || r >= PositionDomain[0, 1])
            {
                x = Normal.Sample(Random, parameters[0, 0], parameters[0, 1]);
                y = Normal.Sample(Random, parameters[1, 0], parameters[1, 1]);
                r = Math.Sqrt(x * x + y * y);
            }

            return Tuple.Create(x, y);
        }

        #endregion

        #region Momentum Space

        private double[] GetMomentumSpaceDistributionParameters()
        {
            var mean = new DiscreteFunction2D((x, y) => x * MomentumSpaceProbabilityDensity.Evaluate(x, y)).Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1], true);
            var var = new DiscreteFunction2D((x, y) => (x - mean) * (x - mean) * MomentumSpaceProbabilityDensity.Evaluate(x, y)).Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1], true);

            return new double[] { mean, Math.Sqrt(var) };
        }

        public double GetProbabilityMomentumSpace(double k)
        {
            var n = Precision * Precision;
            var dk = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (n - 1);
            return MomentumSpaceProbabilityDensity.Integrate(k - dk, k + dk, MomentumDomain[1, 0], MomentumDomain[1, 1], true);
        }

        public double ExpectedMomentum()
        {
            var f = new DiscreteFunction2D(new Func<double, double, double>((kx, ky) => kx * MomentumSpaceProbabilityDensity.Evaluate(kx, ky)));
            return f.Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1], true);
        }

        public double MeasureMomentum(int seed)
        {
            Random = new Random(seed);
            var parameters = MomentumSpaceDistributionParameters;
            var p = Normal.Sample(Random, parameters[0], parameters[1]);

            while (p <= MomentumDomain[0, 0] || p >= MomentumDomain[0, 1])
                p = Normal.Sample(Random, parameters[0], parameters[1]);

            return p; 
        }

        #endregion
    }
}
