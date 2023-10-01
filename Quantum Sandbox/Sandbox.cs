using Quantum_Mechanics.General;
using Quantum_Mechanics.Quantum_Mechanics;
using Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools;
using ScottPlot;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
        private QuantumSystem1D SystemHandle1D { get; set; }
        [AllowNull]
        private QuantumSystem2D SystemHandle2D { get; set; }
        [AllowNull]
        private QuantumSystemPolar SystemHandlePolar { get; set; }

        private double MeasuredX;
        private double MeasuredY;
        private double MeasuredMomentum;
        private double MeasuredEnergy;
        private double MeasuredAngularMomentum;

        private double ExpectedPositionValueX;
        private double ExpectedPositionValueY;
        private double ExpectedMomentumValue;

        private Random SeedGenerator = new Random();
        private bool Superposed;
        private int CurrentSeed;

        public Sandbox()
        {
            InitializeComponent();
            ParametersControls = new Control[] { Parameters, ParametersTitle, EnvironmentTitle, CoordinateSystemTitle, CoordinateSystem, MovementConstraintsTitle, MovementConstraints, PotentialTypeTitle, PotentialType, LaboratorySizeTitle, Direction1Title, MinX, MaxX, Direction2Title, MinY, MaxY, ParticleTitle, EnergyLevelTitle, EnergyLevel, AzimuthalLevelTitle, AzimuthalLevel, Simulate };
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

        public void DrawBounds(double[,] domain)
        {
            if (SystemHandle1D != null)
            {
                MainGraph.Plot.AddVerticalLine(domain[0, 0] - 0.1, Color.Black, 1);
                MainGraph.Plot.AddVerticalLine(domain[0, 1] + 0.1, Color.Black, 1);
            }
            else if (SystemHandle2D != null)
            {
                MainGraph.Plot.AddLine(domain[0, 0] - 0.1, domain[1, 0] - 0.1, domain[0, 1] + 0.1, domain[1, 0] - 0.1, Color.Black, 1);
                MainGraph.Plot.AddLine(domain[0, 0] - 0.1, domain[1, 0] - 0.1, domain[0, 0] - 0.1, domain[1, 1] + 0.1, Color.Black, 1);
                MainGraph.Plot.AddLine(domain[0, 0] - 0.1, domain[1, 1] + 0.1, domain[0, 1] + 0.1, domain[1, 1] + 0.1, Color.Black, 1);
                MainGraph.Plot.AddLine(domain[0, 1] + 0.1, domain[1, 0] - 0.1, domain[0, 1] + 0.1, domain[1, 1] + 0.1, Color.Black, 1);
            }
            else if (SystemHandlePolar != null)
            {
                var dfi = (domain[1, 1] - domain[1, 0]) / 999;
                var r = domain[0, 1] + 0.1;
                var circle = Tuple.Create(new double[1000], new double[1000]);

                for (int i = 0; i < 1000; ++i)
                {
                    var fi = domain[1, 0] + i * dfi;
                    var x = r * Math.Cos(fi);
                    var y = r * Math.Sin(fi);

                    circle.Item1[i] = x;
                    circle.Item2[i] = y;
                    CancelLoading.Token.ThrowIfCancellationRequested();
                }

                MainGraph.Plot.AddScatterLines(circle.Item1, circle.Item2, Color.Black, 1);
            }
        }

        public void Superposition(double[,] domain)
        {
            MainGraph.Reset();

            if (SystemHandle1D != null)
            {
                MainGraph.Plot.AddVerticalLine(domain[0, 0] - 0.1, Color.Black, 1);
                MainGraph.Plot.AddVerticalLine(domain[0, 1] + 0.1, Color.Black, 1);

                for (int i = 0; i < 100; ++i)
                {
                    var seed = SeedGenerator.Next(-10000, 10001);
                    MainGraph.Plot.AddPoint(SystemHandle1D.MeasurePosition(seed), 0, Color.Blue);
                }
            }
            else if (SystemHandle2D != null)
            {
                MainGraph.Plot.AddLine(domain[0, 0] - 0.1, domain[1, 0] - 0.1, domain[0, 1] + 0.1, domain[1, 0] - 0.1, Color.Black, 1);
                MainGraph.Plot.AddLine(domain[0, 0] - 0.1, domain[1, 0] - 0.1, domain[0, 0] - 0.1, domain[1, 1] + 0.1, Color.Black, 1);
                MainGraph.Plot.AddLine(domain[0, 0] - 0.1, domain[1, 1] + 0.1, domain[0, 1] + 0.1, domain[1, 1] + 0.1, Color.Black, 1);
                MainGraph.Plot.AddLine(domain[0, 1] + 0.1, domain[1, 0] - 0.1, domain[0, 1] + 0.1, domain[1, 1] + 0.1, Color.Black, 1);

                for (int i = 0; i < 1000; ++i)
                {
                    var seed = SeedGenerator.Next(-10000, 10001);
                    var r = SystemHandle2D.MeasurePosition(seed);
                    MainGraph.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                }
            }
            else if (SystemHandlePolar != null)
            {
                var dfi = (domain[1, 1] - domain[1, 0]) / 999;
                var r = domain[0, 1] + 0.1;
                var circle = Tuple.Create(new double[1000], new double[1000]);

                for (int i = 0; i < 1000; ++i)
                {
                    var fi = domain[1, 0] + i * dfi;
                    var x = r * Math.Cos(fi);
                    var y = r * Math.Sin(fi);

                    circle.Item1[i] = x;
                    circle.Item2[i] = y;
                }

                MainGraph.Plot.AddScatterLines(circle.Item1, circle.Item2, Color.Black, 1);

                for (int i = 0; i < 1000; ++i)
                {
                    var seed = SeedGenerator.Next(-10000, 10001);
                    var p = SystemHandlePolar.MeasurePosition(seed);
                    MainGraph.Plot.AddPoint(p.Item1, p.Item2, Color.Blue);
                }
            }

            MainGraph.Refresh();
        }

        private Task<int> ConstructSystem(int energyLevel, int azimuthalLevel, CoordinateSystem coordinateSystem, PotentialType potentialType, MovementConstraints movementConstraints, string[,] positionDomainText)
        {
            return Task.Run(() =>
            {
                try
                {
                    CancelLoading.Token.ThrowIfCancellationRequested();
                    var domain = new double[2, 2];

                    switch (coordinateSystem)
                    {
                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D)
                            {
                                CancelLoading.Token.ThrowIfCancellationRequested();
                                var positionDomain1D = new double[] { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real };
                                domain = new double[,] { { positionDomain1D[0], positionDomain1D[1] }, { 0, 0 } };

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandle1D = new QuantumSystem1D(CancelLoading.Token, 500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain1D, new double[] { -5, 5 });
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                                else
                                    SystemHandle1D = new QuantumSystem1D(CancelLoading.Token, 500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[] { -10000, 10000 }, new double[] { -5, 5 });

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                MainGraph.Plot.AddVerticalLine(positionDomain1D[0] - 0.1, Color.Black, 1);
                                MainGraph.Plot.AddVerticalLine(positionDomain1D[1] + 0.1, Color.Black, 1);

                                for (int i = 0; i < 100; ++i)
                                {
                                    CurrentSeed = SeedGenerator.Next(-10000, 10001);
                                    MainGraph.Plot.AddPoint(SystemHandle1D.MeasurePosition(CurrentSeed), 0, Color.Blue);
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                            }

                            break;

                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D)
                            {
                                var positionDomain2D = new double[,] { { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real },
                                                                { RPNParser.Calculate(positionDomainText[1, 0]).Real, RPNParser.Calculate(positionDomainText[1, 1]).Real } };

                                domain = new double[,] { { positionDomain2D[0, 0], positionDomain2D[0, 1] }, { positionDomain2D[1, 0], positionDomain2D[1, 1] } };
                                CancelLoading.Token.ThrowIfCancellationRequested();

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandle2D = new QuantumSystem2D(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain2D, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                                else
                                    SystemHandle2D = new QuantumSystem2D(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { -10000, 10000 }, { -10000, 10000 } }, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                MainGraph.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 0] - 0.1, Color.Black, 1);
                                MainGraph.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                MainGraph.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 1] + 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                MainGraph.Plot.AddLine(positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);

                                for (int i = 0; i < 1000; ++i)
                                {
                                    CurrentSeed = SeedGenerator.Next(-10000, 10001);
                                    var r = SystemHandle2D.MeasurePosition(CurrentSeed);
                                    MainGraph.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                            }

                            break;

                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.POLAR:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.POLAR)
                            {
                                var positionDomainPolar = new double[,] { { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real },
                                                                { RPNParser.Calculate(positionDomainText[1, 0]).Real, RPNParser.Calculate(positionDomainText[1, 1]).Real } };

                                domain = new double[,] { { positionDomainPolar[0, 0], positionDomainPolar[0, 1] }, { positionDomainPolar[1, 0], positionDomainPolar[1, 1] } };
                                CancelLoading.Token.ThrowIfCancellationRequested();

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandlePolar = new QuantumSystemPolar(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomainPolar, new double[,] { { 0, 5 }, { 0, Math.PI * 2 } });
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                                else
                                    SystemHandlePolar = new QuantumSystemPolar(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { 0, 10000 }, { 0, Math.PI * 2 } }, new double[,] { { 0, 5 }, { 0, Math.PI * 2 } });

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                var dfi = (positionDomainPolar[1, 1] - positionDomainPolar[1, 0]) / 999;
                                var r = positionDomainPolar[0, 1] + 0.1;
                                var circle = Tuple.Create(new double[1000], new double[1000]);

                                for (int i = 0; i < 1000; ++i)
                                {
                                    var fi = positionDomainPolar[1, 0] + i * dfi;
                                    var x = r * Math.Cos(fi);
                                    var y = r * Math.Sin(fi);

                                    circle.Item1[i] = x;
                                    circle.Item2[i] = y;
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }

                                MainGraph.Plot.AddScatterLines(circle.Item1, circle.Item2, Color.Black, 1);

                                for (int i = 0; i < 1000; ++i)
                                {
                                    CurrentSeed = SeedGenerator.Next(-10000, 10001);
                                    var p = SystemHandlePolar.MeasurePosition(CurrentSeed);
                                    MainGraph.Plot.AddPoint(p.Item1, p.Item2, Color.Blue);
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                            }

                            break;
                    }

                    RevealScreen.SetPositionDomain(domain);
                    CancelLoading.Token.ThrowIfCancellationRequested();
                    return 1;
                }
                catch (OperationCanceledException)
                {
                    return 0;
                }
                catch (Exception)
                {
                    return -1;
                }
            });
        }

        private async void SetupSimulation(int energyLevel, int azimuthalLevel, CoordinateSystem system, PotentialType type, MovementConstraints constraints, string[,] domain)
        {
            MainGraph.Reset();
            LoadingTimer.Enabled = true;
            Simulate.Enabled = false;
            LoadingScreenControls.SetVisible(true);
            var task = ConstructSystem(energyLevel, azimuthalLevel, system, type, constraints, domain);

            try
            {
                await task;
            }
            catch (Exception)
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
                return;
            }
            else if (task.Result == -1)
            {
                MainGraph.Reset();
                MainGraph.Refresh();
                ErrorMessage.Enabled = true;
                ErrorMessage.Visible = true;
                Simulate.Enabled = true;
                return;
            }

            if (SystemHandle1D != null)
            {
                ExpectedPositionValueX = SystemHandle1D.ExpectedPosition();
                ExpectedMomentumValue = SystemHandle1D.ExpectedMomentum();
                MeasuredEnergy = SystemHandle1D.Energy;
                MeasuredAngularMomentum = SystemHandle1D.MeasureAngularMomentum();
            }
            else if (SystemHandle2D != null)
            {
                var exp = SystemHandle2D.ExpectedPosition();

                ExpectedPositionValueX = exp.Item1;
                ExpectedPositionValueY = exp.Item2;
                ExpectedMomentumValue = SystemHandle2D.ExpectedMomentum();
                MeasuredEnergy = SystemHandle2D.Energy;
                MeasuredAngularMomentum = SystemHandle2D.MeasureAngularMomentum();
            }
            else if (SystemHandlePolar != null)
            {
                var exp = SystemHandlePolar.ExpectedPosition();

                ExpectedPositionValueX = exp.Item1;
                ExpectedPositionValueY = exp.Item2;
                ExpectedMomentumValue = SystemHandlePolar.ExpectedMomentum();
                MeasuredEnergy = SystemHandlePolar.Energy;
                MeasuredAngularMomentum = SystemHandlePolar.MeasureAngularMomentum();
            }

            MainGraph.Refresh();
            ParametersControls.SetVisible(false);
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
                var coordinateSystem = QuantumSystem.ParseCoordinateSystem(CoordinateSystem.Text);
                var potentialType = QuantumSystem.ParsePotentialType(PotentialType.Text);
                var movementConstraints = QuantumSystem.ParseMovementConstraints(MovementConstraints.Text);
                var positionDomainText = new string[,] { { MinX.Text, MaxX.Text }, { MinY.Text, MaxY.Text } };

                SetupSimulation(energyLevel, azimuthalLevel, coordinateSystem, potentialType, movementConstraints, positionDomainText);
            }
            catch (Exception)
            {
                ErrorMessage.Enabled = true;
                ErrorMessage.Visible = true;
            }
        }

        private void Back_Click(object sender, EventArgs e)
        {
            SystemHandle1D = null;
            SystemHandle2D = null;
            SystemHandlePolar = null;

            PositionSpaceGraph.Visible = false;
            PositionSpaceGraph.Enabled = false;

            RevealScreen.SetPosition(0, 0, false);
            RevealScreen.SetMomentum(0, false);
            RevealScreen.SetEnergy(0, false);
            RevealScreen.SetAngularMomentum(0, false);

            MeasureScreen.SetPosition(0, 0, false);
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

            if (SystemHandle1D != null)
                SystemHandle1D.PlotPositionSpace(plot);
            else if (SystemHandle2D != null)
                SystemHandle2D.PlotPositionSpace(plot);
            else if (SystemHandlePolar != null)
                SystemHandlePolar.PlotPositionSpace(plot);

            plot.Refresh();

            PositionSpaceGraph.Visible = true;
            PositionSpaceGraph.Enabled = true;
        }

        private void GraphMomentumSpace_Click(object sender, EventArgs e)
        {
            var plot = MomentumSpaceGraph.GetPlot();

            plot.Reset();

            if (SystemHandle1D != null)
                SystemHandle1D.PlotMomentumSpace(plot);
            else if (SystemHandle2D != null)
                SystemHandle2D.PlotMomentumSpace(plot);
            else if (SystemHandlePolar != null)
                SystemHandlePolar.PlotMomentumSpace(plot);

            plot.Refresh();

            MomentumSpaceGraph.Visible = true;
            MomentumSpaceGraph.Enabled = true;
        }

        private void RevealParticle_Click(object sender, EventArgs e)
        {
            MainGraph.Reset();
            DrawBounds(RevealScreen.GetPositionDomain());
            var r = Tuple.Create(0d, 0d);
            var p = 0d;

            if (MeasurePosition.Checked)
                MainGraph.Plot.AddPoint(MeasuredX, MeasuredY, Color.Blue);

            if (SystemHandle1D != null)
            {
                if (MeasurePosition.Checked)
                {
                    var x = SystemHandle1D.MeasurePosition(CurrentSeed);
                    r = Tuple.Create(x, 0d);
                    MainGraph.Plot.AddPoint(x, 0, Color.Red);
                }

                if (MeasureMomentum.Checked)
                    p = SystemHandle1D.MeasureMomentum(CurrentSeed);
            }
            else if (SystemHandle2D != null)
            {
                if (MeasurePosition.Checked)
                {
                    r = SystemHandle2D.MeasurePosition(CurrentSeed);
                    MainGraph.Plot.AddPoint(r.Item1, r.Item2, Color.Red);
                }

                if (MeasureMomentum.Checked)
                    p = SystemHandle2D.MeasureMomentum(CurrentSeed);
            }
            else if (SystemHandlePolar != null)
            {
                if (MeasurePosition.Checked)
                {
                    r = SystemHandlePolar.MeasurePosition(CurrentSeed);
                    MainGraph.Plot.AddPoint(r.Item1, r.Item2, Color.Red);
                }

                if (MeasureMomentum.Checked)
                    p = SystemHandlePolar.MeasureMomentum(CurrentSeed);
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

            if (SystemHandle1D != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandle1D.MeasurePositionMomentum(CurrentSeed);
                    MeasuredX = m.Item1;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                    MeasuredX = SystemHandle1D.MeasurePosition(CurrentSeed);

                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandle1D.MeasureMomentum(CurrentSeed);
            }
            else if (SystemHandle2D != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandle2D.MeasurePositionMomentum(CurrentSeed);
                    MeasuredX = m.Item1.Item1;
                    MeasuredY = m.Item1.Item2;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                {
                    var r = SystemHandle2D.MeasurePosition(CurrentSeed);
                    MeasuredX = r.Item1;
                    MeasuredY = r.Item2;
                }
                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandle2D.MeasureMomentum(CurrentSeed);
            }
            else if (SystemHandlePolar != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandlePolar.MeasurePositionMomentum(CurrentSeed);
                    MeasuredX = m.Item1.Item1;
                    MeasuredY = m.Item1.Item2;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                {
                    var r = SystemHandlePolar.MeasurePosition(CurrentSeed);
                    MeasuredX = r.Item1;
                    MeasuredY = r.Item2;
                }
                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandlePolar.MeasureMomentum(CurrentSeed);
            }

            if (MeasurePosition.Checked)
            {
                MainGraph.Reset();
                DrawBounds(RevealScreen.GetPositionDomain());
                MainGraph.Plot.AddPoint(MeasuredX, MeasuredY, Color.Blue, 5);
                MainGraph.Refresh();
            }

            MeasureScreen.SetPosition(MeasuredX, MeasuredY, MeasurePosition.Checked);
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
                ExpectedPositionX.Text = Math.Round(ExpectedPositionValueX, 3).ToString();
                ExpectedPositionY.Text = Math.Round(ExpectedPositionValueY, 3).ToString();
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
                Superposition(RevealScreen.GetPositionDomain());
                Superposed = true;
                MainGraph.Refresh();
            }

            if (!CalculateExpectedMomentum.Checked && !CalculateExpectedPosition.Checked)
                Calculate.Enabled = false;
            else
                Calculate.Enabled = true;
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
