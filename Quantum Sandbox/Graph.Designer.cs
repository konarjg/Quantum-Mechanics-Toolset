namespace Quantum_Sandbox
{
    partial class Graph
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            WavefunctionGraph = new ScottPlot.FormsPlot();
            SuspendLayout();
            // 
            // WavefunctionGraph
            // 
            WavefunctionGraph.Location = new Point(24, 12);
            WavefunctionGraph.Margin = new Padding(4, 3, 4, 3);
            WavefunctionGraph.Name = "WavefunctionGraph";
            WavefunctionGraph.Size = new Size(736, 426);
            WavefunctionGraph.TabIndex = 0;
            // 
            // Graph
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(WavefunctionGraph);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Graph";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Graph";
            FormClosing += Graph_FormClosing;
            ResumeLayout(false);
        }

        #endregion

        private ScottPlot.FormsPlot WavefunctionGraph;
    }
}