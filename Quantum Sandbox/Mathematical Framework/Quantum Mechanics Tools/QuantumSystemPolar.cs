using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
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
        private double[] EnergiesR;
        private double[] EnergiesTheta;

        public DiscreteFunctionComplex[] WaveFunctionR { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionTheta { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionMomentumSpace { get; private set; }

        private List<double[]> PositionSpaceDistributionParametersR = new List<double[]>();
        private List<double[]> PositionSpaceDistributionParametersTheta = new List<double[]>();
        private List<double[]> MomentumSpaceDistributionParameters = new List<double[]>();
        public double Energy { get => EnergiesR[EnergyLevel] + EnergiesTheta[AzimuthalLevel]; }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[] PositionDomainR;
        private double[] PositionDomainTheta;
        private double[] MomentumDomain;

        private Random Random;

        public QuantumSystemPolar(CancellationToken token, int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[,] positionDomain, double[] momentumDomain)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            PositionDomainR = new double[] { positionDomain[0, 0], positionDomain[0, 1] };
            PositionDomainTheta = new double[] { positionDomain[1, 0], positionDomain[1, 1] };
            MomentumDomain = momentumDomain;
            Precision = precision;
            EnergyLevel = energyLevel - 1;
            AzimuthalLevel = azimuthalLevel - 1;

            token.ThrowIfCancellationRequested();

            var T = 1 / (2 * mass);
            var V = potential;

            var boundaryConditionsR = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0, 1].ToString(), "0")
            };

            var boundaryConditionsTheta = new BoundaryCondition[]
            {
                new PeriodicBoundaryCondition(2 * Math.PI)
            };

            token.ThrowIfCancellationRequested();
            var schrodingerEquationR = new string[] { T.ToString(), T.ToString() + "/(x+0,1)", V, T.ToString() + "/(x^2+0,1)" };
            var schrodingerEquationTheta = new string[] { T.ToString(), "0", "0", "0" };

            var solutionR = DESolver.SolveODE(schrodingerEquationR, PositionDomainR, boundaryConditionsR, precision);
            var solutionTheta = DESolver.SolveODE(schrodingerEquationTheta, PositionDomainTheta, boundaryConditionsTheta, precision);
            token.ThrowIfCancellationRequested();

            var waveFunctionsR = solutionR.Values.ToArray();
            var waveFunctionsTheta = solutionTheta.Values.ToArray();
            EnergiesR = CreateVector.SparseOfEnumerable(solutionR.Keys).Real().ToArray();
            EnergiesTheta = CreateVector.SparseOfEnumerable(solutionTheta.Keys).Real().ToArray();

            var dr = (PositionDomainR[1] - PositionDomainR[0]) / (Precision - 1);
            var dtheta = (PositionDomainTheta[1] - PositionDomainTheta[0]) / (Precision - 1);
            var r = CreateVector.Sparse<double>(Precision);
            var theta = CreateVector.Sparse<double>(Precision);

            for (int i = 0; i < Precision; ++i)
            {
                r[i] = PositionDomainR[0] + i * dr;
                theta[i] = PositionDomainTheta[0] + i * dtheta;
            }

            WaveFunctionR = new DiscreteFunctionComplex[10];
            WaveFunctionTheta = new DiscreteFunctionComplex[10];
            WaveFunctionMomentumSpace = new DiscreteFunctionComplex[10];

            MessageBox.Show(waveFunctionsR.Length + "");
            for (int i = 0; i < 10; ++i)
            {
                token.ThrowIfCancellationRequested();

                WaveFunctionR[i] = Interpolator.Cubic(r, waveFunctionsR[i]);
                WaveFunctionTheta[i] = Interpolator.Cubic(theta, waveFunctionsTheta[i]);

                var Nr = Math.Sqrt(1d / WaveFunctionR[i].GetMagnitudeSquared().Integrate(PositionDomainR[0], PositionDomainR[1], true));
                var Ntheta = Math.Sqrt(1d / WaveFunctionTheta[i].GetMagnitudeSquared().Integrate(PositionDomainTheta[0], PositionDomainTheta[1]));

                WaveFunctionR[i] = Interpolator.Cubic(r, waveFunctionsR[i] * Nr);
                WaveFunctionTheta[i] = Interpolator.Cubic(theta, waveFunctionsTheta[i] * Ntheta);

                var p = WaveFunctionR[i].FourierTransform(MomentumDomain);

                var Np = Math.Sqrt(1d / p.GetMagnitudeSquared().Integrate(MomentumDomain[0], MomentumDomain[1]));
                WaveFunctionMomentumSpace[i] = new DiscreteFunctionComplex(k => Np * p.Evaluate(k));
                PositionSpaceDistributionParametersR.Add(GetPositionSpaceDistributionParametersR(i));
                PositionSpaceDistributionParametersTheta.Add(GetPositionSpaceDistributionParametersTheta(i));

                MomentumSpaceDistributionParameters.Add(GetMomentumSpaceDistributionParameters(i));
                token.ThrowIfCancellationRequested();
            }

            token.ThrowIfCancellationRequested();
        }

        public void PlotPositionSpace(FormsPlot plot)
        {
            var domain = new double[,] { { -PositionDomainR[1], PositionDomainR[1] }, { -PositionDomainR[1], PositionDomainR[1] } };
            var f = new DiscreteFunction2DComplex((r, theta) => WaveFunctionR[EnergyLevel].Evaluate(r) * WaveFunctionTheta[AzimuthalLevel].Evaluate(theta));
            var g = new DiscreteFunction2DComplex((x, y) =>
            {
                var r = Math.Sqrt(x * x + y * y);
                var theta = Math.Atan2(y, x);

                if (theta < 0)
                    theta += 2 * Math.PI;

                return f.Evaluate(r, theta);
            });

            g.Plot(plot, domain, (int)Math.Sqrt(Precision));
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            WaveFunctionMomentumSpace[EnergyLevel].Plot(plot, MomentumDomain, Precision);
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

        public double[] GetPositionDomainR()
        {
            return PositionDomainR;
        }

        public double[] GetPositionDomainTheta()
        {
            return PositionDomainTheta;
        }

        private double[] GetPositionSpaceDistributionParametersR(int i)
        {
            var density = WaveFunctionR[i];

            var mean_x = new DiscreteFunction(x => x * density.Evaluate(x).MagnitudeSquared()).Integrate(PositionDomainR[0], PositionDomainR[1]);
            var var_x = new DiscreteFunction(x => (x - mean_x) * (x - mean_x) * density.Evaluate(x).MagnitudeSquared()).Integrate(PositionDomainR[0], PositionDomainR[1]);

            return new double[] { mean_x, Math.Sqrt(var_x) };
        }

        private double[] GetPositionSpaceDistributionParametersTheta(int i)
        {
            var density = WaveFunctionTheta[i];

            var mean_y = new DiscreteFunction(y => y * density.Evaluate(y).MagnitudeSquared()).Integrate(PositionDomainTheta[0], PositionDomainTheta[1]);
            var var_y = new DiscreteFunction(y => (y - mean_y) * (y - mean_y) * density.Evaluate(y).MagnitudeSquared()).Integrate(PositionDomainTheta[0], PositionDomainTheta[1]);

            return new double[] { mean_y, Math.Sqrt(var_y) };
        }

        public double GetProbabilityPositionSpace(double x, double y)
        {
            var dx = (PositionDomainR[1] - PositionDomainR[0]) / (Precision - 1);
            var dy = (PositionDomainTheta[1] - PositionDomainTheta[0]) / (Precision - 1);

            var Px = WaveFunctionR[EnergyLevel].GetMagnitudeSquared().Integrate(x - dx, x + dx);
            var Py = WaveFunctionTheta[AzimuthalLevel].GetMagnitudeSquared().Integrate(y - dy, y + dy);

            return Px * Py;
        }

        public Tuple<double, double> ExpectedPosition()
        {
            var parametersX = PositionSpaceDistributionParametersR[EnergyLevel];
            var parametersY = PositionSpaceDistributionParametersTheta[AzimuthalLevel];
            var exp_x = parametersX[0];
            var exp_y = parametersY[0];

            return Tuple.Create(exp_x, exp_y);
        }

        public Tuple<double, double> MeasurePosition(int seed)
        {
            var parametersX = PositionSpaceDistributionParametersR[EnergyLevel];
            var parametersY = PositionSpaceDistributionParametersTheta[AzimuthalLevel];
            Random = new Random(seed);

            var x = Normal.Sample(Random, parametersX[0], parametersX[1]);
            var y = Normal.Sample(Random, parametersY[0], parametersY[1]);

            while (x <= PositionDomainR[0] || x >= PositionDomainR[1])
                x = Normal.Sample(Random, parametersX[0], parametersX[1]);

            while (y <= PositionDomainTheta[0] || y >= PositionDomainTheta[1])
                y = Normal.Sample(Random, parametersY[0], parametersY[1]);

            return Tuple.Create(x, y);
        }

        #endregion

        #region Momentum Space

        private double[] GetMomentumSpaceDistributionParameters(int i)
        {
            var density = WaveFunctionMomentumSpace[i].GetMagnitudeSquared();
            var mean = new DiscreteFunction(x => x * density.Evaluate(x)).Integrate(MomentumDomain[0], MomentumDomain[1]);
            var var = new DiscreteFunction(x => (x - mean) * (x - mean) * density.Evaluate(x)).Integrate(MomentumDomain[0], MomentumDomain[1]);

            return new double[] { mean, Math.Sqrt(var) };
        }

        public double ExpectedMomentum()
        {
            var parameters = MomentumSpaceDistributionParameters[EnergyLevel];
            var p = parameters[0];

            return p;
        }

        public double MeasureMomentum(int seed)
        {
            Random = new Random(seed);
            var parameters = MomentumSpaceDistributionParameters[EnergyLevel];
            var p = Normal.Sample(Random, parameters[0], parameters[1]);

            while (p <= MomentumDomain[0] || p >= MomentumDomain[1])
                p = Normal.Sample(Random, parameters[0], parameters[1]);

            return p; 
        }

        #endregion
    }
}
