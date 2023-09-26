namespace Quantum_Sandbox
{
    partial class Sandbox
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            formsPlot1 = new ScottPlot.FormsPlot();
            timer1 = new System.Windows.Forms.Timer(components);
            listView1 = new ListView();
            label1 = new Label();
            CoordinateSystem = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            MovementConstraints = new ComboBox();
            label4 = new Label();
            PotentialType = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            EnergyLevel = new ComboBox();
            label8 = new Label();
            label7 = new Label();
            label9 = new Label();
            label10 = new Label();
            MinX = new TextBox();
            MaxX = new TextBox();
            MinY = new TextBox();
            MaxY = new TextBox();
            AzimuthalLevel = new ComboBox();
            label11 = new Label();
            Simulate = new Button();
            SuspendLayout();
            // 
            // formsPlot1
            // 
            formsPlot1.Location = new Point(700, 12);
            formsPlot1.Margin = new Padding(4, 3, 4, 3);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(1000, 1000);
            formsPlot1.TabIndex = 0;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 500;
            // 
            // listView1
            // 
            listView1.Location = new Point(54, 134);
            listView1.Name = "listView1";
            listView1.Size = new Size(602, 694);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(180, 155);
            label1.Name = "label1";
            label1.Size = new Size(338, 45);
            label1.TabIndex = 2;
            label1.Text = "Simulation Parameters";
            // 
            // CoordinateSystem
            // 
            CoordinateSystem.DropDownStyle = ComboBoxStyle.DropDownList;
            CoordinateSystem.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CoordinateSystem.Items.AddRange(new object[] { "Cartesian 1D", "Cartesian 2D", "Polar" });
            CoordinateSystem.Location = new Point(345, 257);
            CoordinateSystem.Name = "CoordinateSystem";
            CoordinateSystem.Size = new Size(173, 40);
            CoordinateSystem.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(143, 262);
            label2.Name = "label2";
            label2.Size = new Size(196, 30);
            label2.TabIndex = 4;
            label2.Text = "Coordinate System";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(96, 308);
            label3.Name = "label3";
            label3.Size = new Size(232, 30);
            label3.TabIndex = 5;
            label3.Text = "Movement Constraints";
            // 
            // MovementConstraints
            // 
            MovementConstraints.DropDownStyle = ComboBoxStyle.DropDownList;
            MovementConstraints.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MovementConstraints.Items.AddRange(new object[] { "Free Movement", "Potential Barriers" });
            MovementConstraints.Location = new Point(334, 303);
            MovementConstraints.Name = "MovementConstraints";
            MovementConstraints.Size = new Size(256, 40);
            MovementConstraints.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(96, 361);
            label4.Name = "label4";
            label4.Size = new Size(231, 30);
            label4.TabIndex = 7;
            label4.Text = "External Potential Type";
            // 
            // PotentialType
            // 
            PotentialType.DropDownStyle = ComboBoxStyle.DropDownList;
            PotentialType.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            PotentialType.Items.AddRange(new object[] { "None", "Harmonic", "Electric" });
            PotentialType.Location = new Point(334, 356);
            PotentialType.Name = "PotentialType";
            PotentialType.Size = new Size(256, 40);
            PotentialType.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(266, 216);
            label5.Name = "label5";
            label5.Size = new Size(135, 30);
            label5.TabIndex = 9;
            label5.Text = "Environment";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(297, 603);
            label6.Name = "label6";
            label6.Size = new Size(82, 30);
            label6.TabIndex = 10;
            label6.Text = "Particle";
            // 
            // EnergyLevel
            // 
            EnergyLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            EnergyLevel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            EnergyLevel.FormattingEnabled = true;
            EnergyLevel.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            EnergyLevel.Location = new Point(342, 636);
            EnergyLevel.Name = "EnergyLevel";
            EnergyLevel.Size = new Size(121, 40);
            EnergyLevel.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label8.Location = new Point(200, 641);
            label8.Name = "label8";
            label8.Size = new Size(136, 30);
            label8.TabIndex = 13;
            label8.Text = "Energy Level";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(254, 438);
            label7.Name = "label7";
            label7.Size = new Size(163, 30);
            label7.TabIndex = 15;
            label7.Text = "Laboratory Size";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label9.Location = new Point(228, 485);
            label9.Name = "label9";
            label9.Size = new Size(118, 30);
            label9.TabIndex = 16;
            label9.Text = "Direction 1";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new Point(228, 527);
            label10.Name = "label10";
            label10.Size = new Size(118, 30);
            label10.TabIndex = 17;
            label10.Text = "Direction 2";
            // 
            // MinX
            // 
            MinX.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            MinX.Location = new Point(352, 482);
            MinX.Name = "MinX";
            MinX.PlaceholderText = "Min";
            MinX.Size = new Size(53, 36);
            MinX.TabIndex = 18;
            // 
            // MaxX
            // 
            MaxX.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            MaxX.Location = new Point(411, 482);
            MaxX.Name = "MaxX";
            MaxX.PlaceholderText = "Max";
            MaxX.Size = new Size(53, 36);
            MaxX.TabIndex = 19;
            // 
            // MinY
            // 
            MinY.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            MinY.Location = new Point(352, 524);
            MinY.Name = "MinY";
            MinY.PlaceholderText = "Min";
            MinY.Size = new Size(53, 36);
            MinY.TabIndex = 20;
            // 
            // MaxY
            // 
            MaxY.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            MaxY.Location = new Point(411, 524);
            MaxY.Name = "MaxY";
            MaxY.PlaceholderText = "Max";
            MaxY.Size = new Size(53, 36);
            MaxY.TabIndex = 21;
            // 
            // AzimuthalLevel
            // 
            AzimuthalLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            AzimuthalLevel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            AzimuthalLevel.FormattingEnabled = true;
            AzimuthalLevel.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            AzimuthalLevel.Location = new Point(380, 682);
            AzimuthalLevel.Name = "AzimuthalLevel";
            AzimuthalLevel.Size = new Size(121, 40);
            AzimuthalLevel.TabIndex = 23;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            label11.Location = new Point(180, 687);
            label11.Name = "label11";
            label11.Size = new Size(194, 30);
            label11.TabIndex = 22;
            label11.Text = "Azimuthal Number";
            // 
            // Simulate
            // 
            Simulate.AutoSize = true;
            Simulate.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            Simulate.Location = new Point(266, 745);
            Simulate.Name = "Simulate";
            Simulate.Size = new Size(153, 55);
            Simulate.TabIndex = 24;
            Simulate.Text = "Simulate";
            Simulate.UseVisualStyleBackColor = true;
            Simulate.Click += Simulate_Click;
            // 
            // Sandbox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(Simulate);
            Controls.Add(AzimuthalLevel);
            Controls.Add(label11);
            Controls.Add(MaxY);
            Controls.Add(MinY);
            Controls.Add(MaxX);
            Controls.Add(MinX);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label7);
            Controls.Add(EnergyLevel);
            Controls.Add(label8);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(PotentialType);
            Controls.Add(label4);
            Controls.Add(MovementConstraints);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(CoordinateSystem);
            Controls.Add(label1);
            Controls.Add(listView1);
            Controls.Add(formsPlot1);
            DoubleBuffered = true;
            MaximizeBox = false;
            Name = "Sandbox";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quantum Sandbox";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.FormsPlot formsPlot1;
        private System.Windows.Forms.Timer timer1;
        private ListView listView1;
        private Label label1;
        private ComboBox CoordinateSystem;
        private Label label2;
        private Label label3;
        private ComboBox MovementConstraints;
        private Label label4;
        private ComboBox PotentialType;
        private Label label5;
        private Label label6;
        private ComboBox ParticleType;
        private ComboBox EnergyLevel;
        private Label label8;
        private Label label7;
        private Label label9;
        private Label label10;
        private TextBox MinX;
        private TextBox MaxX;
        private TextBox MinY;
        private TextBox MaxY;
        private ComboBox AzimuthalLevel;
        private Label label11;
        private Button Simulate;
    }
}