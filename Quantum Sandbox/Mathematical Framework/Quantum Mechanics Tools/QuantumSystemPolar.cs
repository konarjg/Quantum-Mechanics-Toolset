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
        public DiscreteFunctionComplex[] WaveFunctionR { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionFI { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionMomentumSpaceR { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionMomentumSpaceFI { get; private set; }

        private List<double[]> PositionSpaceDistributionParametersR;
        private List<double[]> PositionSpaceDistributionParametersFI;
        private List<double[]> MomentumSpaceDistributionParametersR;
        private List<double[]> MomentumSpaceDistributionParametersFI;

        private double[] EnergiesR;
        private double[] EnergiesFI;

        public double Energy { get => EnergiesR[EnergyLevel] + EnergiesFI[AzimuthalLevel]; }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[] PositionDomainR;
        private double[] PositionDomainFI;
        private double[] MomentumDomainR;
        private double[] MomentumDomainFI;

        private Random Random;

        public QuantumSystemPolar(CancellationToken token, int precision, int energyLevel, int azimuthalLevel, double mass, string potentialX, string potentialY, double[,] positionDomain, double[,] momentumDomain)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            PositionDomainR = new double[] { positionDomain[0, 0], positionDomain[0, 1] };
            PositionDomainFI = new double[] { positionDomain[1, 0], positionDomain[1, 1] };
            MomentumDomainR = new double[] { momentumDomain[0, 0], momentumDomain[0, 1] };
            MomentumDomainFI = new double[] { momentumDomain[1, 0], momentumDomain[1, 1] };
            Precision = precision;
            EnergyLevel = energyLevel - 1;
            AzimuthalLevel = azimuthalLevel - 1;

            token.ThrowIfCancellationRequested();

            var T = -1 / (2 * mass);
            var V = new string[] { potentialX, potentialY };

            var boundaryConditionsR = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[0, 1].ToString(), "0"),
            };

            var boundaryConditionsFI = new BoundaryCondition[]
            {
                new PeriodicBoundaryCondition(2 * Math.PI)
            };

            token.ThrowIfCancellationRequested();
            var schrodingerEquationR = new string[] { T + "*r^2", T + "*r", "r^2*(" + V[0] + "-1)" };
            var schrodingerEquationFI = new string[] { -T + "", "0", "0" };

            var solutionR = DESolver.SolveEigenvalueODE(token, DifferenceScheme.CENTRAL, schrodingerEquationR, boundaryConditionsR, PositionDomainR, precision);
            var solutionFI = DESolver.SolveEigenvalueODE(token, DifferenceScheme.CENTRAL, schrodingerEquationFI, boundaryConditionsFI, PositionDomainFI, precision);
            token.ThrowIfCancellationRequested();

            var waveFunctionsR = solutionR.Values.ToArray();
            var waveFunctionsFI = solutionFI.Values.ToArray();
            EnergiesR = CreateVector.SparseOfEnumerable(solutionR.Keys).Real().ToArray();
            EnergiesFI = CreateVector.SparseOfEnumerable(solutionFI.Keys).Real().ToArray();

            var dr = (PositionDomainR[1] - PositionDomainR[0]) / (Precision - 1);
            var dfi = (PositionDomainFI[1] - PositionDomainFI[0]) / (Precision - 1);
            var r = CreateVector.Sparse<double>(Precision);
            var fi = CreateVector.Sparse<double>(Precision);

            for (int i = 0; i < Precision; ++i)
            {
                r[i] = PositionDomainR[0] + i * dr;
                fi[i] = PositionDomainFI[0] + i * dfi;
            }

            WaveFunctionR = new DiscreteFunctionComplex[10];
            WaveFunctionFI = new DiscreteFunctionComplex[10];
            WaveFunctionMomentumSpaceR = new DiscreteFunctionComplex[10];
            WaveFunctionMomentumSpaceFI = new DiscreteFunctionComplex[10];

            for (int i = 0; i < 10; ++i)
            {
                token.ThrowIfCancellationRequested();

                WaveFunctionR[i] = Interpolator.Cubic(r, waveFunctionsR[i]);
                WaveFunctionFI[i] = Interpolator.Cubic(fi, waveFunctionsFI[i]);

                var Nr = Math.Sqrt(1d / WaveFunctionR[i].GetMagnitudeSquared().Integrate(PositionDomainR[0], PositionDomainR[1], true));
                var Nfi = Math.Sqrt(1d / WaveFunctionFI[i].GetMagnitudeSquared().Integrate(PositionDomainFI[0], PositionDomainFI[1]));

                WaveFunctionR[i] = Interpolator.Cubic(r, waveFunctionsR[i] * Nr);
                WaveFunctionFI[i] = Interpolator.Cubic(fi, waveFunctionsFI[i] * Nfi);

                var px = WaveFunctionR[i].FourierTransform(MomentumDomainR);
                var py = WaveFunctionFI[i].FourierTransform(MomentumDomainFI);

                var Npx = Math.Sqrt(1d / px.GetMagnitudeSquared().Integrate(MomentumDomainX[0], MomentumDomainX[1]));
                var Npy = Math.Sqrt(1d / py.GetMagnitudeSquared().Integrate(MomentumDomainY[0], MomentumDomainY[1]));

                WaveFunctionMomentumSpaceX[i] = new DiscreteFunctionComplex(k => Npx * px.Evaluate(k));
                WaveFunctionMomentumSpaceY[i] = new DiscreteFunctionComplex(k => Npy * py.Evaluate(k));
                PositionSpaceDistributionParametersX.Add(GetPositionSpaceDistributionParametersX(i));
                PositionSpaceDistributionParametersY.Add(GetPositionSpaceDistributionParametersY(i));

                MomentumSpaceDistributionParametersX.Add(GetMomentumSpaceDistributionParametersX(i));
                MomentumSpaceDistributionParametersY.Add(GetMomentumSpaceDistributionParametersY(i));
                token.ThrowIfCancellationRequested();
            }

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
