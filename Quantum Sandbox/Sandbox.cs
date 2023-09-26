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
        }

        private async void ConstructSystem(CoordinateSystem coordinateSystem, PotentialType potentialType, MovementConstraints movementConstraints, string[,] positionDomainText)
        {
            var energyLevel = int.Parse(EnergyLevel.Text);
            var azimuthalLevel = int.Parse(AzimuthalLevel.Text);

            formsPlot1.Reset();

            await Task.Run(() =>
            {
                switch (coordinateSystem)
                {
                    case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_1D:
                        var positionDomain1D = new double[] { double.Parse(positionDomainText[0, 0]), double.Parse(positionDomainText[0, 1]) };

                        if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                        {
                            SystemHandle1D = new QuantumSystem1D(500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain1D, new double[] { -5, 5 });
                            formsPlot1.Plot.AddVerticalLine(positionDomain1D[0], Color.Black, 1);
                            formsPlot1.Plot.AddVerticalLine(positionDomain1D[1], Color.Black, 1);
                        }
                        else
                            SystemHandle1D = new QuantumSystem1D(500, energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[] { -10000, 10000 }, new double[] { -5, 5 });

                        for (int i = 0; i < 100; ++i)
                            formsPlot1.Plot.AddPoint(SystemHandle1D.MeasurePosition(), 0, Color.Blue);

                        break;

                    case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_2D:
                        var positionDomain2D = new double[,] { { double.Parse(positionDomainText[0, 0]), double.Parse(positionDomainText[0, 1]) },
                                                                { double.Parse(positionDomainText[1, 0]), double.Parse(positionDomainText[1, 1]) } };

                        if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                        {
                            SystemHandle2D = new QuantumSystem2D((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain2D, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 0], positionDomain2D[1, 0], positionDomain2D[0, 1], positionDomain2D[1, 0], Color.Black, 1);
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 0], positionDomain2D[1, 0], positionDomain2D[0, 0], positionDomain2D[1, 1], Color.Black, 1);
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 0], positionDomain2D[1, 1], positionDomain2D[0, 1], positionDomain2D[1, 1], Color.Black, 1);
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 1], positionDomain2D[1, 0], positionDomain2D[0, 1], positionDomain2D[1, 1], Color.Black, 1);
                        }
                        else
                            SystemHandle2D = new QuantumSystem2D((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { -10000, 10000 }, { -10000, 10000 } }, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });

                        for (int i = 0; i < 1000; ++i)
                        {
                            var r = SystemHandle2D.MeasurePosition();
                            formsPlot1.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                        }

                        break;

                    case Mathematical_Framework.Quantum_Mechanics_Tools.CoordinateSystem.CARTESIAN_POLAR:
                        var positionDomainPolar = new double[,] { { double.Parse(positionDomainText[0, 0]), double.Parse(positionDomainText[0, 1]) },
                                                                { double.Parse(positionDomainText[1, 0]), double.Parse(positionDomainText[1, 1]) } };

                        if (movementConstraints == Mathematical_Framework.Quantum_Mechanics_Tools.MovementConstraints.POTENTIAL_BARRIER)
                        {
                            SystemHandle2D = new QuantumSystem2D((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), positionDomain2D, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 0], positionDomain2D[1, 0], positionDomain2D[0, 1], positionDomain2D[1, 0], Color.Black, 1);
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 0], positionDomain2D[1, 0], positionDomain2D[0, 0], positionDomain2D[1, 1], Color.Black, 1);
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 0], positionDomain2D[1, 1], positionDomain2D[0, 1], positionDomain2D[1, 1], Color.Black, 1);
                            formsPlot1.Plot.AddLine(positionDomain2D[0, 1], positionDomain2D[1, 0], positionDomain2D[0, 1], positionDomain2D[1, 1], Color.Black, 1);
                        }
                        else
                            SystemHandle2D = new QuantumSystem2D((int)Math.Sqrt(500), energyLevel, azimuthalLevel, QuantumConstants.Me, QuantumSystem.PotentialFunction(coordinateSystem, potentialType), new double[,] { { -10000, 10000 }, { -10000, 10000 } }, new double[,] { { -5, 5 }, { -5, 5 } }, new double[] { 0, 5 });

                        for (int i = 0; i < 1000; ++i)
                        {
                            var r = SystemHandle2D.MeasurePosition();
                            formsPlot1.Plot.AddPoint(r.Item1, r.Item2, Color.Blue);
                        }

                        break;
                }
            });

            formsPlot1.Refresh();
        }

        private void Simulate_Click(object sender, EventArgs e)
        {
            var coordinateSystem = QuantumSystem.ParseCoordinateSystem(CoordinateSystem.Text);
            var potentialType = QuantumSystem.ParsePotentialType(PotentialType.Text);
            var movementConstraints = QuantumSystem.ParseMovementConstraints(MovementConstraints.Text);
            var positionDomainText = new string[,] { { MinX.Text, MaxX.Text }, { MinY.Text, MaxY.Text } };

            ConstructSystem(coordinateSystem, potentialType, movementConstraints, positionDomainText);
        }
    }
}

//kocham alicje fuks very mocno<<3333