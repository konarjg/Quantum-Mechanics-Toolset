using Quantum_Mechanics.Quantum_Mechanics;

namespace Quantum_Sandbox
{
    public partial class Sandbox : Form
    {
        int CurrentLoading;

        public Sandbox()
        {
            InitializeComponent();
        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {

        }

        private async void Simulate()
        {
            label1.Enabled = true;
            label1.Visible = true;

            await Task.Run(() =>
            {
                var R = new double[,] { { 0, 5 }, { 0, Math.PI * 2 } };
                var psi = new QuantumSystemPolar((int)Math.Sqrt(500), 1, 1, QuantumConstants.Me, "0", R, R);

                var exp = psi.ExpectedPosition();
                var n = 1000;
                var dfi = (R[1, 1] - R[1, 0]) / (n - 1);
                var r = R[0, 1];
                var circle_x = new double[n];
                var circle_y = new double[n];

                for (int i = 0; i < n; ++i)
                {
                    var fi = R[1, 0] + i * dfi;
                    circle_x[i] = r * Math.Cos(fi);
                    circle_y[i] = r * Math.Sin(fi);
                }

                formsPlot1.Plot.AddScatterLines(circle_x, circle_y, Color.Black, 10);

                for (int i = 0; i < n; ++i)
                {
                    var x = psi.MeasurePosition();
                    formsPlot1.Plot.AddPoint(x.Item1, x.Item2, Color.Blue, 5, ScottPlot.MarkerShape.filledCircle);
                }

                formsPlot1.Plot.AddPoint(exp.Item1, exp.Item2, Color.Red, 10, ScottPlot.MarkerShape.filledCircle, "<r>");
            });

            formsPlot1.Refresh();
            label1.Enabled = false;
            label1.Visible = false;
        }

        private void Sandbox_Load(object sender, EventArgs e)
        {
            Simulate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!label1.Enabled)
                return;

            label1.Text = "Loading";

            for (int i = 0; i < CurrentLoading; ++i)
                label1.Text += ".";

            if (CurrentLoading == 3)
            {
                CurrentLoading = 0;
                return;
            }

            ++CurrentLoading;
        }
    }
}

//kocham alicje fuks very mocno<<3333