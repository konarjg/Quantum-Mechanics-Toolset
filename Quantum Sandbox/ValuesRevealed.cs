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
    public partial class ValuesRevealed : Form
    {
        public ValuesRevealed()
        {
            InitializeComponent();
        }

        private void ValuesRevealed_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            Enabled = false;
            e.Cancel = true;
        }
    }
}
