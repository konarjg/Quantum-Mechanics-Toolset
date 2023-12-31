﻿using System;
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
        private Sandbox ParentSandboxMode;
        private double[,] PositionDomain;

        public ValuesRevealed(Sandbox parent)
        {
            InitializeComponent();
            ParentSandboxMode = parent;
        }

        public void SetPositionDomain(double[,] domain)
        {
            PositionDomain = domain;
        }

        public double[,] GetPositionDomain()
        {
            return PositionDomain;
        }

        public void SetPosition(double x, double y, bool revealed = true)
        {
            if (!revealed)
            {
                PositionX.Text = "";
                PositionY.Text = "";
                return;
            }

            PositionX.Text = Math.Round(x, 3).ToString();
            PositionY.Text = Math.Round(y, 3).ToString();
        }

        public void SetMomentum(double p, bool revealed = true)
        {
            if (!revealed)
            {
                Momentum.Text = "";
                return;
            }

            Momentum.Text = Math.Round(p, 3).ToString();
        }

        public void SetEnergy(double E, bool revealed = true)
        {
            if (!revealed)
            {
                Energy.Text = "";
                return;
            }

            Energy.Text = Math.Round(E, 3).ToString();
        }

        public void SetAngularMomentum(double L, bool revealed = true)
        {
            if (!revealed)
            {
                AngularMomentum.Text = "";
                return;
            }

            AngularMomentum.Text = Math.Round(L, 3).ToString();
        }

        private void ValuesRevealed_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            Enabled = false;
            ParentSandboxMode.RemoveReveal();
            e.Cancel = true;
        }
    }
}
