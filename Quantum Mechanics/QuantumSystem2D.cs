using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
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
        public DiscreteFunction2DComplex WaveFunction { get; private set; }
        public DiscreteFunction2DComplex WaveFunctionMomentumSpace { get; private set; }
        public DiscreteFunction2D PositionSpaceProbabilityDensity { get; private set; }
        public DiscreteFunction2D MomentumSpaceProbabilityDensity { get; private set; }
        public MathNet.Numerics.LinearAlgebra.Matrix<double> PositionSpaceProbabilities { get; private set; }
        public MathNet.Numerics.LinearAlgebra.Matrix<double> MomentumSpaceProbabilities { get; private set; }
        public double Energy { get; private set; }
        public double OrbitalAngularMomentum { get => AzimuthalLevel * (AzimuthalLevel + 1); }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[,] PositionDomain;
        private double[,] MomentumDomain;
        private Random Random;

        public QuantumSystem2D(int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[,] positionDomain, double[,] momentumDomain)
        {
            PositionDomain = positionDomain;
            MomentumDomain = momentumDomain;
            Precision = precision;
            EnergyLevel = energyLevel;
            Random = new Random();

            var T = -1 / (2 * mass);
            var V = potential;

            var boundaryConditions = new BoundaryConditionPDE[]
            {
                new BoundaryConditionPDE(0, 0, positionDomain[0, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 0, positionDomain[0, 1].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 0].ToString(), "0"),
                new BoundaryConditionPDE(0, 1, positionDomain[1, 1].ToString(), "0")
            };

            var schrodingerEquation = new string[] { T.ToString(), T.ToString(), "0", "0", V };

            var solution = DESolver.SolveEigenvaluePDE(DifferenceScheme.CENTRAL, schrodingerEquation, boundaryConditions, positionDomain, precision);
            Energy = solution.Keys.ElementAt(energyLevel - 1).Real;

            var dx = (positionDomain[0, 1] - positionDomain[0, 0]) / (precision - 1);
            var dy = (positionDomain[1, 1] - positionDomain[1, 0]) / (precision - 1);
            var x = CreateVector.Sparse<double>(precision);
            var y = CreateVector.Sparse<double>(precision);
            var u_vector = solution.Values.ElementAt(energyLevel - 1);
            var u = CreateMatrix.Sparse<Complex>(Precision, Precision);

            for (int i = 0; i < precision; ++i)
            {
                for (int j = 0; j < precision; ++j)
                {
                    x[i] = positionDomain[0, 0] + i * dx;
                    y[j] = positionDomain[1, 0] + j * dy;
                    u[i, j] = u_vector[precision * i + j];
                }
            }

            WaveFunction = Interpolator.Bicubic(x, y, u);
            var density = WaveFunction.GetMagnitudeSquared();

            var N = Math.Sqrt(1d / density.Integrate(positionDomain[0, 0], positionDomain[0, 1], positionDomain[1, 0], positionDomain[1, 1]));

            WaveFunction = Interpolator.Bicubic(x, y, N * u);
            WaveFunctionMomentumSpace = WaveFunction.FourierTransform(positionDomain);
            density = WaveFunctionMomentumSpace.GetMagnitudeSquared();

            var Np = Math.Sqrt(1d / density.Integrate(momentumDomain[0, 0], momentumDomain[0, 1], momentumDomain[1, 0], momentumDomain[1, 1]));
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (Precision - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (Precision - 1);
            var kx = CreateVector.Sparse<double>(Precision);
            var ky = CreateVector.Sparse<double>(Precision);
            var k = CreateMatrix.Sparse<Complex>(Precision, Precision);

            for (int i = 0; i < Precision; ++i)
            {
                for (int j = 0; j < Precision; ++j)
                {
                    kx[i] = MomentumDomain[0, 0] + i * dkx;
                    ky[j] = MomentumDomain[1, 0] + j * dky;

                    k[i, j] = Np * WaveFunctionMomentumSpace.Evaluate(kx[i], ky[j]);
                }
            }

            WaveFunctionMomentumSpace = Interpolator.Bicubic(kx, ky, k);
            PositionSpaceProbabilityDensity = WaveFunction.GetMagnitudeSquared();
            MomentumSpaceProbabilityDensity = WaveFunctionMomentumSpace.GetMagnitudeSquared();
            PositionSpaceProbabilities = GetPositionSpaceProbabilityMap();
            MomentumSpaceProbabilities = GetMomentumSpaceProbabilityMap();
        }

        #region Position Space

        private MathNet.Numerics.LinearAlgebra.Matrix<double> GetPositionSpaceProbabilityMap()
        {
            var n = Precision;
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (n - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (n - 1);

            var x = CreateVector.Sparse<double>(n);
            var y = CreateVector.Sparse<double>(n);
            var p = CreateMatrix.Sparse<double>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    x[i] = PositionDomain[0, 0] + i * dx;
                    y[j] = PositionDomain[1, 0] + j * dy;
                    p[i, j] = PositionSpaceProbabilityDensity.Integrate(x[i] - dx, x[i] + dx, y[j] - dy, y[j] + dy);
                }
            }

            return p;
        }

        public double GetProbabilityPositionSpace(double x, double y)
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (Precision - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(x - dx, x + dx, y - dy, y + dy);
        }

        public Tuple<double, double> ExpectedPosition()
        {
            var fx = new DiscreteFunction2D(new Func<double, double, double>((x, y) => x * PositionSpaceProbabilityDensity.Evaluate(x, y)));
            var fy = new DiscreteFunction2D(new Func<double, double, double>((x, y) => y * PositionSpaceProbabilityDensity.Evaluate(x, y)));

            var exp_x = fx.Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);
            var exp_y = fy.Integrate(PositionDomain[0, 0], PositionDomain[0, 1], PositionDomain[1, 0], PositionDomain[1, 1]);
            
            return Tuple.Create(exp_x, exp_y);
        }

        public Tuple<double, double>[] MostProbablePositions()
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (Precision - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (Precision - 1);
            var x = new double[Precision];
            var y = new double[Precision];
            var u = PositionSpaceProbabilities;

            var max_x = 0;
            var max_y = 0;
            var result = new List<Tuple<double, double>>();

            for (int i = 0; i < Precision; ++i)
            {
                for (int j = 0; j < Precision; ++j)
                {
                    x[i] = PositionDomain[0, 0] + i * dx;
                    y[j] = PositionDomain[1, 0] + j * dy;

                    if (u[i, j] > u[max_x, max_y])
                    {
                        max_x = i;
                        max_y = j;
                    }
                }
            }

            for (int i = 0; i < Precision; ++i)
            {
                for (int j = 0; j < Precision; ++j)
                {
                    if (Math.Abs(u[i, j] - u[max_x, max_y]) <= 1e-6 + (1e-4 - 1e-6) / 11 * (EnergyLevel - 1))
                        result.Add(Tuple.Create(x[i], y[j]));
                }
            }

            for (int i = 0; i < result.Count; ++i)
                result.RemoveAll(x => x != result[i] && Math.Abs(x.Item1 - result[i].Item1) <= 0.1 && Math.Abs(x.Item2 - result[i].Item2) <= 0.1);

            return result.ToArray();
        }

        public Tuple<double, double> MeasurePosition()
        {
            var n = Precision;
            var x = new double[n];
            var y = new double[n];
            var p = PositionSpaceProbabilities;
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (n - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (n - 1);

            var P = new Dictionary<Tuple<double, double>, double>();

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    x[i] = PositionDomain[0, 0] + i * dx;
                    y[j] = PositionDomain[1, 0] + j * dy;
                    P.Add(Tuple.Create(x[i], y[j]), p[i, j]);
                }
            }

            var P_sorted = P.OrderByDescending(t => t.Value);
            var s = new Dictionary<Tuple<double, double>, double>();
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

        private MathNet.Numerics.LinearAlgebra.Matrix<double> GetMomentumSpaceProbabilityMap()
        {
            var n = Precision;
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (n - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (n - 1);

            var kx = CreateVector.Sparse<double>(n);
            var ky = CreateVector.Sparse<double>(n);
            var p = CreateMatrix.Sparse<double>(n, n);

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    kx[i] = MomentumDomain[0, 0] + i * dkx;
                    ky[j] = MomentumDomain[1, 0] + j * dky;
                    p[i, j] = MomentumSpaceProbabilityDensity.Integrate(kx[i] - dkx, kx[i] + dkx, ky[j] - dky, ky[j] + dky);
                }
            }

            return p;
        }

        public double GetProbabilityMomentumSpace(double kx, double ky)
        {
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (Precision - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (Precision - 1);
            return PositionSpaceProbabilityDensity.Integrate(kx - dkx, kx + dkx, ky - dky, ky + dky);
        }

        public Tuple<double, double> ExpectedMomentum()
        {
            var fkx = new DiscreteFunction2D(new Func<double, double, double>((kx, ky) => kx * MomentumSpaceProbabilityDensity.Evaluate(kx, ky)));
            var fky = new DiscreteFunction2D(new Func<double, double, double>((kx, ky) => ky * MomentumSpaceProbabilityDensity.Evaluate(kx, ky)));

            var exp_kx = fkx.Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1]);
            var exp_ky = fky.Integrate(MomentumDomain[0, 0], MomentumDomain[0, 1], MomentumDomain[1, 0], MomentumDomain[1, 1]);

            return Tuple.Create(exp_kx, exp_ky);
        }

        public double[] MostProbableMomenta()
        {
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (Precision - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (Precision - 1);
            var kx = new double[Precision];
            var ky = new double[Precision];
            var u = PositionSpaceProbabilities;

            var max_kx = 0;
            var max_ky = 0;
            var result = new List<double>();

            for (int i = 0; i < Precision; ++i)
            {
                for (int j = 0; j < Precision; ++j)
                {
                    kx[i] = MomentumDomain[0, 0] + i * dkx;
                    ky[j] = MomentumDomain[1, 0] + j * dky;

                    if (u[i, j] > u[max_kx, max_ky])
                    {
                        max_kx = i;
                        max_ky = j;
                    }
                }
            }

            for (int i = 0; i < Precision; ++i)
            {
                for (int j = 0; j < Precision; ++j)
                {
                    if (Math.Abs(u[i, j] - u[max_kx, max_ky]) <= 1e-6 + (1e-4 - 1e-6) / 11 * (EnergyLevel - 1))
                        result.Add(Math.Sqrt(kx[i] * kx[i] + ky[j] * ky[j]));
                }
            }

            for (int i = 0; i < result.Count; ++i)
                result.RemoveAll(x => x != result[i] && Math.Abs(x - result[i]) <= 0.1);

            return result.ToArray();
        }

        public double MeasureMomentum()
        {
            var n = Precision;
            var kx = new double[n];
            var ky = new double[n];
            var p = MomentumSpaceProbabilities;
            var dkx = (MomentumDomain[0, 1] - MomentumDomain[0, 0]) / (n - 1);
            var dky = (MomentumDomain[1, 1] - MomentumDomain[1, 0]) / (n - 1);

            var P = new Dictionary<Tuple<double, double>, double>();

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    kx[i] = MomentumDomain[0, 0] + i * dkx;
                    ky[j] = MomentumDomain[1, 0] + j * dky;
                    P.Add(Tuple.Create(kx[i], ky[j]), p[i, j]);
                }
            }

            var P_sorted = P.OrderByDescending(t => t.Value);
            var s = new Dictionary<Tuple<double, double>, double>();
            s.Add(P_sorted.ElementAt(0).Key, P_sorted.ElementAt(0).Value);

            for (int i = 1; i < P_sorted.Count(); ++i)
                s.Add(P_sorted.ElementAt(i).Key, s.ElementAt(i - 1).Value + P_sorted.ElementAt(i).Value);

            var u = Random.NextDouble() * s.Max(x => x.Value);

            for (int i = 0; i < s.Count; ++i)
            {
                if (u < s.ElementAt(i).Value)
                {
                    var k = s.ElementAt(i).Key;

                    return Math.Sqrt(k.Item1 * k.Item1 + k.Item2 * k.Item2);
                }
            }

            throw new ArgumentException();
        }

        public void PlotPositionSpace()
        {
            WaveFunction.Plot(PositionDomain, "position_space.png", 20);
        }

        public void PlotMomentumSpace(double[] magnitudeDomain)
        {
            var n = Precision * Precision;
            var dk = (magnitudeDomain[1] - magnitudeDomain[0]) / (n - 1);
            var k = CreateVector.Sparse<double>(n);
            var p = CreateVector.Sparse<double>(n);

            for (int i = 0; i < n; ++i)
            {
                k[i] = magnitudeDomain[0] + i * dk;
                p[i] = MomentumSpaceProbabilityDensity.IntegratePolar(k[i] - dk, k[i] + dk, 0, Math.PI * 2);
            }

            var f = Interpolator.Cubic(k, p);
            f.Plot(magnitudeDomain, "momentum_space.png", n);
        }

        #endregion
    }
}
