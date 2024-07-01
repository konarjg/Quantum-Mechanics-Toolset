using Accord;
using FEM.Symbolics;
using MathNet.Numerics.LinearAlgebra;
using Python.Runtime;
using Quantum_Mechanics.Quantum_Mechanics;
using Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools;
using ScottPlot;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
//fajny dzidziuœ tu by³ <33

namespace Quantum_Sandbox
{
    public partial class Sandbox : Form
    {
        private int CurrentLoading;
        private bool LoadingCancelled;
        private CancellationTokenSource CancelLoading = new CancellationTokenSource();

        private Graph PositionSpaceGraph = new Graph();
        private Graph MomentumSpaceGraph = new Graph();
        private ValuesRevealed RevealScreen;
        private ValuesMeasured MeasureScreen;
        private Control[] ParametersControls;
        private Control[] ToolsControls;
        private Control[] LoadingScreenControls;

        [AllowNull]
        private QuantumSystem1D SystemHandle { get; set; }
        [AllowNull]
        private QuantumSystem2D SystemHandle2D { get; set; }

        public static Dictionary<Scenario, dynamic> ScenarioSolutions = new Dictionary<Scenario, dynamic>();
        private Scenario Scenario { get; set; }

        private double Measured;
        private double MeasuredMomentum;
        private double MeasuredEnergy;
        private double MeasuredAngularMomentum;

        private double ExpectedPositionValue;
        private double ExpectedMomentumValue;

        private Random SeedGenerator = new Random();
        private bool Superposed;
        private int CurrentSeed;

        public Sandbox()
        {
            InitializeComponent();

            ParametersControls = new Control[] { Parameters, ParametersTitle, EnvironmentTitle, PotentialTypeTitle, ScenarioDropdown, ParticleTitle, EnergyLevelTitle, EnergyLevel, AzimuthalLevelTitle, AzimuthalLevel, Simulate };
            ToolsControls = new Control[] { ToolsTitle, WavefunctionTitle, Back, GraphPositionSpace, GraphMomentumSpace, MeasurementsTitle, Measure, MeasurePosition, MeasureMomentum, MeasureAngularMomentum, MeasureEnergy, CalculationsTitle, Calculate, CalculateExpectedPosition, ExpectedPositionX, ExpectedPositionY, CalculateExpectedMomentum, ExpectedMomentum, RevealParticle, ToolsMenu };
            LoadingScreenControls = new Control[] { LoadingTitle, LoadingProgressBar, LoadingMessage, CancelLoadingButton, LoadingScreen };

            PositionSpaceGraph.TopMost = true;
            PositionSpaceGraph.Visible = false;
            PositionSpaceGraph.Enabled = false;

            MomentumSpaceGraph.TopMost = true;
            MomentumSpaceGraph.Visible = false;
            MomentumSpaceGraph.Enabled = false;

            RevealScreen = new ValuesRevealed(this);
            RevealScreen.TopMost = true;
            RevealScreen.Visible = false;
            RevealScreen.Enabled = false;

            MeasureScreen = new ValuesMeasured(this);
            MeasureScreen.TopMost = true;
            MeasureScreen.Visible = false;
            MeasureScreen.Enabled = false;

            LoadingTimer.Enabled = false;
            ParametersControls.SetVisible(true);
            LoadingScreenControls.SetVisible(false);
            ToolsControls.SetVisible(false);
        }

        public void DrawBounds()
        {
            MainGraph.Reset();

            if (SystemHandle != null)
            {
                switch (Scenario)
                {
                    case Scenario.INFINITE_RECTANGULAR_WELL:
                        MainGraph.Plot.AddVerticalLine(0, Color.Black, 1);
                        MainGraph.Plot.AddVerticalLine(5, Color.Black, 1);
                        break;

                    case Scenario.HARMONIC_OSCILLATOR:
                        MainGraph.Plot.AddVerticalLine(-5, Color.Black, 1);
                        MainGraph.Plot.AddVerticalLine(5, Color.Black, 1);
                        break;

                    case Scenario.FINITE_RECTANGULAR_WELL:
                        MainGraph.Plot.AddVerticalLine(-2.5, Color.Black, 1);
                        MainGraph.Plot.AddVerticalLine(2.5, Color.Black, 1);
                        break;

                    case Scenario.DELTA_POTENTIAL:
                        MainGraph.Plot.AddVerticalLine(0, Color.Black, 1);
                        break;

                    case Scenario.HYDROGEN_ATOM:
                        MainGraph.Plot.AddPoint(0, 0, Color.Black, 10);
                        break;
                }

                SetAxisLimits(SystemHandle.AzimuthalLevel);
            }

            MainGraph.Refresh();
        }

