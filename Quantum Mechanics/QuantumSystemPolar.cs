using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Quantum_Mechanics.DE_Solver;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystemPolar
    {
        public DiscreteFunction2DComplex WaveFunction { get; private set; }
        public DiscreteFunction2D PositionSpaceProbabilityDensity { get; private set; }
  
        public Dictionary<Tuple<double, double>, double> PositionSpaceProbabilities { get; private set; }
        
        public double Energy { get; private set; }
        public double OrbitalAngularMomentum { get => AzimuthalLevel * (AzimuthalLevel + 1); }

        private int EnergyLevel;
        private int AzimuthalLevel;
        private int Precision;
        private double[,] PositionDomain;
        private Random Random;

        public QuantumSystemPolar(int precision, int energyLevel, int azimuthalLevel, double mass, string potential, double[,] positionDomain)
        {
            PositionDomain = positionDomain;
            Precision = precision;
            EnergyLevel = energyLevel;
            AzimuthalLevel = azimuthalLevel;
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

            var schrodingerEquation = new string[] { T.ToString(), T + "/(x^2 + 0,0001)", T + "/(x + 0,0001)", "0", V };

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

            var N = Math.Sqrt(1d / density.Integrate(positionDomain[0, 0], positionDomain[0, 1], positionDomain[1, 0], positionDomain[1, 1], true));

            WaveFunction = Interpolator.Bicubic(x, y, N * u);
            PositionSpaceProbabilityDensity = WaveFunction.GetMagnitudeSquared();
            PositionSpaceProbabilities = GetPositionSpaceProbabilityMap();
        }

        public void PlotPositionSpace()
        {
            WaveFunction.Plot(PositionDomain, "position_space.png", Precision);
        }

        #region Position Space

        private Dictionary<Tuple<double, double>, double> GetPositionSpaceProbabilityMap()
        {
            var n = Precision;
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (n - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (n - 1);
            var P = new Dictionary<Tuple<double, double>, double>();  

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    var x = PositionDomain[0, 0] + i * dx;
                    var y = PositionDomain[1, 0] + j * dy;

                    P.Add(Tuple.Create(x, y), PositionSpaceProbabilityDensity.Integrate(x - dx, x + dx, y - dy, y + dy, true));
                }
            }

            return P;
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

        public Tuple<double, double>[] MostProbablePositions()
        {
            var dx = (PositionDomain[0, 1] - PositionDomain[0, 0]) / (Precision - 1);
            var dy = (PositionDomain[1, 1] - PositionDomain[1, 0]) / (Precision - 1);
            var x = new double[Precision];
            var y = new double[Precision];
            var u = PositionSpaceProbabilities;

            var max = 0;
            var result = new List<Tuple<double, double>>();

            for (int i = 0; i < u.Count; ++i)
            {
                if (u.ElementAt(i).Value > u.ElementAt(max).Value)
                    max = i;
            }

            for (int i = 0; i < u.Count; ++i)
            {
                if (Math.Abs(u.ElementAt(i).Value - u.ElementAt(max).Value) <= 1e-6 + (1e-4 - 1e-6) / 11 * (EnergyLevel - 1))
                    result.Add(u.ElementAt(i).Key);
            }

            for (int i = 0; i < result.Count; ++i)
                result.RemoveAll(x => x != result[i] && Math.Abs(x.Item1 - result[i].Item1) <= 0.1 && Math.Abs(x.Item2 - result[i].Item2) <= 0.1);

            return result.ToArray();
        }

        public Tuple<double, double> MeasurePosition()
        {
            var P_sorted = PositionSpaceProbabilities.OrderByDescending(t => t.Value);
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

        public double MeasureAngularMomentum()
        {
            return Math.Sqrt(OrbitalAngularMomentum);
        }
    }
}
