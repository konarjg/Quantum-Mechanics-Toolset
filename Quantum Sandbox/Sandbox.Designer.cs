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
            MainGraph = new ScottPlot.FormsPlot();
            LoadingTimer = new System.Windows.Forms.Timer(components);
            Parameters = new ListView();
            ParametersTitle = new Label();
            PotentialTypeTitle = new Label();
            ScenarioDropdown = new ComboBox();
            EnvironmentTitle = new Label();
            ParticleTitle = new Label();
            EnergyLevel = new ComboBox();
            EnergyLevelTitle = new Label();
            AzimuthalLevel = new ComboBox();
            AzimuthalLevelTitle = new Label();
            Simulate = new Button();
            ErrorMessage = new Label();
            ToolsMenu = new ListView();
            ToolsTitle = new Label();
            WavefunctionTitle = new Label();
            GraphPositionSpace = new Button();
            GraphMomentumSpace = new Button();
            MeasurementsTitle = new Label();
            MeasurePosition = new CheckBox();
            MeasureMomentum = new CheckBox();
            MeasureAngularMomentum = new CheckBox();
            MeasureEnergy = new CheckBox();
            CalculationsTitle = new Label();
            ExpectedPositionX = new TextBox();
            CalculateExpectedPosition = new CheckBox();
            ExpectedMomentum = new TextBox();
            CalculateExpectedMomentum = new CheckBox();
            RevealParticle = new Button();
            Back = new Button();
            LoadingProgressBar = new ProgressBar();
            LoadingScreen = new ListView();
            LoadingTitle = new Label();
            LoadingMessage = new Label();
            CancelLoadingButton = new Button();
            ExpectedPositionY = new TextBox();
            Measure = new Button();
            Calculate = new Button();
            MainTimer = new System.Windows.Forms.Timer(components);
            checkBox1 = new CheckBox();
            SuspendLayout();
            // 
            // MainGraph
            // 
            MainGraph.Location = new Point(700, 12);
            MainGraph.Margin = new Padding(4, 3, 4, 3);
            MainGraph.Name = "MainGraph";
            MainGraph.Size = new Size(1000, 1000);
            MainGraph.TabIndex = 0;
            // 
            // LoadingTimer
            // 
            LoadingTimer.Enabled = true;
            LoadingTimer.Interval = 300;
            LoadingTimer.Tick += LoadingTimer_Tick;
            // 
            // Parameters
            // 
            Parameters.Location = new Point(54, 134);
            Parameters.Name = "Parameters";
            Parameters.Size = new Size(602, 617);
            Parameters.TabIndex = 1;
            Parameters.UseCompatibleStateImageBehavior = false;
            // 
            // ParametersTitle
            // 
            ParametersTitle.Anchor = AnchorStyles.Top;
            ParametersTitle.AutoSize = true;
            ParametersTitle.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            ParametersTitle.Location = new Point(180, 155);
            ParametersTitle.Name = "ParametersTitle";
            ParametersTitle.Size = new Size(338, 45);
            ParametersTitle.TabIndex = 2;
            ParametersTitle.Text = "Simulation Parameters";
            // 
            // PotentialTypeTitle
            // 
            PotentialTypeTitle.AutoSize = true;
            PotentialTypeTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            PotentialTypeTitle.Location = new Point(175, 261);
            PotentialTypeTitle.Name = "PotentialTypeTitle";
            PotentialTypeTitle.Size = new Size(96, 30);
            PotentialTypeTitle.TabIndex = 7;
            PotentialTypeTitle.Text = "Scenario";
            // 
            // ScenarioDropdown
            // 
            ScenarioDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            ScenarioDropdown.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ScenarioDropdown.Items.AddRange(new object[] { "Infinite rectangular well", "Harmonic oscillator", "Finite rectangular well", "Delta potential", "Hydrogen atom", "Hall effect" });
            ScenarioDropdown.Location = new Point(281, 256);
            ScenarioDropdown.Name = "ScenarioDropdown";
            ScenarioDropdown.Size = new Size(256, 40);
            ScenarioDropdown.TabIndex = 8;
            // 
            // EnvironmentTitle
            // 
            EnvironmentTitle.AutoSize = true;
            EnvironmentTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            EnvironmentTitle.Location = new Point(266, 216);
            EnvironmentTitle.Name = "EnvironmentTitle";
            EnvironmentTitle.Size = new Size(135, 30);
            EnvironmentTitle.TabIndex = 9;
            EnvironmentTitle.Text = "Environment";
            // 
            // ParticleTitle
            // 
            ParticleTitle.AutoSize = true;
            ParticleTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            ParticleTitle.Location = new Point(295, 359);
            ParticleTitle.Name = "ParticleTitle";
            ParticleTitle.Size = new Size(82, 30);
            ParticleTitle.TabIndex = 10;
            ParticleTitle.Text = "Particle";
            // 
            // EnergyLevel
            // 
            EnergyLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            EnergyLevel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            EnergyLevel.FormattingEnabled = true;
            EnergyLevel.Items.AddRange(new object[] { "1", "2", "3", "4" });
            EnergyLevel.Location = new Point(340, 392);
            EnergyLevel.Name = "EnergyLevel";
            EnergyLevel.Size = new Size(121, 40);
            EnergyLevel.TabIndex = 14;
            // 
            // EnergyLevelTitle
            // 
            EnergyLevelTitle.AutoSize = true;
            EnergyLevelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            EnergyLevelTitle.Location = new Point(198, 397);
            EnergyLevelTitle.Name = "EnergyLevelTitle";
            EnergyLevelTitle.Size = new Size(136, 30);
            EnergyLevelTitle.TabIndex = 13;
            EnergyLevelTitle.Text = "Energy Level";
            // 
            // AzimuthalLevel
            // 
            AzimuthalLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            AzimuthalLevel.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            AzimuthalLevel.FormattingEnabled = true;
            AzimuthalLevel.Items.AddRange(new object[] { "0", "1", "2", "3" });
            AzimuthalLevel.Location = new Point(378, 438);
            AzimuthalLevel.Name = "AzimuthalLevel";
            AzimuthalLevel.Size = new Size(121, 40);
            AzimuthalLevel.TabIndex = 23;
            // 
            // AzimuthalLevelTitle
            // 
            AzimuthalLevelTitle.AutoSize = true;
            AzimuthalLevelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            AzimuthalLevelTitle.Location = new Point(178, 443);
            AzimuthalLevelTitle.Name = "AzimuthalLevelTitle";
            AzimuthalLevelTitle.Size = new Size(194, 30);
            AzimuthalLevelTitle.TabIndex = 22;
            AzimuthalLevelTitle.Text = "Azimuthal Number";
            // 
            // Simulate
            // 
            Simulate.AutoSize = true;
            Simulate.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            Simulate.Location = new Point(271, 495);
            Simulate.Name = "Simulate";
            Simulate.Size = new Size(153, 55);
            Simulate.TabIndex = 24;
            Simulate.Text = "Simulate";
            Simulate.UseVisualStyleBackColor = true;
            Simulate.Click += Simulate_Click;
            // 
            // ErrorMessage
            // 
            ErrorMessage.AutoSize = true;
            ErrorMessage.BackColor = SystemColors.Window;
            ErrorMessage.Enabled = false;
            ErrorMessage.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ErrorMessage.ForeColor = Color.Red;
            ErrorMessage.Location = new Point(228, 562);
            ErrorMessage.Name = "ErrorMessage";
            ErrorMessage.Size = new Size(240, 32);
            ErrorMessage.TabIndex = 25;
            ErrorMessage.Text = "Incorrect parameters!";
            ErrorMessage.Visible = false;
            // 
            // ToolsMenu
            // 
            ToolsMenu.Location = new Point(27, 71);
            ToolsMenu.Name = "ToolsMenu";
            ToolsMenu.Size = new Size(639, 838);
            ToolsMenu.TabIndex = 26;
            ToolsMenu.UseCompatibleStateImageBehavior = false;
            // 
            // ToolsTitle
            // 
            ToolsTitle.AutoSize = true;
            ToolsTitle.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            ToolsTitle.Location = new Point(241, 98);
            ToolsTitle.Name = "ToolsTitle";
            ToolsTitle.Size = new Size(212, 37);
            ToolsTitle.TabIndex = 27;
            ToolsTitle.Text = "Simulation Tools";
            // 
            // WavefunctionTitle
            // 
            WavefunctionTitle.AutoSize = true;
            WavefunctionTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            WavefunctionTitle.Location = new Point(265, 149);
            WavefunctionTitle.Name = "WavefunctionTitle";
            WavefunctionTitle.Size = new Size(161, 32);
            WavefunctionTitle.TabIndex = 28;
            WavefunctionTitle.Text = "Wavefunction";
            // 
            // GraphPositionSpace
            // 
            GraphPositionSpace.AutoSize = true;
            GraphPositionSpace.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            GraphPositionSpace.Location = new Point(208, 187);
            GraphPositionSpace.Name = "GraphPositionSpace";
            GraphPositionSpace.Size = new Size(249, 63);
            GraphPositionSpace.TabIndex = 29;
            GraphPositionSpace.Text = "Graph Position Space";
            GraphPositionSpace.UseVisualStyleBackColor = true;
            GraphPositionSpace.Click += GraphPositionSpace_Click;
            // 
            // GraphMomentumSpace
            // 
            GraphMomentumSpace.AutoSize = true;
            GraphMomentumSpace.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            GraphMomentumSpace.Location = new Point(191, 255);
            GraphMomentumSpace.Name = "GraphMomentumSpace";
            GraphMomentumSpace.Size = new Size(292, 63);
            GraphMomentumSpace.TabIndex = 30;
            GraphMomentumSpace.Text = "Graph Momentum Space";
            GraphMomentumSpace.UseVisualStyleBackColor = true;
            GraphMomentumSpace.Click += GraphMomentumSpace_Click;
            // 
            // MeasurementsTitle
            // 
            MeasurementsTitle.AutoSize = true;
            MeasurementsTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasurementsTitle.Location = new Point(237, 358);
            MeasurementsTitle.Name = "MeasurementsTitle";
            MeasurementsTitle.Size = new Size(172, 32);
            MeasurementsTitle.TabIndex = 31;
            MeasurementsTitle.Text = "Measurements";
            // 
            // MeasurePosition
            // 
            MeasurePosition.AutoSize = true;
            MeasurePosition.BackColor = SystemColors.Window;
            MeasurePosition.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasurePosition.Location = new Point(259, 404);
            MeasurePosition.Name = "MeasurePosition";
            MeasurePosition.Size = new Size(117, 36);
            MeasurePosition.TabIndex = 32;
            MeasurePosition.Text = "Position";
            MeasurePosition.UseVisualStyleBackColor = false;
            // 
            // MeasureMomentum
            // 
            MeasureMomentum.AutoSize = true;
            MeasureMomentum.BackColor = SystemColors.Window;
            MeasureMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasureMomentum.Location = new Point(237, 446);
            MeasureMomentum.Name = "MeasureMomentum";
            MeasureMomentum.Size = new Size(160, 36);
            MeasureMomentum.TabIndex = 33;
            MeasureMomentum.Text = "Momentum";
            MeasureMomentum.UseVisualStyleBackColor = false;
            // 
            // MeasureAngularMomentum
            // 
            MeasureAngularMomentum.AutoSize = true;
            MeasureAngularMomentum.BackColor = SystemColors.Window;
            MeasureAngularMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasureAngularMomentum.Location = new Point(196, 531);
            MeasureAngularMomentum.Name = "MeasureAngularMomentum";
            MeasureAngularMomentum.Size = new Size(250, 36);
            MeasureAngularMomentum.TabIndex = 36;
            MeasureAngularMomentum.Text = "Angular Momentum";
            MeasureAngularMomentum.UseVisualStyleBackColor = false;
            // 
            // MeasureEnergy
            // 
            MeasureEnergy.AutoSize = true;
            MeasureEnergy.BackColor = SystemColors.Window;
            MeasureEnergy.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasureEnergy.Location = new Point(271, 489);
            MeasureEnergy.Name = "MeasureEnergy";
            MeasureEnergy.Size = new Size(106, 36);
            MeasureEnergy.TabIndex = 38;
            MeasureEnergy.Text = "Energy";
            MeasureEnergy.UseVisualStyleBackColor = false;
            // 
            // CalculationsTitle
            // 
            CalculationsTitle.AutoSize = true;
            CalculationsTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CalculationsTitle.Location = new Point(250, 667);
            CalculationsTitle.Name = "CalculationsTitle";
            CalculationsTitle.Size = new Size(142, 32);
            CalculationsTitle.TabIndex = 42;
            CalculationsTitle.Text = "Calculations";
            // 
            // ExpectedPositionX
            // 
            ExpectedPositionX.Enabled = false;
            ExpectedPositionX.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ExpectedPositionX.Location = new Point(330, 710);
            ExpectedPositionX.Name = "ExpectedPositionX";
            ExpectedPositionX.ReadOnly = true;
            ExpectedPositionX.Size = new Size(107, 39);
            ExpectedPositionX.TabIndex = 44;
            // 
            // CalculateExpectedPosition
            // 
            CalculateExpectedPosition.AutoSize = true;
            CalculateExpectedPosition.BackColor = SystemColors.Window;
            CalculateExpectedPosition.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CalculateExpectedPosition.Location = new Point(107, 713);
            CalculateExpectedPosition.Name = "CalculateExpectedPosition";
            CalculateExpectedPosition.Size = new Size(220, 36);
            CalculateExpectedPosition.TabIndex = 43;
            CalculateExpectedPosition.Text = "Expected Position";
            CalculateExpectedPosition.UseVisualStyleBackColor = false;
            // 
            // ExpectedMomentum
            // 
            ExpectedMomentum.Enabled = false;
            ExpectedMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ExpectedMomentum.Location = new Point(429, 754);
            ExpectedMomentum.Name = "ExpectedMomentum";
            ExpectedMomentum.ReadOnly = true;
            ExpectedMomentum.Size = new Size(108, 39);
            ExpectedMomentum.TabIndex = 46;
            // 
            // CalculateExpectedMomentum
            // 
            CalculateExpectedMomentum.AutoSize = true;
            CalculateExpectedMomentum.BackColor = SystemColors.Window;
            CalculateExpectedMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CalculateExpectedMomentum.Location = new Point(154, 757);
            CalculateExpectedMomentum.Name = "CalculateExpectedMomentum";
            CalculateExpectedMomentum.Size = new Size(263, 36);
            CalculateExpectedMomentum.TabIndex = 45;
            CalculateExpectedMomentum.Text = "Expected Momentum";
            CalculateExpectedMomentum.UseVisualStyleBackColor = false;
            // 
            // RevealParticle
            // 
            RevealParticle.AutoSize = true;
            RevealParticle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            RevealParticle.Location = new Point(314, 581);
            RevealParticle.Name = "RevealParticle";
            RevealParticle.Size = new Size(176, 53);
            RevealParticle.TabIndex = 47;
            RevealParticle.Text = "Reveal Particle";
            RevealParticle.UseVisualStyleBackColor = true;
            RevealParticle.Click += RevealParticle_Click;
            // 
            // Back
            // 
            Back.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            Back.Location = new Point(35, 85);
            Back.Name = "Back";
            Back.Size = new Size(78, 56);
            Back.TabIndex = 48;
            Back.Text = "Back";
            Back.UseVisualStyleBackColor = true;
            Back.Click += Back_Click;
            // 
            // LoadingProgressBar
            // 
            LoadingProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LoadingProgressBar.Location = new Point(761, 401);
            LoadingProgressBar.MarqueeAnimationSpeed = 40;
            LoadingProgressBar.Name = "LoadingProgressBar";
            LoadingProgressBar.Size = new Size(393, 23);
            LoadingProgressBar.Style = ProgressBarStyle.Marquee;
            LoadingProgressBar.TabIndex = 50;
            // 
            // LoadingScreen
            // 
            LoadingScreen.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LoadingScreen.Location = new Point(686, 290);
            LoadingScreen.Name = "LoadingScreen";
            LoadingScreen.Size = new Size(540, 267);
            LoadingScreen.TabIndex = 51;
            LoadingScreen.UseCompatibleStateImageBehavior = false;
            // 
            // LoadingTitle
            // 
            LoadingTitle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LoadingTitle.AutoSize = true;
            LoadingTitle.BackColor = SystemColors.Window;
            LoadingTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            LoadingTitle.Location = new Point(822, 359);
            LoadingTitle.Name = "LoadingTitle";
            LoadingTitle.Size = new Size(300, 32);
            LoadingTitle.TabIndex = 52;
            LoadingTitle.Text = "Setting up the simulation...";
            LoadingTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoadingMessage
            // 
            LoadingMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LoadingMessage.AutoSize = true;
            LoadingMessage.BackColor = SystemColors.Window;
            LoadingMessage.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            LoadingMessage.Location = new Point(699, 436);
            LoadingMessage.Name = "LoadingMessage";
            LoadingMessage.Size = new Size(520, 32);
            LoadingMessage.TabIndex = 53;
            LoadingMessage.Text = "This may take some time for complex scenarios";
            LoadingMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CancelLoadingButton
            // 
            CancelLoadingButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CancelLoadingButton.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CancelLoadingButton.Location = new Point(900, 482);
            CancelLoadingButton.Name = "CancelLoadingButton";
            CancelLoadingButton.Size = new Size(117, 51);
            CancelLoadingButton.TabIndex = 54;
            CancelLoadingButton.Text = "Cancel";
            CancelLoadingButton.UseVisualStyleBackColor = true;
            CancelLoadingButton.Click += CancelLoadingButton_Click;
            // 
            // ExpectedPositionY
            // 
            ExpectedPositionY.Enabled = false;
            ExpectedPositionY.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ExpectedPositionY.Location = new Point(445, 710);
            ExpectedPositionY.Name = "ExpectedPositionY";
            ExpectedPositionY.ReadOnly = true;
            ExpectedPositionY.Size = new Size(107, 39);
            ExpectedPositionY.TabIndex = 56;
            // 
            // Measure
            // 
            Measure.AutoSize = true;
            Measure.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            Measure.Location = new Point(175, 581);
            Measure.Name = "Measure";
            Measure.Size = new Size(131, 53);
            Measure.TabIndex = 57;
            Measure.Text = "Measure";
            Measure.UseVisualStyleBackColor = true;
            Measure.Click += Measure_Click;
            // 
            // Calculate
            // 
            Calculate.AutoSize = true;
            Calculate.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            Calculate.Location = new Point(269, 801);
            Calculate.Name = "Calculate";
            Calculate.Size = new Size(131, 53);
            Calculate.TabIndex = 58;
            Calculate.Text = "Calculate";
            Calculate.UseVisualStyleBackColor = true;
            Calculate.Click += Calculate_Click;
            // 
            // MainTimer
            // 
            MainTimer.Enabled = true;
            MainTimer.Interval = 1;
            MainTimer.Tick += MainTimer_Tick;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            checkBox1.Location = new Point(241, 302);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(190, 34);
            checkBox1.TabIndex = 59;
            checkBox1.Text = "Two Dimensional";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // Sandbox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(checkBox1);
            Controls.Add(CancelLoadingButton);
            Controls.Add(LoadingMessage);
            Controls.Add(LoadingTitle);
            Controls.Add(LoadingProgressBar);
            Controls.Add(LoadingScreen);
            Controls.Add(ErrorMessage);
            Controls.Add(Simulate);
            Controls.Add(AzimuthalLevel);
            Controls.Add(AzimuthalLevelTitle);
            Controls.Add(EnergyLevel);
            Controls.Add(EnergyLevelTitle);
            Controls.Add(ParticleTitle);
            Controls.Add(EnvironmentTitle);
            Controls.Add(ScenarioDropdown);
            Controls.Add(PotentialTypeTitle);
            Controls.Add(ParametersTitle);
            Controls.Add(MainGraph);
            Controls.Add(Parameters);
            Controls.Add(Calculate);
            Controls.Add(Measure);
            Controls.Add(ExpectedPositionY);
            Controls.Add(Back);
            Controls.Add(RevealParticle);
            Controls.Add(ExpectedMomentum);
            Controls.Add(CalculateExpectedMomentum);
            Controls.Add(ExpectedPositionX);
            Controls.Add(CalculateExpectedPosition);
            Controls.Add(CalculationsTitle);
            Controls.Add(MeasureEnergy);
            Controls.Add(MeasureAngularMomentum);
            Controls.Add(MeasureMomentum);
            Controls.Add(MeasurePosition);
            Controls.Add(MeasurementsTitle);
            Controls.Add(GraphMomentumSpace);
            Controls.Add(GraphPositionSpace);
            Controls.Add(WavefunctionTitle);
            Controls.Add(ToolsTitle);
            Controls.Add(ToolsMenu);
            DoubleBuffered = true;
            MaximizeBox = false;
            Name = "Sandbox";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quantum Sandbox";
            WindowState = FormWindowState.Maximized;
            FormClosed += Sandbox_FormClosed;
            Load += Sandbox_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.FormsPlot MainGraph;
        private System.Windows.Forms.Timer LoadingTimer;
        private ListView Parameters;
        private Label ParametersTitle;
        private Label PotentialTypeTitle;
        private ComboBox ScenarioDropdown;
        private Label EnvironmentTitle;
        private Label ParticleTitle;
        private ComboBox EnergyLevel;
        private Label EnergyLevelTitle;
        private ComboBox AzimuthalLevel;
        private Label AzimuthalLevelTitle;
        private Button Simulate;
        private Label ErrorMessage;
        private ListView ToolsMenu;
        private Label ToolsTitle;
        private Label WavefunctionTitle;
        private Button GraphPositionSpace;
        private Button GraphMomentumSpace;
        private Label MeasurementsTitle;
        private CheckBox MeasurePosition;
        private CheckBox MeasureMomentum;
        private CheckBox MeasureAngularMomentum;
        private CheckBox MeasureEnergy;
        private Label CalculationsTitle;
        private TextBox ExpectedPositionX;
        private CheckBox CalculateExpectedPosition;
        private TextBox ExpectedMomentum;
        private CheckBox CalculateExpectedMomentum;
        private Button RevealParticle;
        private Button Back;
        private ProgressBar LoadingProgressBar;
        private ListView LoadingScreen;
        private Label LoadingTitle;
        private Label LoadingMessage;
        private Button CancelLoadingButton;
        private TextBox ExpectedPositionY;
        private Button Measure;
        private Button Calculate;
        private System.Windows.Forms.Timer MainTimer;
        private CheckBox checkBox1;
    }
}