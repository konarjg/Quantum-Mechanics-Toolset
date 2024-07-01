using Accord.Math;
using Accord.Math.Transforms;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Generate = MathNet.Numerics.Generate;

namespace Quantum_Mechanics.Quantum_Mechanics
{
    public static class Utils
    {
        public static Complex Sum(this Matrix<Complex> A)
        {
            var re = A.Real();
            var im = A.Imaginary();

            return re.ToArray().Sum() + Complex.ImaginaryOne * im.ToArray().Sum();
        }
    }

    public class QuantumSystem2D
    {
        public double[,] Wavefunction { get; private set; }
        public List<(double, (double, double))> PositionMeasurements = new List<(double, (double, double))>();
        public List<(double, (double, double))> MomentumMeasurements = new List<(double, (double, double))>();
        public Complex[,] WavefunctionMomentum { get; private set; }
        public double[] Energies { get; private set; }
        public (double[], double[]) Grid { get; private set; }
        public (double[], double[]) MomentumGrid { get; private set; }

        public double Energy { get => Energies[EnergyLevel];}

        private int EnergyLevel;
        public int AzimuthalLevel { get; private set; }
        private int Precision;

        private Random Random;

        private Complex[] CalculateMomentumSpaceWavefunction1D(double[] Wavefunction, double[] Grid, double[] MomentumGrid)
        {
            var psi_x = CreateVector.SparseOfArray(Wavefunction).ToComplex();

            Func<double, Complex> F = p =>
            {
                var k = CreateVector.SparseOfArray(Grid).ToComplex().Multiply(-Complex.ImaginaryOne * p);
                var F = 1 / Math.Sqrt(2 * Math.PI) * k.PointwiseExp().PointwiseMultiply(psi_x);

                return F.Sum() * (Grid[1] - Grid[0]);
            };

            var psi_p = new Complex[MomentumGrid.Length];

            for (int j = 0; j < MomentumGrid.Length; ++j)
                psi_p[j] = F(MomentumGrid[j]);

            var N = Math.Sqrt(1d / (psi_p.Magnitude().Pow(2).Sum() * (MomentumGrid[1] - MomentumGrid[0])));

            return CreateVector.SparseOfArray(psi_p).Multiply(N).ToArray();
        }

        private void CalculateMomentumSpace(double[] ux, double[] uy, double[] x, double[] y)
        {
            var p = Generate.LinearSpaced(x.Length, -3, 3);
            var upx = CreateVector.SparseOfArray(CalculateMomentumSpaceWavefunction1D(ux, x, p));
            var upy = CreateVector.SparseOfArray(CalculateMomentumSpaceWavefunction1D(uy, y, p));

            WavefunctionMomentum = upx.OuterProduct(upy).ToArray();
        }

        private void GeneratePositionMeasurements()
        {
            var p = new List<(double, (double, double))>();
            var PDF = Wavefunction.Pow(2);

            for (int i = 0; i < PDF.GetLength(0); ++i)
                for (int j = 0; j < PDF.GetLength(1); ++j)
                    p.Add((PDF[i, j], (Grid.Item1[i], Grid.Item2[i])));

            p = p.OrderByDescending(p => p.Item1).ToList();
            var s = new double[p.Count];
            s[0] = p[0].Item1 * (Grid.Item1[1] - Grid.Item1[0]) * (Grid.Item2[1] - Grid.Item2[0]);

            for (int i = 1; i < p.Count; ++i)
                s[i] = s[i - 1] + p[i].Item1 * (Grid.Item1[1] - Grid.Item1[0]) * (Grid.Item2[1] - Grid.Item2[0]);

            for (int i = 0; i < p.Count; ++i)
                PositionMeasurements.Add((s[i], p[i].Item2));
        }

        private void GenerateMomentumMeasurements()
        {
            var p = new List<(double, (double, double))>();
            var PDF = WavefunctionMomentum.Magnitude().Pow(2);

            for (int i = 0; i < PDF.GetLength(0); ++i)
                for (int j = 0; j < PDF.GetLength(1); ++j)
                    p.Add((PDF[i, j], (MomentumGrid.Item1[i], MomentumGrid.Item2[i])));

            p = p.OrderByDescending(p => p.Item1).ToList();
            var s = new double[p.Count];
            s[0] = p[0].Item1 * (MomentumGrid.Item1[1] - MomentumGrid.Item1[0]) * (MomentumGrid.Item2[1] - MomentumGrid.Item2[0]);

            for (int i = 1; i < p.Count; ++i)
                s[i] = s[i - 1] + p[i].Item1 * (MomentumGrid.Item1[1] - MomentumGrid.Item1[0]) * (MomentumGrid.Item2[1] - MomentumGrid.Item2[0]);

            for (int i = 0; i < p.Count; ++i)
                PositionMeasurements.Add((s[i], p[i].Item2));
        }

