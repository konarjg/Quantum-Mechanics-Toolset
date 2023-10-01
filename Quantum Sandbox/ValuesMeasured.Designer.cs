namespace Quantum_Sandbox
{
    partial class ValuesMeasured
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
            Title = new Label();
            label2 = new Label();
            PositionX = new TextBox();
            PositionY = new TextBox();
            label3 = new Label();
            Momentum = new TextBox();
            label4 = new Label();
            label5 = new Label();
            AngularMomentum = new TextBox();
            Energy = new TextBox();
            SuspendLayout();
            // 
            // Title
            // 
            Title.AutoSize = true;
            Title.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            Title.Location = new Point(47, 9);
            Title.Name = "Title";
            Title.Size = new Size(261, 45);
            Title.TabIndex = 0;
            Title.Text = "Measured Values";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(15, 59);
            label2.Name = "label2";
            label2.Size = new Size(88, 30);
            label2.TabIndex = 1;
            label2.Text = "Position";
            // 
            // PositionX
            // 
            PositionX.Location = new Point(107, 62);
            PositionX.Name = "PositionX";
            PositionX.ReadOnly = true;
            PositionX.Size = new Size(88, 23);
            PositionX.TabIndex = 2;
            // 
            // PositionY
            // 
            PositionY.Location = new Point(205, 62);
            PositionY.Name = "PositionY";
            PositionY.ReadOnly = true;
            PositionY.Size = new Size(89, 23);
            PositionY.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(15, 101);
            label3.Name = "label3";
            label3.Size = new Size(127, 30);
            label3.TabIndex = 4;
            label3.Text = "Momentum";
            // 
            // Momentum
            // 
            Momentum.Location = new Point(148, 110);
            Momentum.Name = "Momentum";
            Momentum.ReadOnly = true;
            Momentum.Size = new Size(89, 23);
            Momentum.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(15, 146);
            label4.Name = "label4";
            label4.Size = new Size(80, 30);
            label4.TabIndex = 6;
            label4.Text = "Energy";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(15, 189);
            label5.Name = "label5";
            label5.Size = new Size(208, 30);
            label5.TabIndex = 7;
            label5.Text = "Angular Momentum";
            // 
            // AngularMomentum
            // 
            AngularMomentum.Location = new Point(229, 196);
            AngularMomentum.Name = "AngularMomentum";
            AngularMomentum.ReadOnly = true;
            AngularMomentum.Size = new Size(89, 23);
            AngularMomentum.TabIndex = 8;
            // 
            // Energy
            // 
            Energy.Location = new Point(101, 153);
            Energy.Name = "Energy";
            Energy.ReadOnly = true;
            Energy.Size = new Size(89, 23);
            Energy.TabIndex = 9;
            // 
            // ValuesMeasured
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(357, 247);
            Controls.Add(Energy);
            Controls.Add(AngularMomentum);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(Momentum);
            Controls.Add(label3);
            Controls.Add(PositionY);
            Controls.Add(PositionX);
            Controls.Add(label2);
            Controls.Add(Title);
            Location = new Point(400, 115);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ValuesMeasured";
            StartPosition = FormStartPosition.Manual;
            Text = "ValuesRevealed";
            FormClosing += ValuesMeasured_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title;
        private Label label2;
        private TextBox PositionX;
        private TextBox PositionY;
        private Label label3;
        private TextBox Momentum;
        private Label label4;
        private Label label5;
        private TextBox AngularMomentum;
        private TextBox Energy;
    }
}