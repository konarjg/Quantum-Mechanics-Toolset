using Accord.Math;
using FEM;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using Python.Runtime;
using Quantum_Mechanics.Differential_Equations_Solver;
using Quantum_Sandbox;
using Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Generate = MathNet.Numerics.Generate;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public class QuantumSystem1D
    {
        private bool Spherical;
        public double[] Wavefunction { get; private set; }
        public List<(double, double)> PositionMeasurements = new List<(double, double)>();
        public List<(double, double)> MomentumMeasurements = new List<(double, double)>();
        public Complex[] WavefunctionMomentum { get; private set; }
        public double[] Energies { get; private set; }
        public double[] Grid { get; private set; }
        public double[] MomentumGrid { get; private set; }

        public double Energy { get => Energies[EnergyLevel];}

        private int EnergyLevel;
        public int AzimuthalLevel { get; private set; }
        private int Precision;

        private Random Random;

        private void GeneratePositionMeasurements()
        {
            var p = new List<(double, double)>();
            var PDF = Wavefunction.Pow(2);

            for (int i = 0; i < PDF.Length; ++i)
                p.Add((PDF[i], Grid[i]));

            p = p.OrderByDescending(p => p.Item1).ToList();
            var s = new double[p.Count];
            s[0] = p[0].Item1 * (Grid[1] - Grid[0]);

            if (Spherical)
                s[0] = p[0].Item1 * 4 * Math.PI * (Grid[1] - Grid[0]);

            for (int i = 1; i < p.Count; ++i)
            {
                s[i] = s[i - 1] + p[i].Item1 * (Grid[1] - Grid[0]);

                if (Spherical)
                    s[i] = s[i - 1] + p[i].Item1 * 4 * Math.PI * (Grid[1] - Grid[0]);
            }

            for (int i = 0; i < p.Count; ++i)
                PositionMeasurements.Add((s[i], p[i].Item2));
        }

        private void GenerateMomentumMeasurements()
        {
            var p = new List<(double, double)>();
            var PDF = WavefunctionMomentum.Magnitude().Pow(2);

            for (int i = 0; i < PDF.Length; ++i)
                p.Add((PDF[i], MomentumGrid[i]));

            p = p.OrderByDescending(p => p.Item1).ToList();
            var s = new double[p.Count];
            s[0] = p[0].Item1 * (MomentumGrid[1] - MomentumGrid[0]);

            if (Spherical)
                s[0] = p[0].Item1 * 4 * Math.PI * (MomentumGrid[1] - MomentumGrid[0]);

            for (int i = 1; i < p.Count; ++i)
            {
                s[i] = s[i - 1] + p[i].Item1 * (MomentumGrid[1] - MomentumGrid[0]);

                if (Spherical)
                    s[i] = s[i - 1] + p[i].Item1 * 4 * Math.PI * (MomentumGrid[1] - MomentumGrid[0]);
            }

            for (int i = 0; i < p.Count; ++i)
                MomentumMeasurements.Add((s[i], p[i].Item2));
        }

        private void CalculateMomentumSpaceWavefunction()
        {
            var psi_x = CreateVector.SparseOfArray(Wavefunction).ToComplex();

            Func<double, Complex> F = p =>
            {
                var k = CreateVector.SparseOfArray(Grid).ToComplex().Multiply(-Complex.ImaginaryOne * p);
                var F = 1 / Math.Sqrt(2 * Math.PI) * k.PointwiseExp().PointwiseMultiply(psi_x);

                if (Spherical)
                    return F.Sum() * 4 * Math.PI * (Grid[1] - Grid[0]);

                return F.Sum() * (Grid[1] - Grid[0]);
            };

            var psi_p = new Complex[MomentumGrid.Length];

            for (int j = 0; j < MomentumGrid.Length; ++j)
                psi_p[j] = F(MomentumGrid[j]);

            var N = Math.Sqrt(1d / (psi_p.Magnitude().Pow(2).Sum() * (MomentumGrid[1] - MomentumGrid[0])));
            
            if (Spherical)
                N = Math.Sqrt(1d / (psi_p.Magnitude().Pow(2).Sum() * 4 * Math.PI * (MomentumGrid[1] - MomentumGrid[0])));

            WavefunctionMomentum = CreateVector.SparseOfArray(psi_p).Multiply(N).ToArray();
        }

        public QuantumSystem1D(CancellationToken token, double[] energies, double[][] wavefunctions, double[] grid, int precision, int energyLevel, int azimuthalLevel, bool spherical = false)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            Precision = precision;
            EnergyLevel = energyLevel - 1;
            AzimuthalLevel = azimuthalLevel;
            Spherical = spherical;

            token.ThrowIfCancellationRequested();

            Energies = energies;
            Wavefunction = wavefunctions[EnergyLevel];
            Grid = grid;
            MomentumGrid = Generate.LinearSpaced(Grid.Length, -3, 3);

            token.ThrowIfCancellationRequested();

            CalculateMomentumSpaceWavefunction();
            GeneratePositionMeasurements();
            GenerateMomentumMeasurements();
        }

        public void PlotPositionSpace(FormsPlot plot)
        {
            var x = Grid;
            var u = new double[Precision];

            for (int i = 0; i < Precision; ++i)
                u[i] = Wavefunction[i] * Wavefunction[i];

            plot.Plot.SetAxisLimits(Grid[0], Grid.Last(), u.Min(), u.Max());
            plot.Plot.AddSignalXY(x, u);
            plot.Refresh();
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            var k = MomentumGrid;
            var u = new double[Precision];

            for (int i = 0; i < Precision; ++i)
                u[i] = WavefunctionMomentum[i].MagnitudeSquared();

            plot.Plot.SetAxisLimits(MomentumGrid[0], MomentumGrid.Last(), u.Min(), u.Max());
            plot.Plot.AddSignalXY(k, u);
            plot.Refresh();
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

            return Tuple.Create(x + RandomSign() * Random.NextDouble() * 0.5, p);
        }

        public double MeasureAngularMomentum()
        {
            return Math.Sqrt(AzimuthalLevel * (AzimuthalLevel + 1));
        }

        #region Position Space

        public double ExpectedPosition()
        {
            if (Spherical)
                return Wavefunction.Pow(2).Multiply(Grid).Sum() * 4 * Math.PI * (Grid[1] - Grid[0]);

            return Wavefunction.Pow(2).Multiply(Grid).Sum() * (Grid[1] - Grid[0]);
        }

        public double MeasurePosition(int seed)
        {
            Random = new Random(seed);

            var u = Random.NextDouble();

            for (int i = 0; i < PositionMeasurements.Count; ++i)
            {
                if (u <= PositionMeasurements[i].Item1)
                    return PositionMeasurements[i].Item2;
            }

            throw new ArgumentException();
        }

        #endregion

        #region Momentum Space

        public double ExpectedMomentum()
        {
            var u = new double[Precision];

            for (int i = 0; i < Precision; ++i)
                u[i] = WavefunctionMomentum[i].MagnitudeSquared();

            if (Spherical)
                return u.Multiply(MomentumGrid).Sum() * 4 * Math.PI * (MomentumGrid[1] - MomentumGrid[0]);

            return u.Multiply(MomentumGrid).Sum() * (MomentumGrid[1] - MomentumGrid[0]);
        }

        public double MeasureMomentum(int seed)
        {
            Random = new Random(seed);

            var u = Random.NextDouble();

            for (int i = 0; i < MomentumMeasurements.Count; ++i)
            {
                if (u <= MomentumMeasurements[i].Item1)
                    return MomentumMeasurements[i].Item2;
            }

            throw new ArgumentException();
        }

        #endregion
    }
}
