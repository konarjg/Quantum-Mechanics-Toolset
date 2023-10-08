using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystem1D
    {
        public DiscreteFunctionComplex[] WaveFunction { get; private set; }
        public DiscreteFunctionComplex[] WaveFunctionMomentumSpace { get; private set; }
        private List<double[]> PositionSpaceDistributionParameters;
        private List<double[]> MomentumSpaceDistributionParameters;

        public double Energy { get => Energies[EnergyLevel]; }

        private double[] Energies;
        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[] PositionDomain;
        private double[] MomentumDomain;

        private Random Random;

        public QuantumSystem1D(CancellationToken token, int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[] positionDomain, double[] momentumDomain)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            PositionDomain = positionDomain;
            MomentumDomain = momentumDomain;
            Precision = precision;
            EnergyLevel = energyLevel - 1;
            AzimuthalLevel = azimuthalLevel;

            var T = -1 / (2 * mass);
            var V = potential;

            var boundaryConditions = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[1].ToString(), "0")
            };

            token.ThrowIfCancellationRequested();
            var schrodingerEquation = new string[] { T.ToString(), "0", V };

            var solution = DESolver.SolveEigenvalueODE(token, DifferenceScheme.CENTRAL, schrodingerEquation, boundaryConditions, positionDomain, precision);
            token.ThrowIfCancellationRequested();

            Energies = new double[10];
            WaveFunction = new DiscreteFunctionComplex[10];
            WaveFunctionMomentumSpace = new DiscreteFunctionComplex[10];
            PositionSpaceDistributionParameters = new List<double[]>();
            MomentumSpaceDistributionParameters = new List<double[]>();

            var dx = (PositionDomain[1] - PositionDomain[0]) / (Precision - 1);
            var x = CreateVector.Sparse<double>(Precision);

            for (int i = 0; i < Precision; ++i)
            {
                x[i] = PositionDomain[0] + i * dx;
                token.ThrowIfCancellationRequested();
            }

            for (int i = 0; i < 10; ++i)
            {
                token.ThrowIfCancellationRequested();
                Energies[i] = solution.Keys.ElementAt(i).Real;
                var function = solution.Values.ElementAt(i);

                var fx = Interpolator.Cubic(x, function);
                var N = Math.Sqrt(1d / fx.GetMagnitudeSquared().Integrate(PositionDomain[0], PositionDomain[1]));
                
                WaveFunction[i] = new DiscreteFunctionComplex(x => N * fx.Evaluate(x));
                token.ThrowIfCancellationRequested();

                var fp = WaveFunction[i].FourierTransform(MomentumDomain);
                var Np = Math.Sqrt(1d / fp.GetMagnitudeSquared().Integrate(MomentumDomain[0], MomentumDomain[1]));

                WaveFunctionMomentumSpace[i] = new DiscreteFunctionComplex(p => Np * fp.Evaluate(p));

                PositionSpaceDistributionParameters.Add(GetPositionSpaceDistributionParameters(i));
                MomentumSpaceDistributionParameters.Add(GetMomentumSpaceDistributionParameters(i));
                token.ThrowIfCancellationRequested();
            }

           

            token.ThrowIfCancellationRequested();
        }

        public void PlotPositionSpace(FormsPlot plot)
        {
            WaveFunction[EnergyLevel].Plot(plot, PositionDomain, Precision);
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            WaveFunctionMomentumSpace[EnergyLevel].Plot(plot, MomentumDomain, Precision);
        }

        private double RandomSign()
        {
            var sign = Random.Next(0, 2);

            if (sign == 0)
                return -1;

            return 1;
        }

        public Tuple<double, double> MeasurePositionMomentum(int seed)
        {
            Random = new Random(seed);
            var x = MeasurePosition(seed);
            var p = MeasureMomentum(seed);

            return Tuple.Create(x + RandomSign() * Random.NextDouble() * 0.5, p + RandomSign() * Random.NextDouble() * 0.5);
        }

        public double MeasureAngularMomentum()
        {
            return AzimuthalLevel * (AzimuthalLevel + 1);
        }

        #region Position Space


        public double[] GetPositionDomain()
        {
            return PositionDomain;
        }

        private double[] GetPositionSpaceDistributionParameters(int i)
        {
            var f = WaveFunction[i].GetMagnitudeSquared();
            var mean = new DiscreteFunction(x => x * f.Evaluate(x)).Integrate(PositionDomain[0], PositionDomain[1]);
            var std = Math.Sqrt(new DiscreteFunction(x => (x - mean) * (x - mean) * f.Evaluate(x)).Integrate(PositionDomain[0], PositionDomain[1]));

            return new double[] { mean, std };
        }

        private double[] GetMomentumSpaceDistributionParameters(int i)
        {
            var f = WaveFunctionMomentumSpace[i].GetMagnitudeSquared();
            var mean = new DiscreteFunction(p => p * f.Evaluate(p)).Integrate(MomentumDomain[0], MomentumDomain[1]);
            var std = Math.Sqrt(new DiscreteFunction(p => (p - mean) * (p - mean) * f.Evaluate(p)).Integrate(MomentumDomain[0], MomentumDomain[1]));

            return new double[] { mean, std };
        }

        public double GetProbabilityPositionSpace(double x)
        {
            var dx = (PositionDomain[1] - PositionDomain[0]) / (Precision - 1);
            return WaveFunction[EnergyLevel].GetMagnitudeSquared().Integrate(x - dx, x + dx);
        }

        public double ExpectedPosition()
        {
            return PositionSpaceDistributionParameters[EnergyLevel][0];
        }

        public double MeasurePosition(int seed)
        {
            Random = new Random(seed);
            var mean = PositionSpaceDistributionParameters[EnergyLevel][0];
            var std = PositionSpaceDistributionParameters[EnergyLevel][1];
            var x = Normal.Sample(Random, mean, std);

            while (x <= PositionDomain[0] || x >= PositionDomain[1])
                x = Normal.Sample(Random, mean, std);   

            return x;
        }

        #endregion

        #region Momentum Space

        public double GetProbabilityMomentumSpace(double k)
        {
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (Precision - 1);
            return WaveFunctionMomentumSpace[EnergyLevel].GetMagnitudeSquared().Integrate(k - dk, k + dk);
        }

        public double ExpectedMomentum()
        {
            return MomentumSpaceDistributionParameters[EnergyLevel][0];
        }

        public double MeasureMomentum(int seed)
        {
            Random = new Random(seed);
            var mean = MomentumSpaceDistributionParameters[EnergyLevel][0];
            var std = MomentumSpaceDistributionParameters[EnergyLevel][1];
            var p = Normal.Sample(Random, mean, std);

            while (p <= MomentumDomain[0] || p >= MomentumDomain[1])
                p = Normal.Sample(Random, mean, std);
            
            return p;
        }

        #endregion
    }
}
