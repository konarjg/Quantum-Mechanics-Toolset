using ScottPlot;
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
    public partial class Graph : Form
    {
        public Graph()
        {
            InitializeComponent();
            WavefunctionGraph.Configuration.Pan = false;
            WavefunctionGraph.Configuration.Zoom = false;
        }

        public FormsPlot GetPlot()
        {
            return WavefunctionGraph;
        }

        private void Graph_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            Enabled = false;
            e.Cancel = true;
        }
    }
}
