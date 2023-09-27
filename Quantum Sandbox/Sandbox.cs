using Quantum_Mechanics.General;
using Quantum_Mechanics.Quantum_Mechanics;
using Quantum_Sandbox.Mathematical_Framework.Quantum_Mechanics_Tools;
using ScottPlot;

namespace Quantum_Sandbox
{
    public partial class Sandbox : Form
    {
        int CurrentLoading;
        private QuantumSystem1D SystemHandle1D;
        private QuantumSystem2D SystemHandle2D;
        private QuantumSystemPolar SystemHandlePolar;

        public Sandbox()
        {
            InitializeComponent();
            Parameters.Controls.AddRange(new Control[] { });
            ToolsMenu.Controls.AddRange(new Control[] { Back, ToolsTitle, WavefunctionTitle, GraphPositionSpace, GraphMomentumSpace, MeasurementsTitle, MeasurePosition, PositionMeasurement, MeasureMomentum, MomentumMeasurement, MeasureAngularMomentum, AngularMomentumMeasurement, MeasureEnergy, EnergyMeasurement, CalculationsTitle, CalculateExpectedPosition, ExpectedPosition, CalculateExpectedMomentum, ExpectedMomentum, RevealParticle });
        }

        private Task<bool> ConstructSystem(int energyLevel, int azimuthalLevel, CoordinateSystem coordinateSystem, PotentialType potentialType, MovementConstraints movementConstraints, string[,] positionDomainText)
        {
            return Task.Run(() =>
            {
                try
                {
                    switch (coordinateSystem)
                    {
                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D)
                            {
                                var positionDomain1D = new double[] { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real };

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandle1D = new QuantumSystem1D(500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain1D, new double[] { -5, 5 });
                                    formsPlot1.Plot.AddVerticalLine(positionDomain1D[0] - 0.1, Color.Black, 1);
                                    formsPlot1.Plot.AddVerticalLine(positionDomain1D[1] + 0.1, Color.Black, 1);
                                }
                                else
                                    SystemHandle1D = new QuantumSystem1D(500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[] { -10000, 10000 }, new double[] { -5, 5 });

                                for (int i = 0; i < 100; ++i)
                                    formsPlot1.Plot.AddPoint(SystemHandle1D.MeasurePosition(), 0, Color.Blue);

                            }

                            break;

                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D)
                            {
                                var positionDomain2D = new double[,] { { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real },
                                                                { RPNParser.Calculate(positionDomainText[1, 0]).Real, RPNParser.Calculate(positionDomainText[1, 1]).Real } };

                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandle2D = new QuantumSystem2D((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain2D, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });
                                    formsPlot1.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 0] - 0.1, Color.Black, 1);
                                    formsPlot1.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                    formsPlot1.Plot.AddLine(positionDomain2D[0, 0] - 0.1, positionDomain2D[1, 1] + 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                    formsPlot1.Plot.AddLine(positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 0] - 0.1, positionDomain2D[0, 1] + 0.1, positionDomain2D[1, 1] + 0.1, Color.Black, 1);
                                }
                                else
                                    SystemHandle2D = new QuantumSystem2D((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { -10000, 10000 }, { -10000, 10000 } }, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });

                                for (int i = 0; i < 1000; ++i)
                                {
                                    var r = SystemHandle2D.MeasurePosition();
                                    formsPlot1.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                                }
                            }

                            break;

                        case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.POLAR:
                            if (coordinateSystem == Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.POLAR)
                            {
                                var positionDomainPolar = new double[,] { { RPNParser.Calculate(positionDomainText[0, 0]).Real, RPNParser.Calculate(positionDomainText[0, 1]).Real },
                                                                { RPNParser.Calculate(positionDomainText[1, 0]).Real, RPNParser.Calculate(positionDomainText[1, 1]).Real } };


                                if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                                {
                                    SystemHandlePolar = new QuantumSystemPolar((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomainPolar, new double[,] { { 0, 5 }, { 0, Math.PI * 2 } });
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
                                    }

                                    formsPlot1.Plot.AddScatterLines(circle.Item1, circle.Item2, Color.Black, 1);
                                }
                                else
                                    SystemHandlePolar = new QuantumSystemPolar((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { 0, 10000 }, { 0, Math.PI * 2 } }, new double[,] { { 0, 5 }, { 0, Math.PI * 2 } });

                                for (int i = 0; i < 1000; ++i)
                                {
                                    var r = SystemHandlePolar.MeasurePosition();
                                    formsPlot1.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                                }
                            }

                            break;
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private async void SetupSimulation(int energyLevel, int azimuthalLevel, CoordinateSystem system, PotentialType type, MovementConstraints constraints, string[,] domain)
        {
            formsPlot1.Reset();
            var task = ConstructSystem(energyLevel, azimuthalLevel, system, type, constraints, domain);

            await task;

            if (!task.Result)
            {
                ErrorMessage.Enabled = true;
                ErrorMessage.Visible = true;
            }

            formsPlot1.Refresh();
        }

        private void Simulate_Click(object sender, EventArgs e)
        {
            try
            {
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
    }
}

//Kocham Alicjê Fuks <3
