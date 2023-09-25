using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystem1D
    {
        public DiscreteFunctionComplex WaveFunction { get; private set; }
        public DiscreteFunctionComplex WaveFunctionMomentumSpace { get; private set; }
        public DiscreteFunction PositionSpaceProbabilityDensity { get; private set; }
        public DiscreteFunction MomentumSpaceProbabilityDensity { get; private set; }
        public MathNet.Numerics.LinearAlgebra.Vector<double> PositionSpaceProbabilities { get; private set; }
        public MathNet.Numerics.LinearAlgebra.Vector<double> MomentumSpaceProbabilities { get; private set; }
        public double Energy { get; private set; }
        public double OrbitalAngularMomentum { get => AzimuthalLevel * (AzimuthalLevel + 1); }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[] PositionDomain;
        private double[] MomentumDomain;
        private Random Random;

        public QuantumSystem1D(int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[] positionDomain, double[] momentumDomain)
        {
            PositionDomain = positionDomain;
            MomentumDomain = momentumDomain;
            Precision = precision;
            EnergyLevel = energyLevel;
            Random = new Random();

            var T = -1 / (2 * mass);
            var V = potential;

            var boundaryConditions = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[1].ToString(), "0")
            };

            var schrodingerEquation = new string[] { T.ToString(), "0", V };

            var solution = DESolver.SolveEigenvalueODE(DifferenceScheme.CENTRAL, schrodingerEquation, boundaryConditions, positionDomain, precision);
            Energy = solution.Keys.ElementAt(energyLevel - 1).Real;

            var dx = (positionDomain[1] - positionDomain[0]) / (precision - 1);
            var x = CreateVector.Sparse<double>(precision);
            var y = solution.Values.ElementAt(energyLevel - 1);

            for (int i = 0; i < precision; ++i)
                x[i] = positionDomain[0] + i * dx;

            WaveFunction = Interpolator.Cubic(x, y);
            var density = WaveFunction.GetMagnitudeSquared();

            var N = Math.Sqrt(1d / density.Integrate(positionDomain[0], positionDomain[1]));

            WaveFunction = Interpolator.Cubic(x, N * y);
            WaveFunctionMomentumSpace = WaveFunction.FourierTransform(positionDomain);
            density = WaveFunctionMomentumSpace.GetMagnitudeSquared();

            var Np = Math.Sqrt(1d / density.Integrate(momentumDomain[0], momentumDomain[1]));
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (Precision - 1);
            var k = CreateVector.Sparse<double>(Precision);
            var p = CreateVector.Sparse<Complex>(Precision);

            for (int i = 0; i < Precision; ++i)
            {
                k[i] = MomentumDomain[0] + i * dk;
                p[i] = Np * WaveFunctionMomentumSpace.Evaluate(k[i]);
            }

            WaveFunctionMomentumSpace = Interpolator.Cubic(k, p);
            PositionSpaceProbabilityDensity = WaveFunction.GetMagnitudeSquared();
            MomentumSpaceProbabilityDensity = WaveFunctionMomentumSpace.GetMagnitudeSquared();
            PositionSpaceProbabilities = GetPositionSpaceProbabilityMap();
            MomentumSpaceProbabilities = GetMomentumSpaceProbabilityMap();
        }

        #region Position Space

        private MathNet.Numerics.LinearAlgebra.Vector<double> GetPositionSpaceProbabilityMap()
        {
            var n = Precision;
            var dx = (PositionDomain[1] - PositionDomain[0]) / (n - 1);

            var x = CreateVector.Sparse<double>(n);
            var p = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                x[i] = PositionDomain[0] + i * dx;
                p[i] = PositionSpaceProbabilityDensity.Integrate(x[i] - dx, x[i] + dx);
            }

            return p;
        }

        public double GetProbabilityPositionSpace(double x)
        {
            var dx = (PositionDomain[1] - PositionDomain[0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(x - dx, x + dx);
        }

        public double ExpectedPosition()
        {
            var f = new Func<double, double>(x => x * PositionSpaceProbabilityDensity.Evaluate(x));

            return new DiscreteFunction(f).Integrate(PositionDomain[0], PositionDomain[1]);
        }

        public double[] MostProbablePositions()
        {
            var dx = (PositionDomain[1] - PositionDomain[0]) / (Precision - 1);
            var x = new double[Precision];
            var y = PositionSpaceProbabilities;

            var max = 0;
            var result = new List<double>();

            for (int i = 0; i < Precision; ++i)
            {
                x[i] = PositionDomain[0] + i * dx;

                if (y[i] > y[max])
                    max = i;
            }

            for (int i = 0; i < Precision; ++i)
            {
                if (Math.Abs(y[i] - y[max]) <= 1e-6 + (1e-4 - 1e-6) / 11 * (EnergyLevel - 1))
                    result.Add(x[i]);
            }

            for (int i = 0; i < result.Count; ++i)
                result.RemoveAll(x => x != result[i] && Math.Abs(x - result[i]) <= 0.1);

            return result.ToArray();
        }

        public double MeasurePosition()
        {
            var n = Precision;
            var x = new double[n];
            var p = PositionSpaceProbabilities;
            var dx = (PositionDomain[1] - PositionDomain[0]) / (n - 1);

            var P = new Dictionary<double, double>();

            for (int i = 0; i < n; ++i)
            {
                x[i] = PositionDomain[0] + i * dx;
                P.Add(x[i], p[i]);
            }

            var P_sorted = P.OrderByDescending(t => t.Value);
            var s = new Dictionary<double, double>();
            s.Add(P_sorted.ElementAt(0).Key, P_sorted.ElementAt(0).Value);

            for (int i = 1; i < P_sorted.Count(); ++i)
                s.Add(P_sorted.ElementAt(i).Key, s.ElementAt(i - 1).Value + P_sorted.ElementAt(i).Value);

            var u = Random.NextDouble() * s.Max(x => x.Value);

            for (int i = 0; i < s.Count; ++i)
            {
                if (u < s.ElementAt(i).Value)
                    return s.ElementAt(i).Key;
            }

            throw new ArgumentException();
        }

        #endregion

        #region Momentum Space

        private MathNet.Numerics.LinearAlgebra.Vector<double> GetMomentumSpaceProbabilityMap()
        {
            var n = Precision;
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (n - 1);

            var k = CreateVector.Sparse<double>(n);
            var p = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                k[i] = MomentumDomain[0] + i * dk;
                p[i] = MomentumSpaceProbabilityDensity.Integrate(k[i] - dk, k[i] + dk);
            }

            return p;
        }

        public double GetProbabilityMomentumSpace(double k)
        {
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(k - dk, k + dk);
        }

        public double ExpectedMomentum()
        {
            var f = new Func<double, double>(k => k * MomentumSpaceProbabilityDensity.Evaluate(k));

            return new DiscreteFunction(f).Integrate(MomentumDomain[0], MomentumDomain[1]);
        }

        public double[] MostProbableMomenta()
        {
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (Precision - 1);
            var k = new double[Precision];
            var y = MomentumSpaceProbabilities;

            var max = 0;
            var result = new List<double>();

            for (int i = 0; i < Precision; ++i)
            {
                k[i] = MomentumDomain[0] + i * dk;

                if (y[i] > y[max])
                    max = i;
            }

            for (int i = 0; i < Precision; ++i)
            {
                if (Math.Abs(y[i] - y[max]) <= 1e-6 + (1e-4 - 1e-6) / 11 * (EnergyLevel - 1))
                    result.Add(k[i]);
            }

            for (int i = 0; i < result.Count; ++i)
                result.RemoveAll(k => k != result[i] && Math.Abs(k - result[i]) <= 0.1);

            return result.ToArray();
        }

        public double MeasureMomentum()
        {
            var n = Precision;
            var k = new double[n];
            var p = MomentumSpaceProbabilities;
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / (n - 1);

            var P = new Dictionary<double, double>();

            for (int i = 0; i < n; ++i)
            {
                k[i] = MomentumDomain[0] + i * dk;
                P.Add(k[i], p[i]);
            }

            var P_sorted = P.OrderByDescending(t => t.Value);
            var s = new Dictionary<double, double>();
            s.Add(P_sorted.ElementAt(0).Key, P_sorted.ElementAt(0).Value);

            for (int i = 1; i < P_sorted.Count(); ++i)
                s.Add(P_sorted.ElementAt(i).Key, s.ElementAt(i - 1).Value + P_sorted.ElementAt(i).Value);

            var u = Random.NextDouble() * s.Max(x => x.Value);

            for (int i = 0; i < s.Count; ++i)
            {
                if (u < s.ElementAt(i).Value)
                    return s.ElementAt(i).Key;
            }

            throw new ArgumentException();
        }

        #endregion
    }
}
