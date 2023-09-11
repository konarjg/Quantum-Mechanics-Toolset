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
    public class WaveFunction2D
    {
        public float Energy { get; private set; }
        public Matrix<double> PositionDomain { get; private set; }
        public Matrix<double> MomentumDomain { get; private set; }

        private Matrix<Complex32> PositionSpaceValues;
        private Func<double, double, Complex32> PositionSpaceHandle;
        private Matrix<double> PositionSpaceProbabilities;

        private Matrix<Complex32> MomentumSpaceValues;
        private Func<double, double, Complex32> MomentumSpaceHandle;
        public Matrix<double> MomentumSpaceProbabilities;

        public WaveFunction2D(double mass, int energyLevel, string potential, Matrix<double> positionDomain)
        {
            PositionDomain = positionDomain;
            MomentumDomain = positionDomain;

            var T = "(0-" + (QuantumConstants.H * QuantumConstants.H / (2 * mass)) + ")";
            var equation = new string[] { T, T, "0", "0", potential };
            var boundaryConditions = new BoundaryConditionPDE[]
            {
                new BoundaryConditionPDE(0, 0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 0, positionDomain[0, 1].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 1].ToString(), "0"),
            };

            var solution = DESolver.SolveEigenvaluePDE(DifferenceScheme.CENTRAL, equation, boundaryConditions, positionDomain.ToArray(), 20);
            var U = solution.Values.ElementAt(energyLevel - 1);
            Energy = solution.Keys.ElementAt(energyLevel - 1).Real;

            var u = CreateMatrix.Sparse<double>(20, 20);
            var psi = CreateMatrix.Sparse<Complex32>(20, 20);

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    u[i, j] = U[20 * i + j].MagnitudeSquared;
                    psi[i, j] = U[20 * i + j];
                }
            }

            var N = 1 / DiscreteFunctions.DoubleIntegrate(PositionDomain, u, PositionDomain);
            u.Multiply(N);
            psi.Multiply((float)Math.Sqrt(N));

            PositionSpaceValues = psi;
            PositionSpaceProbabilities = u;
            PositionSpaceHandle = Interpolator.InterpolateComplex(positionDomain.ToArray(), psi, 20);

            MomentumSpaceHandle = GetMomentumSpace();
            MomentumSpaceValues = GetMomentumSpaceValues();
            MomentumSpaceProbabilities = GetProbabilityMapMomentumSpace();
        }

        #region Position Space

        public Func<double, double, Complex32> GetPositionSpace()
        {
            return PositionSpaceHandle;
        }

        public Matrix<Complex32> GetPositionSpaceValues()
        {
            return PositionSpaceValues;
        }

        public Matrix<double> GetProbabilityMapPositionSpace()
        {
            return PositionSpaceProbabilities;
        }

        #endregion

        #region Momentum Space
        public Func<double, double, Complex32> GetMomentumSpace()
        {
            var momentumSpace = CreateMatrix.SparseOfArray(PositionSpaceValues.ToArray());

            var x = CreateVector.Sparse<Complex32>(20);
            var y = CreateVector.Sparse<Complex32>(20);
            var kx = CreateVector.Sparse<Complex32>(20);
            var ky = CreateVector.Sparse<Complex32>(20);

            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

            for (int i = 0; i < 20; ++i)
            {
                x[i] = (float)PositionDomain[0, 0] + i * (float)dx;
                kx[i] = (float)PositionDomain[0, 0] + i * (float)dx;
                kx[i] *= 1 / (float)QuantumConstants.H;

                for (int j = 0; j < 20; ++j)
                {
                    y[j] = (float)PositionDomain[1, 0] + j * (float)dx;
                    ky[j] = (float)PositionDomain[1, 0] + j * (float)dx;
                    ky[j] *= 1 / (float)QuantumConstants.H;
                }
            }

            var fourier = DiscreteFunctions.Fourier2D(x, y, kx, ky, momentumSpace, 20);
            var values = CreateMatrix.Sparse<double>(20, 20);

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                    values[i, j] = fourier[i, j].MagnitudeSquared;
            }

            var k_real = CreateMatrix.SparseOfArray(new double[,] { { MomentumDomain[0, 0], MomentumDomain[0, 1] }, { MomentumDomain[1, 0], MomentumDomain[1, 1] } });

            var N = MathF.Sqrt(1f / (float)DiscreteFunctions.DoubleIntegrate(k_real, values, MomentumDomain));
            return Interpolator.InterpolateComplex(MomentumDomain.ToArray(), fourier * N, 200);
        }

        public Matrix<Complex32> GetMomentumSpaceValues()
        {
            var psi = MomentumSpaceHandle;
            var values = CreateMatrix.Sparse<Complex32>(20, 20);
            var dpx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / 19;
            var dpy = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / 19;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                    values[i, j] = psi.Invoke(MomentumDomain[0, 0] + i * dpx, MomentumDomain[1, 0] + j * dpy);
            }

            return values;
        }

        public Matrix<double> GetProbabilityMapMomentumSpace()
        {
            var map = CreateMatrix.Sparse<double>(20, 20);
            var values = MomentumSpaceValues;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                    map[i, j] = values[i, j].MagnitudeSquared;
            }

            return map;
        }

        #endregion
    }

    public class QuantumSystem2D
    {
        public double Mass { get; set; }
        public int EnergyLevel { get; set; }
        public string PotentialFunction { get; set; }
        public Matrix<double> PositionDomain { get; set; }
        public WaveFunction2D WaveFunction { get; set; }
        private Random Generator;

        public QuantumSystem2D(double mass, int energyLevel, string potentialFunction, Matrix<double> positionDomain)
        {
            Mass = mass;
            EnergyLevel = energyLevel;
            PotentialFunction = potentialFunction;
            PositionDomain = positionDomain;

            WaveFunction = new WaveFunction2D(Mass, EnergyLevel, PotentialFunction, PositionDomain);
            Generator = new Random();
        }

        public Matrix<Complex32> GetPlot()
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;
            var psi = CreateMatrix.Sparse<Complex32>(20, 20);

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;
                    psi[i, j] = WaveFunction.GetPositionSpace().Invoke(x, y);
                }
            }

            return psi;
        }

        public Func<double, double, double> GetProbabilityFunctionPositionSpace()
        {
            return (x, y) =>
            {
                var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
                var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

                if (x >= PositionDomain[0, 0] && x <= PositionDomain[0, 1] && y >= PositionDomain[1, 0] && y <= PositionDomain[1, 1])
                {
                    var map = WaveFunction.GetProbabilityMapPositionSpace();
                    var i = (x - PositionDomain[0, 0]) / dx;
                    var j = (y - PositionDomain[1, 0]) / dy;

                    return map[(int)i, (int)j];
                }

                throw new ArgumentException();
            };
        }

        public Func<double, double, double> GetProbabilityFunctionMomentumSpace()
        {
            return (kx, ky) =>
            {
                var dkx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
                var dky = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

                if (kx >= PositionDomain[0, 0] && kx <= PositionDomain[0, 1] && ky >= PositionDomain[1, 0] && ky <= PositionDomain[1, 1])
                {
                    var map = WaveFunction.GetProbabilityMapMomentumSpace();
                    var i = (kx - PositionDomain[0, 0]) / dkx;
                    var j = (ky - PositionDomain[1, 0]) / dky;

                    return map[(int)i, (int)j];
                }

                throw new ArgumentException();
            };
        }

        public Tuple<double, double> MaxProbabilityPoint()
        {
            var p = GetProbabilityFunctionPositionSpace();
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[0, 0]) / 19;

            var max_x = 0d;
            var max_y = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;

                    var P = p.Invoke(x, y);

                    if (P >= p.Invoke(max_x, max_y))
                    {
                        max_x = x;
                        max_y = y;
                    }
                }
            }

            return new Tuple<double, double>(max_x, max_y);
        }

        public Tuple<double, double> MaxProbabilityMomentum()
        {
            var p = GetProbabilityFunctionMomentumSpace();
            var dkx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dky = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;

            var max_kx = 0d;
            var max_ky = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var kx = PositionDomain[0, 0] + i * dkx;
                    var ky = PositionDomain[1, 0] + j * dky;

                    var P = p.Invoke(kx, ky);

                    if (P >= p.Invoke(max_kx, max_ky))
                    {
                        max_kx = kx;
                        max_ky = ky;
                    }
                }
            }

            return new Tuple<double, double>(max_kx, max_ky);
        }

        public Tuple<double, double> ExpectedPosition()
        {
            var P = WaveFunction.GetProbabilityMapPositionSpace();

            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;
            var x_exp = 0d;
            var y_exp = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;
                    x_exp += x * P[i, j];
                    y_exp += y * P[i, j];
                }
            }

            return new Tuple<double, double>(x_exp, y_exp);
        }

        public Tuple<double, double> ExpectedMomentum()
        {
            var P = WaveFunction.GetProbabilityMapMomentumSpace();

            var dkx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;
            var dky = (PositionDomain[1, 1] - PositionDomain[1, 0]) / 19;
            var kx_exp = 0d;
            var ky_exp = 0d;

            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    var kx = PositionDomain[0, 0] + i * dkx;
                    var ky = PositionDomain[1, 0] + j * dky;
                    kx_exp += kx * P[i, j];
                    ky_exp += ky * P[i, j];
                }
            }

            return new Tuple<double, double>(kx_exp, ky_exp);
        }

        public Tuple<double> MeasurePosition()
        {
            var map = CreateVector.Sparse<double>(200);
            var p = WaveFunction.GetProbabilityMapPositionSpace();
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / 19;

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