        public QuantumSystem2D(CancellationToken token, double[] energies, double[][][] wavefunctions, double[][] wavefunctions_X, double[][] wavefunctions_Y, (double[], double[]) grid, (double[], double[]) momentum_grid, int precision, int energyLevel, int azimuthalLevel)
        {
            Random = new Random();
            token.ThrowIfCancellationRequested();

            Precision = precision;
            EnergyLevel = energyLevel - 1;
            AzimuthalLevel = azimuthalLevel;

            token.ThrowIfCancellationRequested();

            Energies = energies;
            Wavefunction = new double[Precision, Precision];
            WavefunctionMomentum = new Complex[Precision, Precision];

            for (int i = 0; i < Precision; ++i)
                for (int j = 0; j < Precision; ++j)
                    Wavefunction[i, j] = wavefunctions[EnergyLevel][i][j];

            Grid = grid;

            token.ThrowIfCancellationRequested();

           // CalculateMomentumSpace();
            GeneratePositionMeasurements();
            GenerateMomentumMeasurements();
        }

        public void PlotPositionSpace(FormsPlot plot)
        {
            var x = Grid.Item1;
            var y = Grid.Item2;
            var u = new double[Precision, Precision];

            for (int i = 0; i < Precision; ++i)
                for (int j = 0; j < Precision; ++j)
                    u[i, j] = Wavefunction[i, j] * Wavefunction[i, j];

            plot.Plot.SetAxisLimits(x[0], x.Last(), y[0], y.Last());
            var map = plot.Plot.AddHeatmap(u);
            map.OffsetX = x[0];
            map.OffsetY = y[0];
            map.CellWidth = x[1] - x[0];
            map.CellHeight = x[1] - x[0];
            plot.Plot.AddColorbar(map);
            plot.Refresh();
        }

        public void PlotMomentumSpace(FormsPlot plot)
        {
            var kx = MomentumGrid.Item1;
            var ky = MomentumGrid.Item2;
            var u = new double[Precision, Precision];

            for (int i = 0; i < Precision; ++i)
                for (int j = 0; j < Precision; ++j)
                    u[i, j] = WavefunctionMomentum[i, j].MagnitudeSquared();

            plot.Plot.SetAxisLimits(kx[0], kx.Last(), ky[0], ky.Last());
            var map = plot.Plot.AddHeatmap(u);
            map.CellWidth = kx[1] - kx[0];
            map.CellHeight = ky[1] - ky[0];
            map.OffsetX = kx[0];
            map.OffsetY = ky[0];
            plot.Plot.AddColorbar(map);
            plot.Refresh();
        }

        private double RandomSign()
        {
            var sign = Random.Next(0, 2);

            if (sign == 0)
                return -1;

            return 1;
        }

        public ((double, double), (double, double)) MeasurePositionMomentum(int seed)
        {
            Random = new Random(seed);
            var r = MeasurePosition(seed);
            var p = MeasureMomentum(seed);

            var x = r.Item1 + RandomSign() * Random.NextDouble() * 0.5;
            var y = r.Item2 + RandomSign() * Random.NextDouble() * 0.5;
            var px = p.Item1;
            var py = p.Item2;

            return ((x, y), (px, py));
        }

        public double MeasureAngularMomentum()
        {
            return Math.Sqrt(AzimuthalLevel * (AzimuthalLevel + 1));
        }

        #region Position Space

        public (double, double) ExpectedPosition()
        {
            var x = Grid.Item1;
            var y = Grid.Item2;

            var R = Matrix.MeshGrid(x, y);

            var exp_x = Wavefunction.Pow(2).Multiply(R.Item1).Sum() * (x[1] - x[0]) * (y[1] - y[0]);
            var exp_y = Wavefunction.Pow(2).Multiply(R.Item2).Sum() * (x[1] - x[0]) * (y[1] - y[0]);
            return (exp_x, exp_y);
        }

        public (double, double) MeasurePosition(int seed)
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

        public (double, double) ExpectedMomentum()
        {
            /*var x = MomentumGrid.Item1;
            var y = MomentumGrid.Item2;

            var R = Matrix.MeshGrid(x, y);

            var exp_x = CreateMatrix.SparseOfArray(WavefunctionMomentum).PointwiseMultiply(R.Item1).Sum() * (x[1] - x[0]) * (y[1] - y[0]);
            var exp_y = CreateMatrix.SparseOfArray(WavefunctionMomentum).Multiply(R.Item2).Sum() * (x[1] - x[0]) * (y[1] - y[0]);
            return (exp_x, exp_y);*/
            return (0, 0);
        }

        public (double, double) MeasureMomentum(int seed)
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
