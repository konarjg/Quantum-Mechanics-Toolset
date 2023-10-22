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
    public class QuantumSystem2D
    {
        private double[] EnergiesX;
        private double[] EnergiesY;
        public DiscreteFunctionComplex[] WaveFunctionX { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionY { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionMomentumSpaceX { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionMomentumSpaceY { get; private set; }

        private List<double[]> PositionSpaceDistributionParametersX = new List<double[]>();
        private List<double[]> PositionSpaceDistributionParametersY = new List<double[]>();
        private List<double[]> MomentumSpaceDistributionParametersX = new List<double[]>();
        private List<double[]> MomentumSpaceDistributionParametersY = new List<double[]>();

        public double Energy { get => EnergiesX[EnergyLevelX] + EnergiesY[EnergyLevelY]; }

        private int EnergyLevelX;
        private int EnergyLevelY;
        private int AzimuthalLevel;
        private int Precision;
        private double[] PositionDomainX;
        private double[] PositionDomainY;
        private double[] MomentumDomainX;
        private double[] MomentumDomainY;
        private double[] MomentumMagnitudeDomain;

        private Random Random;

        public QuantumSystem2D(CancellationToken token, int precision, int energyLevelX, int energyLevelY, int azimuthalLevel, double mass, string potentialX, string potentialY, double[,] positionDomain, double[,] momentumDomain, double[] momentumMagnitudeDomain)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            PositionDomainX = new double[] { positionDomain[0, 0], positionDomain[0, 1] };
            PositionDomainY = new double[] { positionDomain[1, 0], positionDomain[1, 1] };
            MomentumDomainX = new double[] { momentumDomain[0, 0], momentumDomain[0, 1] };
            MomentumDomainY = new double[] { momentumDomain[1, 0], momentumDomain[1, 1] };
            MomentumMagnitudeDomain = momentumMagnitudeDomain;
            Precision = precision;
            EnergyLevelX = energyLevelX - 1;
            EnergyLevelY = energyLevelY - 1;
            AzimuthalLevel = azimuthalLevel;

            token.ThrowIfCancellationRequested();

            var T = -1 / (2 * mass);
            var V = new string[] { potentialX, potentialY };

            var boundaryConditionsX = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[0, 1].ToString(), "0")
            };

            var boundaryConditionsY = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[1, 0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[1, 1].ToString(), "0")
            };

            token.ThrowIfCancellationRequested();
            var schrodingerEquationX = new string[] { T.ToString(), "0", V[0], "0" };
            var schrodingerEquationY = new string[] { T.ToString(), "0", V[1], "0" };

            var solutionX = DESolver.SolveODE(schrodingerEquationX, PositionDomainX, boundaryConditionsX, precision);
            var solutionY = DESolver.SolveODE(schrodingerEquationY, PositionDomainY, boundaryConditionsY, precision);
            token.ThrowIfCancellationRequested();

            var waveFunctionsX = new DiscreteFunctionComplex[10];
            var waveFunctionsY = new DiscreteFunctionComplex[10];
            EnergiesX = new double[10];
            EnergiesY = new double[10];

            for (int i = 0; i < 10; ++i)
            {
                EnergiesX[i] = solutionX[i].Item1;
                EnergiesY[i] = solutionY[i].Item1;
                waveFunctionsX[i] = solutionX[i].Item2;
                waveFunctionsY[i] = solutionY[i].Item2;
            }

            var dx = (PositionDomainX[1] - PositionDomainX[0]) / (Precision - 1);
            var dy = (PositionDomainY[1] - PositionDomainY[0]) / (Precision - 1);
            var x = CreateVector.Sparse<double>(Precision);
            var y = CreateVector.Sparse<double>(Precision);

            for (int i = 0; i < Precision; ++i)
            {
                x[i] = PositionDomainX[0] + i * dx;
                y[i] = PositionDomainY[0] + i * dy;
            }

            WaveFunctionX = new DiscreteFunctionComplex[10];
            WaveFunctionY = new DiscreteFunctionComplex[10];
            WaveFunctionMomentumSpaceX = new DiscreteFunctionComplex[10];
            WaveFunctionMomentumSpaceY = new DiscreteFunctionComplex[10];

            for (int i = 0; i < 10; ++i)
            {
                token.ThrowIfCancellationRequested();

                WaveFunctionX[i] = waveFunctionsX[i];
                WaveFunctionY[i] = waveFunctionsY[i];

                var Nx = Math.Sqrt(1d / WaveFunctionX[i].GetMagnitudeSquared().Integrate(PositionDomainX[0], PositionDomainX[1]));
                var Ny = Math.Sqrt(1d / WaveFunctionY[i].GetMagnitudeSquared().Integrate(PositionDomainY[0], PositionDomainY[1]));

                WaveFunctionX[i] = new DiscreteFunctionComplex(x => Nx * waveFunctionsX[i].Evaluate(x));
                WaveFunctionY[i] = new DiscreteFunctionComplex(y => Ny * waveFunctionsY[i].Evaluate(y));

                var px = WaveFunctionX[i].FourierTransform(MomentumDomainX);
                var py = WaveFunctionY[i].FourierTransform(MomentumDomainY);

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
            var f = new DiscreteFunction2DComplex((x, y) => WaveFunctionX[EnergyLevelX].Evaluate(x) * WaveFunctionY[EnergyLevelY].Evaluate(y));
            f.Plot(plot, new double[,] { { PositionDomainX[0], PositionDomainX[1] }, { PositionDomainY[0], PositionDomainY[1] } }, (int)Math.Sqrt(Precision));
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            var n = Precision / 5;
            var dp = (MomentumMagnitudeDomain[1] - MomentumMagnitudeDomain[0]) / (n - 1);

            var fx = WaveFunctionMomentumSpaceX[EnergyLevelX];
            var fy = WaveFunctionMomentumSpaceY[EnergyLevelY];
            
            var psi = new DiscreteFunction2DComplex((x, y) => fx.Evaluate(x) * fy.Evaluate(y));
            var p = CreateVector.Sparse<double>(n);
            var u = CreateVector.Sparse<Complex>(n);
            
            for (int i = 0; i < n; ++i)
            {
                p[i] = MomentumMagnitudeDomain[0] + i * dp;
                u[i] = psi.IntegratePolar(p[i] - dp, p[i] + dp, 0, 2 * Math.PI);
            }

            var f = Interpolator.Cubic(p, u);
            f.Plot(plot, MomentumMagnitudeDomain, n);
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

        public double[] GetPositionDomainX()
        {
            return PositionDomainX;
        }

        public double[] GetPositionDomainY()
        {
            return PositionDomainY;
        }

        private double[] GetPositionSpaceDistributionParametersX(int i)
        {
            var density = WaveFunctionX[i];

            var mean_x = new DiscreteFunction(x => x * density.Evaluate(x).MagnitudeSquared()).Integrate(PositionDomainX[0], PositionDomainX[1]);
            var var_x = new DiscreteFunction(x => (x - mean_x) * (x - mean_x) * density.Evaluate(x).MagnitudeSquared()).Integrate(PositionDomainX[0], PositionDomainX[1]);

            return new double[] { mean_x, Math.Sqrt(var_x) };
        }

        private double[] GetPositionSpaceDistributionParametersY(int i)
        {
            var density = WaveFunctionY[i];

            var mean_y = new DiscreteFunction(y => y * density.Evaluate(y).MagnitudeSquared()).Integrate(PositionDomainY[0], PositionDomainY[1]);
            var var_y = new DiscreteFunction(y => (y - mean_y) * (y - mean_y) * density.Evaluate(y).MagnitudeSquared()).Integrate(PositionDomainY[0], PositionDomainY[1]);

            return new double[] { mean_y, Math.Sqrt(var_y) };
        }

        public double GetProbabilityPositionSpace(double x, double y)
        {
            var dx = (PositionDomainX[1] - PositionDomainX[0]) / (Precision - 1);
            var dy = (PositionDomainY[1] - PositionDomainY[0]) / (Precision - 1);

            var Px = WaveFunctionX[EnergyLevelX].GetMagnitudeSquared().Integrate(x - dx, x + dx);
            var Py = WaveFunctionY[EnergyLevelY].GetMagnitudeSquared().Integrate(y - dy, y + dy);

            return Px * Py;
        }

        public Tuple<double, double> ExpectedPosition()
        {
            var parametersX = PositionSpaceDistributionParametersX[EnergyLevelX];
            var parametersY = PositionSpaceDistributionParametersY[EnergyLevelY];
            var exp_x = parametersX[0];
            var exp_y = parametersY[0];

            return Tuple.Create(exp_x, exp_y);
        }

        public Tuple<double, double> MeasurePosition(int seed)
        {
            var parametersX = PositionSpaceDistributionParametersX[EnergyLevelX];
            var parametersY = PositionSpaceDistributionParametersY[EnergyLevelY];
            Random = new Random(seed);

            var x = Normal.Sample(Random, parametersX[0], parametersX[1]);
            var y = Normal.Sample(Random, parametersY[0], parametersY[1]);

            while (x <= PositionDomainX[0] || x >= PositionDomainX[1])
                x = Normal.Sample(Random, parametersX[0], parametersX[1]);

            while (y <= PositionDomainY[0] || y >= PositionDomainY[1])
                y = Normal.Sample(Random, parametersY[0], parametersY[1]);

            return Tuple.Create(x, y);
        }

        #endregion

        #region Momentum Space

        private double[] GetMomentumSpaceDistributionParametersX(int i)
        {
            var density = WaveFunctionMomentumSpaceX[i].GetMagnitudeSquared();
            var mean = new DiscreteFunction(x => x * density.Evaluate(x)).Integrate(MomentumDomainX[0], MomentumDomainX[1]);
            var var = new DiscreteFunction(x => (x - mean) * (x - mean) * density.Evaluate(x)).Integrate(MomentumDomainX[0], MomentumDomainX[1]);

            return new double[] { mean, Math.Sqrt(var) };
        }

        private double[] GetMomentumSpaceDistributionParametersY(int i)
        {
            var density = WaveFunctionMomentumSpaceY[i].GetMagnitudeSquared();
            var mean = new DiscreteFunction(y => y * density.Evaluate(y)).Integrate(MomentumDomainY[0], MomentumDomainY[1]);
            var var = new DiscreteFunction(y => (y - mean) * (y - mean) * density.Evaluate(y)).Integrate(MomentumDomainY[0], MomentumDomainY[1]);

            return new double[] { mean, Math.Sqrt(var) };
        }

        public double GetProbabilityMomentumSpace(double k)
        {
            var n = Precision * Precision;
            var dk = (MomentumMagnitudeDomain[1] - MomentumMagnitudeDomain[0]) / (n - 1);
            var x = WaveFunctionX[EnergyLevelX].GetMagnitudeSquared();
            var y = WaveFunctionY[EnergyLevelY].GetMagnitudeSquared();

            var f = new DiscreteFunction2D((p, fi) => x.Evaluate(p * Math.Cos(fi) * y.Evaluate(p * Math.Sin(fi))));
            return f.Integrate(k - dk, k + dk, 0, 2 * Math.PI);
        }

        public double ExpectedMomentum()
        {
            var parametersX = MomentumSpaceDistributionParametersX[EnergyLevelX];
            var parametersY = MomentumSpaceDistributionParametersY[EnergyLevelY];

            var px = parametersX[0];
            var py = parametersY[0];

            return Math.Sqrt(px * px + py * py);
        }

        public double MeasureMomentum(int seed)
        {
            Random = new Random(seed);
            var parametersX = MomentumSpaceDistributionParametersX[EnergyLevelX];
            var parametersY = MomentumSpaceDistributionParametersY[EnergyLevelY];
            var px = Normal.Sample(Random, parametersX[0], parametersX[1]);
            var py = Normal.Sample(Random, parametersY[0], parametersY[1]);
            var p = Math.Sqrt(px * px + py * py);

            while (p <= MomentumMagnitudeDomain[0] || p >= MomentumMagnitudeDomain[1])
            {
                px = Normal.Sample(Random, parametersX[0], parametersX[1]);
                py = Normal.Sample(Random, parametersY[0], parametersY[1]);
                p = Math.Sqrt(px * px + py * py);
            }

            return p; 
        }

        #endregion
    }
}