        public void SetAxisLimits(int l)
        {
            switch (Scenario)
            {
                case Scenario.INFINITE_RECTANGULAR_WELL:
                    MainGraph.Plot.SetAxisLimits(-3, 8);
                    break;

                case Scenario.HARMONIC_OSCILLATOR:
                    MainGraph.Plot.SetAxisLimits(-8, 8);
                    break;

                case Scenario.FINITE_RECTANGULAR_WELL:
                    MainGraph.Plot.SetAxisLimits(-5.5, 5.5);
                    break;

                case Scenario.DELTA_POTENTIAL:
                    MainGraph.Plot.SetAxisLimits(-3, 3);
                    break;

                case Scenario.HYDROGEN_ATOM:
                    MainGraph.Plot.SetAxisLimits(-3, 10 * (l + 1));
                    break;
            }
        }

        public void Superposition()
        {
            MainGraph.Reset();

            if (SystemHandle != null)
            {
                switch (Scenario)
                {
                    case Scenario.INFINITE_RECTANGULAR_WELL:
                        MainGraph.Plot.AddVerticalLine(0, Color.Black, 1);
                        MainGraph.Plot.AddVerticalLine(5, Color.Black, 1);
                        break;

                    case Scenario.HARMONIC_OSCILLATOR:
                        MainGraph.Plot.AddVerticalLine(-5, Color.Black, 1);
                        MainGraph.Plot.AddVerticalLine(5, Color.Black, 1);
                        break;

                    case Scenario.FINITE_RECTANGULAR_WELL:
                        MainGraph.Plot.AddVerticalLine(-2.5, Color.Black, 1);
                        MainGraph.Plot.AddVerticalLine(2.5, Color.Black, 1);
                        break;

                    case Scenario.DELTA_POTENTIAL:
                        MainGraph.Plot.AddVerticalLine(0, Color.Black, 1);
                        break;

                    case Scenario.HYDROGEN_ATOM:
                        MainGraph.Plot.AddPoint(0, 0, Color.Black, 10);
                        break;
                }

                for (int i = 0; i < 500; ++i)
                {
                    var seed = SeedGenerator.Next(-10000, 10001);
                    MainGraph.Plot.AddPoint(SystemHandle.MeasurePosition(seed), 0, Color.Blue);
                }

                SetAxisLimits(SystemHandle.AzimuthalLevel);
            }

            MainGraph.Refresh();
        }

        private Task<int> ConstructSystem(int energyLevel, int azimuthalLevel, Scenario scenario)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!checkBox1.Checked)
                    {
                        double[] energies = null;
                        double[][] wavefunctions = null;
                        double[] grid = null;

                        var pythonDll = @"C:\Program Files\Python312\python312.dll";
                        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll);
                        PythonEngine.Initialize();

                        using (Py.GIL())
                        {
                            dynamic Solver = Py.Import("quantum_mechanics");
                            dynamic solution = default;

                            switch (scenario)
                            {
                                case Scenario.INFINITE_RECTANGULAR_WELL:
                                    solution = Solver.infinite_rect_well(5, 2000);
                                    break;

                                case Scenario.HARMONIC_OSCILLATOR:
                                    solution = Solver.harmonic_oscillator(2000);
                                    break;

                                case Scenario.FINITE_RECTANGULAR_WELL:
                                    solution = Solver.finite_rect_well(2, 5, 2000);
                                    break;

                                case Scenario.DELTA_POTENTIAL:
                                    solution = Solver.delta_potential(2000);
                                    break;

                                case Scenario.HYDROGEN_ATOM:
                                    solution = Solver.hydrogen_atom(2000, azimuthalLevel);
                                    break;
                            }

                            energies = solution[0].As<double[]>();
                            wavefunctions = solution[1].As<double[][]>();
                            grid = solution[2].As<double[]>();

                            solution.Dispose();
                            Solver.Dispose();
                        }

                        PythonEngine.Shutdown();

