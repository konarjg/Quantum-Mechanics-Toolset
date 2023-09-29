namespace Quantum_Sandbox
{
    partial class ValuesRevealed
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
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label3 = new Label();
            textBox3 = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(60, 9);
            label1.Name = "label1";
            label1.Size = new Size(179, 45);
            label1.TabIndex = 0;
            label1.Text = "Real Values";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(60, 59);
            label2.Name = "label2";
            label2.Size = new Size(88, 30);
            label2.TabIndex = 1;
            label2.Text = "Position";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(153, 62);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(34, 23);
            textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(193, 62);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(34, 23);
            textBox2.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(60, 101);
            label3.Name = "label3";
            label3.Size = new Size(127, 30);
            label3.TabIndex = 4;
            label3.Text = "Momentum";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(193, 108);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(34, 23);
            textBox3.TabIndex = 5;
            // 
            // ValuesRevealed
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(294, 170);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ValuesRevealed";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ValuesRevealed";
            FormClosing += ValuesRevealed_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label3;
        private TextBox textBox3;
    }
}