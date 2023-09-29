using Quantum_Mechanics.General;
using Quantum_Mechanics.Quantum_Mechanics;
using Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools;
using ScottPlot;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Quantum_Sandbox
{
    public partial class Sandbox : Form
    {
        private int CurrentLoading;
        private bool LoadingCancelled;
        private CancellationTokenSource CancelLoading = new CancellationTokenSource();

        private Graph PositionSpaceGraph = new Graph();
        private Graph MomentumSpaceGraph = new Graph();
        private ValuesRevealed RevealScreen = new ValuesRevealed();
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

        public Sandbox()
        {
            InitializeComponent();
            ParametersControls = new Control[] { Parameters, ParametersTitle, EnvironmentTitle, CoordinateSystemTitle, CoordinateSystem, MovementConstraintsTitle, MovementConstraints, PotentialTypeTitle, PotentialType, LaboratorySizeTitle, Direction1Title, MinX, MaxX, Direction2Title, MinY, MaxY, ParticleTitle, EnergyLevelTitle, EnergyLevel, AzimuthalLevelTitle, AzimuthalLevel, Simulate };
            ToolsControls = new Control[] { ToolsTitle, WavefunctionTitle, Back, GraphPositionSpace, GraphMomentumSpace, MeasurementsTitle, MeasurePosition, PositionMeasurementX, PositionMeasurementY, MeasureMomentum, MomentumMeasurement, MeasureAngularMomentum, AngularMomentumMeasurement, MeasureEnergy, EnergyMeasurement, CalculationsTitle, CalculateExpectedPosition, ExpectedPosition, CalculateExpectedMomentum, ExpectedMomentum, RevealParticle, ToolsMenu };
            LoadingScreenControls = new Control[] { LoadingTitle, LoadingProgressBar, LoadingMessage, CancelLoadingButton, LoadingScreen };

            PositionSpaceGraph.TopMost = true;
            PositionSpaceGraph.Visible = false;
            PositionSpaceGraph.Enabled = false;

            MomentumSpaceGraph.TopMost = true;
            MomentumSpaceGraph.Visible = false;
            MomentumSpaceGraph.Enabled = false;

            RevealScreen.TopMost = true;
            RevealScreen.Visible = false;
            RevealScreen.Enabled = false;

            LoadingTimer.Enabled = false;
            ParametersControls.SetVisible(true);
            LoadingScreenControls.SetVisible(false);
            ToolsControls.SetVisible(false);
        }

        private Task<int> ConstructSystem(int energyLevel, int azimuthalLevel, CoordinateSystem coordinateSystem, PotentialType potentialType, MovementConstraints movementConstraints, string[,] positionDomainText)
        {
            return Task.Run(() =>
            {
                try
                {
                    CancelLoading.Token.ThrowIfCancellationRequested();

                    switch (coordinateSystem)
                    {
                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D)
                            {
                                CancelLoading.Token.ThrowIfCancellationRequested();
                                var positionDomain1D = new double[] { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real };

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandle1D = new QuantumSystem1D(CancelLoading.Token, 500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain1D, new double[] { -5, 5 });
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                    MainGraph.Plot.AddVerticalLine(positionDomain1D[0] - 0.1, Color.Black, 1);
                                    MainGraph.Plot.AddVerticalLine(positionDomain1D[1] + 0.1, Color.Black, 1);
                                }
                                else
                                    SystemHandle1D = new QuantumSystem1D(CancelLoading.Token, 500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[] { -10000, 10000 }, new double[] { -5, 5 });

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                for (int i = 0; i < 100; ++i)
                                {
                                    MainGraph.Plot.AddPoint(SystemHandle1D.MeasurePosition(), 0, Color.Blue);
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                            }

                            break;

                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D)
                            {
                                var positionDomain2D = new double[,] { { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real },
                                                                { RPNParser.Calculate(positionDomainText[1, 0]).Real, RPNParser.Calculate(positionDomainText[1, 1]).Real } };

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandle2D = new QuantumSystem2D(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain2D, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });
                                    CancelLoading.Token.ThrowIfCancellationRequested();

                                    MainGraph.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 0] - 0.1, Color.Black, 1);
                                    MainGraph.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                    MainGraph.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 1] + 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                    MainGraph.Plot.AddLine(positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                }
                                else
                                    SystemHandle2D = new QuantumSystem2D(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { -10000, 10000 }, { -10000, 10000 } }, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                for (int i = 0; i < 1000; ++i)
                                {
                                    var r = SystemHandle2D.MeasurePosition();
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

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandlePolar = new QuantumSystemPolar(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomainPolar, new double[,] { { 0, 5 }, { 0, Math.PI * 2 } });
                                    CancelLoading.Token.ThrowIfCancellationRequested();

                                    var dfi = (positionDomainPolar[1, 1] - positionDomainPolar[1, 0]) / 999;
                                    var r = positionDomainPolar[0, 1] + 0.1;
                                    var circle = Tuple.Create<double[], double[]>(new double[1000], new double[1000]);

                                    for (int i = 0; i < 1000; ++i)
                                    {
                                        var fi = positionDomainPolar[1, 0] + i * dfi;
                                        var x = r * Math.Cos(fi);
                                        var y = r * Math.Sin(fi);

                                        circle.Item1[i] = x;
                                        circle.Item2[i] = y;
                                        CancelLoading.Token.ThrowIfCancellationRequested();
                                    }

                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                    MainGraph.Plot.AddScatterLines(circle.Item1, circle.Item2, Color.Black, 1);
                                }
                                else
                                    SystemHandlePolar = new QuantumSystemPolar(CancelLoading.Token, (int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { 0, 10000 }, { 0, Math.PI * 2 } }, new double[,] { { 0, 5 }, { 0, Math.PI * 2 } });

                                CancelLoading.Token.ThrowIfCancellationRequested();

                                for (int i = 0; i < 1000; ++i)
                                {
                                    var r = SystemHandlePolar.MeasurePosition();
                                    MainGraph.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                                    CancelLoading.Token.ThrowIfCancellationRequested();
                                }
                            }

                            break;
                    }

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

            MainTimer.Enabled = true;
            MeasurementTimer.Enabled = true;
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

            MainGraph.Reset();
            MainGraph.Refresh();
            ParametersControls.SetVisible(true);
            ToolsControls.SetVisible(false);

            MainTimer.Enabled = false;
            MeasurementTimer.Enabled = false;
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
            RevealScreen.Visible = true;
            RevealScreen.Enabled = true;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (SystemHandle1D != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandle1D.MeasurePositionMomentum();
                    MeasuredX = m.Item1;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                    MeasuredX = SystemHandle1D.MeasurePosition();
                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandle1D.MeasureMomentum();
            }
            else if (SystemHandle2D != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandle2D.MeasurePositionMomentum();
                    MeasuredX = m.Item1.Item1;
                    MeasuredY = m.Item1.Item2;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                {
                    var r = SystemHandle2D.MeasurePosition();
                    MeasuredX = r.Item1;
                    MeasuredY = r.Item2;
                }
                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandle2D.MeasureMomentum();
            }
            else if (SystemHandlePolar != null)
            {
                if (MeasurePosition.Checked && MeasureMomentum.Checked)
                {
                    var m = SystemHandlePolar.MeasurePositionMomentum();
                    MeasuredX = m.Item1.Item1;
                    MeasuredY = m.Item1.Item2;
                    MeasuredMomentum = m.Item2;
                }
                else if (MeasurePosition.Checked)
                {
                    var r = SystemHandlePolar.MeasurePosition();
                    MeasuredX = r.Item1;
                    MeasuredY = r.Item2;
                }
                else if (MeasureMomentum.Checked)
                    MeasuredMomentum = SystemHandlePolar.MeasureMomentum();
            }
        }

        private void MeasurementTimer_Tick(object sender, EventArgs e)
        {
            if (MeasurePosition.Checked)
            {
                PositionMeasurementX.Text = MeasuredX.ToString();
                PositionMeasurementY.Text = MeasuredY.ToString();
            }
            else
            {
                PositionMeasurementX.Text = "";
                PositionMeasurementY.Text = "";
            }

            if (MeasureMomentum.Checked)
                MomentumMeasurement.Text = MeasuredMomentum.ToString();
            else
                MomentumMeasurement.Text = "";

            if (MeasureEnergy.Checked)
                EnergyMeasurement.Text = MeasuredEnergy.ToString();
            else
                EnergyMeasurement.Text = "";

            if (MeasureAngularMomentum.Checked)
                AngularMomentumMeasurement.Text = MeasuredAngularMomentum.ToString();
            else
                AngularMomentumMeasurement.Text = "";
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