                        SystemHandle = new QuantumSystem1D(CancelLoading.Token, energies, wavefunctions, grid, grid.Length, energyLevel, azimuthalLevel, scenario == Scenario.HYDROGEN_ATOM);

                        CancelLoading.Token.ThrowIfCancellationRequested();

                        //RevealScreen.SetPositionDomain(new double[,] { system.Grid, });
                        CancelLoading.Token.ThrowIfCancellationRequested();
                        Superposed = false;
                        return 1;
                    }
                    else
                    {
                       /* double[] energies = null;
                        double[][][] wavefunctions = null;
                        Complex[][][] wavefunctions_momentum = null;
                        (double[], double[]) grid;
                        (double[], double[]) momentum_grid;

                        var pythonDll = @"C:\Program Files\Python312\python312.dll";
                        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll);
                        PythonEngine.Initialize();

                        using (Py.GIL())
                        {
                            dynamic np = Py.Import("numpy");
                            dynamic Solver = Py.Import("quantum_mechanics");
                            dynamic solution = default;

                            switch (scenario)
                            {
                                case Scenario.INFINITE_RECTANGULAR_WELL:
                                    solution = Solver.infinite_rect_well_2d(2000, 5, 5);
                                    break;

                                case Scenario.HARMONIC_OSCILLATOR:
                                    solution = Solver.harmonic_oscillator_2d(2000);
                                    break;

                                case Scenario.FINITE_RECTANGULAR_WELL:
                                    solution = Solver.finite_rect_well_2d(2000, 2, 5, 5);
                                    break;

                                case Scenario.DELTA_POTENTIAL:
                                    solution = Solver.delta_potential_2d(2000);
                                    break;

                                case Scenario.HYDROGEN_ATOM:
                                    solution = Solver.hydrogen_atom2d(1000);
                                    break;
                            }

                            energies = solution[0].As<double[]>();
                            wavefunctions = solution[1].As<double[][][]>();
                            wavefunctions_momentum = solution[2].As<Complex[][][]>();
                            var x = solution[2].As<double[]>();
                            var y = solution[3].As<double[]>();
                            var px = solution[4].As<double[]>();
                            var py = solution[5].As<double[]>();

                            grid = (x, y);
                            momentum_grid = (px, py);
                            solution.Dispose();
                            Solver.Dispose();
                        }

                        PythonEngine.Shutdown();

                        SystemHandle2D = new QuantumSystem2D(CancelLoading.Token, energies, wavefunctions, wavefunctions_momentum, grid, momentum_grid, grid.Item1.Length, energyLevel, azimuthalLevel);

                        CancelLoading.Token.ThrowIfCancellationRequested();

                        //RevealScreen.SetPositionDomain(new double[,] { system.Grid, });
                        CancelLoading.Token.ThrowIfCancellationRequested();
                        Superposed = false;*/
                        return 1;
                    }
                }
                catch (OperationCanceledException)
                {
                    return 0;
                }
                catch (Exception e) 
                {
                    MessageBox.Show(e.Message + "\n" + e.StackTrace);
                    return -1;
                }
            });
        }

        private async void SetupSimulation(int energyLevel, int azimuthalLevel, Scenario scenario)
        {
            MainGraph.Reset();
            LoadingTimer.Enabled = true;
            Simulate.Enabled = false;
            LoadingScreenControls.SetVisible(true);
            var task = ConstructSystem(energyLevel, azimuthalLevel, scenario);

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                MainGraph.Reset();
                MainGraph.Refresh();
                LoadingScreenControls.SetVisible(false);
                ErrorMessage.Enabled = false;
                ErrorMessage.Visible = false;
                Simulate.Enabled = true;
                LoadingCancelled = false;
                CancelLoading.Dispose();
                CancelLoading = new CancellationTokenSource();
                MessageBox.Show(ex.Message);
                return;
            }

            LoadingScreenControls.SetVisible(false);

            if (task.Result == 0)
            {
                MainGraph.Reset();
                MainGraph.Refresh();
                ErrorMessage.Enabled = false;
                ErrorMessage.Visible = false;
                Simulate.Enabled = true;
                LoadingCancelled = false;
                CancelLoading.Dispose();
                CancelLoading = new CancellationTokenSource();
                GC.Collect();
                return;
            }
            else if (task.Result == -1)
            {
                MainGraph.Reset();
                MainGraph.Refresh();
                ErrorMessage.Enabled = true;
                ErrorMessage.Visible = true;
                Simulate.Enabled = true;
                GC.Collect();
                return;
            }

            GC.Collect();

            if (SystemHandle != null)
            {
                ExpectedPositionValue = SystemHandle.ExpectedPosition();
                ExpectedMomentumValue = SystemHandle.ExpectedMomentum();
                MeasuredEnergy = SystemHandle.Energy;
                MeasuredAngularMomentum = SystemHandle.MeasureAngularMomentum();
            }
            else if (SystemHandle2D != null)
            {
                ExpectedPositionValue = SystemHandle2D.ExpectedPosition().Item1;
                ExpectedMomentumValue = SystemHandle2D.ExpectedMomentum().Item1;
                MeasuredEnergy = SystemHandle2D.Energy;
                MeasuredAngularMomentum = SystemHandle2D.MeasureAngularMomentum();
            }

            MainGraph.Refresh();
            ParametersControls.SetVisible(false);
            LoadingTimer.Enabled = false;
            ToolsControls.SetVisible(true);
        }

        private void Simulate_Click(object sender, EventArgs e)
        {
            try
            {
                LoadingMessage.Text = "This may take some time for complex scenarios";
                ErrorMessage.Enabled = false;
                ErrorMessage.Visible = false;

                var energyLevel = int.Parse(EnergyLevel.Text);
                var azimuthalLevel = int.Parse(AzimuthalLevel.Text);
                var scenario = QuantumSystem.ParseScenario(ScenarioDropdown.Text);
                Scenario = scenario;

                SetupSimulation(energyLevel, azimuthalLevel, scenario);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ErrorMessage.Enabled = true;
                ErrorMessage.Visible = true;
            }
        }

        private void Back_Click(object sender, EventArgs e)
        {
            SystemHandle = null;
            GC.Collect();

            PositionSpaceGraph.Visible = false;
            PositionSpaceGraph.Enabled = false;

            RevealScreen.SetPosition(0, 0, false);
            RevealScreen.SetMomentum(0, false);
            RevealScreen.SetEnergy(0, false);
            RevealScreen.SetAngularMomentum(0, false);

            MeasureScreen.SetPosition(0, false);
            MeasureScreen.SetMomentum(0, false);
            MeasureScreen.SetEnergy(0, false);
            MeasureScreen.SetAngularMomentum(0, false);

            ExpectedPositionX.Text = "";
            ExpectedPositionY.Text = "";
            ExpectedMomentum.Text = "";

            MainGraph.Reset();
            MainGraph.Refresh();
            ParametersControls.SetVisible(true);
            ToolsControls.SetVisible(false);
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentLoading > 3)
            {
                CurrentLoading = 0;
                return;
            }

            if (!LoadingCancelled)
                LoadingTitle.Text = "Setting up the simulation";
            else
                LoadingTitle.Text = "Cancelling the simulation";

            for (int i = 0; i < CurrentLoading; ++i)
                LoadingTitle.Text += ".";

            ++CurrentLoading;
        }

        private void CancelLoadingButton_Click(object sender, EventArgs e)
        {
            CancelLoading.Cancel();
            CancelLoadingButton.Enabled = false;
            LoadingCancelled = true;
            LoadingMessage.Text = "It may take some time to free the used memory";
        }

        private void GraphPositionSpace_Click(object sender, EventArgs e)
        {
            var plot = PositionSpaceGraph.GetPlot();

            plot.Reset();

            if (SystemHandle != null)
                SystemHandle.PlotPositionSpace(plot);
            else if (SystemHandle2D != null)
                SystemHandle2D.PlotPositionSpace(plot);

            plot.Refresh();

            PositionSpaceGraph.Visible = true;
            PositionSpaceGraph.Enabled = true;
        }

        private void GraphMomentumSpace_Click(object sender, EventArgs e)
        {
            var plot = MomentumSpaceGraph.GetPlot();

            plot.Reset();

            if (SystemHandle != null)
                SystemHandle.PlotMomentumSpace(plot);

            plot.Refresh();

            MomentumSpaceGraph.Visible = true;
            MomentumSpaceGraph.Enabled = true;
        }

        private void RevealParticle_Click(object sender, EventArgs e)
        {
            MainGraph.Reset();
            DrawBounds();
            var r = Tuple.Create(0d, 0d);
            var p = 0d;

            if (MeasurePosition.Checked)
                MainGraph.Plot.AddPoint(Measured, 0, Color.Blue);

            if (SystemHandle != null)
            {
                if (MeasurePosition.Checked)
                {
                    var x = SystemHandle.MeasurePosition(CurrentSeed);
                    r = Tuple.Create(x, 0d);
                    MainGraph.Plot.AddPoint(x, 0, Color.Red);
                }

                if (MeasureMomentum.Checked)
                    p = SystemHandle.MeasureMomentum(CurrentSeed);

                SetAxisLimits(SystemHandle.AzimuthalLevel);
            }

            RevealScreen.SetEnergy(MeasuredEnergy, MeasureEnergy.Checked);
            RevealScreen.SetAngularMomentum(MeasuredAngularMomentum, MeasureAngularMomentum.Checked);
            RevealScreen.SetPosition(r.Item1, r.Item2, MeasurePosition.Checked);
            RevealScreen.SetMomentum(p, MeasureMomentum.Checked);

            RevealScreen.Visible = true;
            RevealScreen.Enabled = true;

            MainGraph.Refresh();
        }

        public void RemoveReveal()
        {
            var points = MainGraph.Plot.GetPlottables();

            Superposed = false;
            MainGraph.Plot.RemoveAt(points.Length - 1);
            MainGraph.Refresh();
        }

        public void RemoveMeasurement()
        {
            Superposed = false;
        }

        private void Measure_Click(object sender, EventArgs e)
        {
            CurrentSeed = SeedGenerator.Next(-10000, 10001);
            var domain = RevealScreen.GetPositionDomain();

            if (SystemHandle != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandle.MeasurePositionMomentum(CurrentSeed);
                    Measured = m.Item1;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                    Measured = SystemHandle.MeasurePosition(CurrentSeed);

                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandle.MeasureMomentum(CurrentSeed);
            }

            if (MeasurePosition.Checked)
            {
                MainGraph.Reset();
                DrawBounds();
                MainGraph.Plot.AddPoint(Measured, 0, Color.Blue, 5);
                SetAxisLimits(SystemHandle.AzimuthalLevel);
                MainGraph.Refresh();
            }

            MeasureScreen.SetPosition(Measured, MeasurePosition.Checked);
            MeasureScreen.SetMomentum(MeasuredMomentum, MeasureMomentum.Checked);
            MeasureScreen.SetEnergy(MeasuredEnergy, MeasureEnergy.Checked);
            MeasureScreen.SetAngularMomentum(MeasuredAngularMomentum, MeasureAngularMomentum.Checked);

            MeasureScreen.Visible = true;
            MeasureScreen.Enabled = true;
        }

        private void Calculate_Click(object sender, EventArgs e)
        {
            if (CalculateExpectedPosition.Checked)
            {
                ExpectedPositionX.Text = Math.Round(ExpectedPositionValue, 3).ToString();
            }
            else
            {
                ExpectedPositionX.Text = "";
                ExpectedPositionY.Text = "";
            }

            if (CalculateExpectedMomentum.Checked)
                ExpectedMomentum.Text = Math.Round(ExpectedMomentumValue, 3).ToString();
            else
                ExpectedMomentum.Text = "";
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (!MeasurePosition.Checked && !MeasureMomentum.Checked && !MeasureAngularMomentum.Checked && !MeasureEnergy.Checked)
            {
                Measure.Enabled = false;
                RevealParticle.Enabled = false;
            }
            else
            {
                Measure.Enabled = true;
                RevealParticle.Enabled = true;
            }

            if (!Superposed)
            {
                MainGraph.Reset();
                Superposition();
                Superposed = true;
                MainGraph.Refresh();
            }

            if (!CalculateExpectedMomentum.Checked && !CalculateExpectedPosition.Checked)
                Calculate.Enabled = false;
            else
                Calculate.Enabled = true;
        }

        private void Sandbox_Load(object sender, EventArgs e)
        {

        }

        private void Sandbox_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    internal static class ControlUtils
    {
        public static void SetVisible(this Control[] controls, bool visible)
        {
            for (int i = 0; i < controls.Length; ++i)
            {
                controls[i].Enabled = visible;
                controls[i].Visible = visible;
            }
        }
    }
}

//Kocham Alicjê Fuks <3
