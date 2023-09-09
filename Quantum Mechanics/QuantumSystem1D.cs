using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics;
using System.Data.Common;
using MathNet.Numerics.Providers.FourierTransform;
using Quantum_Mechanics.General;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class WaveFunction1D
    {
        public float Energy { get; private set; }
        public Vector<double> PositionDomain { get; private set; }
        public Vector<double> MomentumDomain { get; private set; }

        private Vector<Complex32> PositionSpaceValues;
        private Func<double, Complex32> PositionSpaceHandle;
        private Vector<double> PositionSpaceProbabilities;

        private Vector<Complex32> MomentumSpaceValues;
        private Func<double, Complex32> MomentumSpaceHandle;

        public WaveFunction1D(double mass, int energyLevel, string potential, Vector<double> positionDomain)
        {
            PositionDomain = positionDomain;
            MomentumDomain = positionDomain;

            var T = "(0-" + (QuantumConstants.H * QuantumConstants.H / (2 * mass)) + ")";
            var equation = new string[] { T, "0", potential };
            var boundaryConditions = new BoundaryCondition[]
            {
                new BoundaryCondition(0, positionDomain[0].ToString(), "0"),
                new BoundaryCondition(0, positionDomain[1].ToString(), "0")
            };

            var solution = DESolver.SolveEigenvalueODE(DifferenceScheme.CENTRAL, equation, boundaryConditions, positionDomain.ToArray(), 200);
            var U = solution.Values.ElementAt(energyLevel - 1);
            Energy = solution.Keys.ElementAt(energyLevel - 1).Real;
 
            var u = CreateVector.Sparse<double>(200);
            var psi = CreateVector.Sparse<Complex32>(200);

            for (int i = 0; i < 200; ++i)
            {
                u[i] = U[i].MagnitudeSquared;
                psi[i] = U[i];
            }

            var N = 1 / DiscreteFunctions.Integrate(PositionDomain, u, PositionDomain.ToArray());
            u.Multiply(N);
            psi.Multiply((float)Math.Sqrt(N));

            PositionSpaceValues = psi;
            PositionSpaceProbabilities = u;
            PositionSpaceHandle = Interpolator.InterpolateComplex(positionDomain.ToArray(), psi, 200);

            MomentumSpaceHandle = GetMomentumSpace();
            MomentumSpaceValues = GetMomentumSpaceValues();
        }

        #region Position Space

        public Func<double, Complex32> GetPositionSpace()
        {
            return PositionSpaceHandle;
        }

        public Vector<Complex32> GetPositionSpaceValues()
        {
            return PositionSpaceValues;
        }

        public Vector<double> GetProbabilityMapPositionSpace()
        {
            return PositionSpaceProbabilities;
        }

        #endregion

        #region Momentum Space
        public Func<double, Complex32> GetMomentumSpace()
        {
            var momentumSpace = CreateVector.SparseOfArray(PositionSpaceValues.ToArray());

            var x = CreateVector.Sparse<Complex32>(200);
            var k = CreateVector.Sparse<Complex32>(200);
            var k_real = CreateVector.Sparse<double>(200);

            var dx = (PositionDomain[1] - PositionDomain[0]) / 199;
            var dk = (MomentumDomain[1] - MomentumDomain[0]) / 199;

            for (int i = 0; i < 200; ++i)
            {
                x[i] = (float)PositionDomain[0] + i * (float)dx;
                k[i] = (float)MomentumDomain[0] + i * (float)dk;
                k_real[i] = MomentumDomain[0] + i * dk;
            }

            var fourier = DiscreteFunctions.Fourier(x, k, momentumSpace, 200);
            var values = CreateVector.Sparse<double>(200);

            for (int i = 0; i < 200; ++i)
                values[i] = fourier[i].MagnitudeSquared;

            var N = MathF.Sqrt(1f / (float)DiscreteFunctions.Integrate(k_real, values, MomentumDomain.ToArray()));
            return Interpolator.InterpolateComplex(MomentumDomain.ToArray(), fourier * N, 200);
        }

        public Vector<Complex32> GetMomentumSpaceValues()
        {
            var psi = MomentumSpaceHandle;
            var values = CreateVector.Sparse<Complex32>(200);
            var dp = (MomentumDomain[1] - MomentumDomain[0]) / 199;

            for (int i = 0; i < 200; ++i)
                values[i] = psi.Invoke(MomentumDomain[0] + i * dp);
    
            return values;
        }

        public Vector<double> GetProbabilityMapMomentumSpace()
        {
            var map = CreateVector.Sparse<double>(200);
            var values = MomentumSpaceValues;

            for (int i = 0; i < 200; ++i)
                map[i] = values[i].MagnitudeSquared;

            return map;
        }

        #endregion
    }

    public class QuantumSystem1D
    {
        public double Mass { get; set; }
        public int EnergyLevel { get; set; }
        public string PotentialFunction { get; set; }
        public Vector<double> PositionDomain { get; set; }
        public WaveFunction1D WaveFunction { get; set; }
        private Random Generator;

        public QuantumSystem1D(double mass, int energyLevel, string potentialFunction, Vector<double> positionDomain) 
        {
            Mass = mass;
            EnergyLevel = energyLevel;
            PotentialFunction = potentialFunction;
            PositionDomain = positionDomain;

            WaveFunction = new WaveFunction1D(Mass, EnergyLevel, PotentialFunction, PositionDomain);
            Generator = new Random();
        }

        public Vector<Complex32> GetPlot()
        {
            var dx = (PositionDomain[1] - PositionDomain[0]) / 199;
            var psi = CreateVector.Sparse<Complex32>(200);

            for (int i = 0; i < 200; ++i)
            {
                var x = PositionDomain[0] + i * dx;
                psi[i] = WaveFunction.GetPositionSpace().Invoke(x);
            }

            return psi;
        }

        public Func<double, double> GetProbabilityFunctionPositionSpace()
        {
            return x =>
            {
                var dx = (PositionDomain[1] - PositionDomain[0]) / 199;

                if (x >= PositionDomain[0] && x <= PositionDomain[1])
                {
                    var map = WaveFunction.GetProbabilityMapPositionSpace();
                    var i = (x - PositionDomain[0]) / dx;

                    return map[(int)i];
                }

                throw new ArgumentException();
            };
        }

        public Func<double, double> GetProbabilityFunctionMomentumSpace()
        {
            return k =>
            {
                var dk = (PositionDomain[1] - PositionDomain[0]) / 199;

                if (k >= PositionDomain[0] && k <= PositionDomain[1])
                {
                    var map = WaveFunction.GetProbabilityMapMomentumSpace();
                    var i = (k - PositionDomain[0]) / dk;

                    return map[(int)i];
                }

                throw new ArgumentException();
            };
        }

        public double MaxProbabilityPoint()
        {
            var p = GetProbabilityFunctionPositionSpace();
            var dx = (PositionDomain[1] - PositionDomain[0]) / 199;

            var max_x = 0d;

            for (int i = 0; i < 200; ++i)
            {
                var x = PositionDomain[0] + i * dx;

                var P = p.Invoke(x);

                if (P >= p.Invoke(max_x))
                    max_x = x;
            }

            return max_x;
        }

        public double MaxProbabilityMomentum()
        {
            var p = GetProbabilityFunctionMomentumSpace();
            var dk = (PositionDomain[1] - PositionDomain[0]) / 199;

            var max_k = 0d;

            for (int i = 0; i < 200; ++i)
            {
                var k = PositionDomain[0] + i * dk;

                var P = p.Invoke(k);

                if (P >= p.Invoke(max_k))
                    max_k = k;
            }

            return max_k;
        }

        public double ExpectedPosition()
        {
            var P = WaveFunction.GetProbabilityMapPositionSpace();

            var dx = (PositionDomain[1] - PositionDomain[0]) / 199;
            var x_exp = 0d;

            for (int i = 0; i < 200; ++i)
            {
                var x = PositionDomain[0] + i * dx;
                x_exp += x * P[i];
            }

            return x_exp;
        }

        public double ExpectedMomentum()
        {
            var P = WaveFunction.GetProbabilityMapMomentumSpace();

            var dk = (PositionDomain[1] - PositionDomain[0]) / 199;
            var k_exp = 0d;

            for (int i = 0; i < 200; ++i)
            {
                var k = PositionDomain[0] + i * dk;
                k_exp += k * P[i];
            }

            return k_exp;
        }

        public double MeasurePosition()
        {
            var map = CreateVector.Sparse<double>(200);
            var p = WaveFunction.GetProbabilityMapPositionSpace();
            var dx = (PositionDomain[1] - PositionDomain[0]) / 199;

            var x = CreateVector.Sparse<double>(200);
            var P = new Dictionary<double, double>();

            for (int i = 0; i < 200; ++i)
            {
                x[i] = PositionDomain[0] + i * dx;
                map[i] = p[i];

                P.Add(x[i], map[i]);
            }

            var sorted = P.OrderByDescending(t => t.Value);
            var S = CreateVector.Sparse<double>(200);

            S[0] = sorted.ElementAt(0).Value;

            for (int i = 1; i < 200; ++i)
                S[i] = sorted.ElementAt(i).Value + S[i - 1];

            var s = Generator.NextDouble() * S.Max();

            for (int i = 0; i < 200; ++i)
            {
                if (s < S[i])
                    return sorted.ElementAt(i).Key;
            }

            throw new ArgumentException();
        }

        public double MeasureMomentum()
        {
            var map = CreateVector.Sparse<double>(200);
            var p = WaveFunction.GetProbabilityMapMomentumSpace();
            var dk = (PositionDomain[1] - PositionDomain[0]) / 199;

            var k = CreateVector.Sparse<double>(200);
            var P = new Dictionary<double, double>();

            for (int i = 0; i < 200; ++i)
            {
                k[i] = PositionDomain[0] + i * dk;
                map[i] = p[i];

                P.Add(k[i], map[i]);
            }

            var sorted = P.OrderByDescending(t => t.Value);
            var S = CreateVector.Sparse<double>(200);

            S[0] = sorted.ElementAt(0).Value;

            for (int i = 1; i < 200; ++i)
                S[i] = sorted.ElementAt(i).Value + S[i - 1];

            var s = Generator.NextDouble() * S.Max();

            for (int i = 0; i < 200; ++i)
            {
                if (s < S[i])
                     return sorted.ElementAt(i).Key;
            }

            throw new ArgumentException();
        }
    }
}
