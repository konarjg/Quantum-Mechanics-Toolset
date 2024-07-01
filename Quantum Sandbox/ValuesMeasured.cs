using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quantum_Sandbox
{
    public partial class ValuesMeasured : Form
    {
        private Sandbox ParentSandboxMode;

        public ValuesMeasured(Sandbox parent)
        {
            InitializeComponent();
            ParentSandboxMode = parent;
        }

        public void SetPosition(double x, bool measured = true)
        {
            if (!measured)
            {
                Position.Text = "";
                return;
            }

            Position.Text = Math.Round(x, 3).ToString();
        }

        public void SetMomentum(double p, bool measured = true)
        {
            if (!measured)
            {
                Momentum.Text = "";
                return;
            }

            Momentum.Text = Math.Round(p, 3).ToString();
        }

        public void SetEnergy(double E, bool measured = true)
        {
            if (!measured)
            {
                Energy.Text = "";
                return;
            }

            Energy.Text = Math.Round(E, 3).ToString() + " eV";
        }

        public void SetAngularMomentum(double L, bool measured = true)
        {
            if (!measured)
            {
                AngularMomentum.Text = "";
                return;
            }

            AngularMomentum.Text = Math.Round(L, 3).ToString();
        }

        private void ValuesMeasured_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            Enabled = false;
            ParentSandboxMode.RemoveMeasurement();
            e.Cancel = true;
        }

        private void ValuesMeasured_Load(object sender, EventArgs e)
        {

        }
    }
}
