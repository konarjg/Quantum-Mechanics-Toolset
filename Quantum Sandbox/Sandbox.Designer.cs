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
            CoordinateSystem = new ComboBox();
            CoordinateSystemTitle = new Label();
            MovementConstraintsTitle = new Label();
            MovementConstraints = new ComboBox();
            PotentialTypeTitle = new Label();
            PotentialType = new ComboBox();
            EnvironmentTitle = new Label();
            ParticleTitle = new Label();
            EnergyLevel = new ComboBox();
            EnergyLevelTitle = new Label();
            LaboratorySizeTitle = new Label();
            Direction1Title = new Label();
            Direction2Title = new Label();
            MinX = new TextBox();
            MaxX = new TextBox();
            MinY = new TextBox();
            MaxY = new TextBox();
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
            PositionMeasurement = new TextBox();
            MomentumMeasurement = new TextBox();
            MeasureAngularMomentum = new CheckBox();
            AngularMomentumMeasurement = new TextBox();
            MeasureEnergy = new CheckBox();
            EnergyMeasurement = new TextBox();
            CalculationsTitle = new Label();
            ExpectedPosition = new TextBox();
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
            Parameters.Size = new Size(602, 775);
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
            // CoordinateSystemTitle
            // 
            CoordinateSystemTitle.AutoSize = true;
            CoordinateSystemTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            CoordinateSystemTitle.Location = new Point(143, 262);
            CoordinateSystemTitle.Name = "CoordinateSystemTitle";
            CoordinateSystemTitle.Size = new Size(196, 30);
            CoordinateSystemTitle.TabIndex = 4;
            CoordinateSystemTitle.Text = "Coordinate System";
            // 
            // MovementConstraintsTitle
            // 
            MovementConstraintsTitle.AutoSize = true;
            MovementConstraintsTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            MovementConstraintsTitle.Location = new Point(96, 308);
            MovementConstraintsTitle.Name = "MovementConstraintsTitle";
            MovementConstraintsTitle.Size = new Size(232, 30);
            MovementConstraintsTitle.TabIndex = 5;
            MovementConstraintsTitle.Text = "Movement Constraints";
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
            // PotentialTypeTitle
            // 
            PotentialTypeTitle.AutoSize = true;
            PotentialTypeTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            PotentialTypeTitle.Location = new Point(96, 361);
            PotentialTypeTitle.Name = "PotentialTypeTitle";
            PotentialTypeTitle.Size = new Size(231, 30);
            PotentialTypeTitle.TabIndex = 7;
            PotentialTypeTitle.Text = "External Potential Type";
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
            ParticleTitle.Location = new Point(297, 603);
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
            EnergyLevel.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            EnergyLevel.Location = new Point(342, 636);
            EnergyLevel.Name = "EnergyLevel";
            EnergyLevel.Size = new Size(121, 40);
            EnergyLevel.TabIndex = 14;
            // 
            // EnergyLevelTitle
            // 
            EnergyLevelTitle.AutoSize = true;
            EnergyLevelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            EnergyLevelTitle.Location = new Point(200, 641);
            EnergyLevelTitle.Name = "EnergyLevelTitle";
            EnergyLevelTitle.Size = new Size(136, 30);
            EnergyLevelTitle.TabIndex = 13;
            EnergyLevelTitle.Text = "Energy Level";
            // 
            // LaboratorySizeTitle
            // 
            LaboratorySizeTitle.AutoSize = true;
            LaboratorySizeTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            LaboratorySizeTitle.Location = new Point(263, 438);
            LaboratorySizeTitle.Name = "LaboratorySizeTitle";
            LaboratorySizeTitle.Size = new Size(163, 30);
            LaboratorySizeTitle.TabIndex = 15;
            LaboratorySizeTitle.Text = "Laboratory Size";
            // 
            // Direction1Title
            // 
            Direction1Title.AutoSize = true;
            Direction1Title.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            Direction1Title.Location = new Point(228, 485);
            Direction1Title.Name = "Direction1Title";
            Direction1Title.Size = new Size(118, 30);
            Direction1Title.TabIndex = 16;
            Direction1Title.Text = "Direction 1";
            // 
            // Direction2Title
            // 
            Direction2Title.AutoSize = true;
            Direction2Title.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            Direction2Title.Location = new Point(228, 527);
            Direction2Title.Name = "Direction2Title";
            Direction2Title.Size = new Size(118, 30);
            Direction2Title.TabIndex = 17;
            Direction2Title.Text = "Direction 2";
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
            // AzimuthalLevelTitle
            // 
            AzimuthalLevelTitle.AutoSize = true;
            AzimuthalLevelTitle.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            AzimuthalLevelTitle.Location = new Point(180, 687);
            AzimuthalLevelTitle.Name = "AzimuthalLevelTitle";
            AzimuthalLevelTitle.Size = new Size(194, 30);
            AzimuthalLevelTitle.TabIndex = 22;
            AzimuthalLevelTitle.Text = "Azimuthal Number";
            // 
            // Simulate
            // 
            Simulate.AutoSize = true;
            Simulate.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
            Simulate.Location = new Point(266, 760);
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
            ErrorMessage.Location = new Point(223, 827);
            ErrorMessage.Name = "ErrorMessage";
            ErrorMessage.Size = new Size(240, 32);
            ErrorMessage.TabIndex = 25;
            ErrorMessage.Text = "Incorrect parameters!";
            ErrorMessage.Visible = false;
            // 
            // ToolsMenu
            // 
            ToolsMenu.Location = new Point(54, 91);
            ToolsMenu.Name = "ToolsMenu";
            ToolsMenu.Size = new Size(639, 838);
            ToolsMenu.TabIndex = 26;
            ToolsMenu.UseCompatibleStateImageBehavior = false;
            // 
            // ToolsTitle
            // 
            ToolsTitle.AutoSize = true;
            ToolsTitle.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            ToolsTitle.Location = new Point(273, 118);
            ToolsTitle.Name = "ToolsTitle";
            ToolsTitle.Size = new Size(212, 37);
            ToolsTitle.TabIndex = 27;
            ToolsTitle.Text = "Simulation Tools";
            // 
            // WavefunctionTitle
            // 
            WavefunctionTitle.AutoSize = true;
            WavefunctionTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            WavefunctionTitle.Location = new Point(297, 169);
            WavefunctionTitle.Name = "WavefunctionTitle";
            WavefunctionTitle.Size = new Size(161, 32);
            WavefunctionTitle.TabIndex = 28;
            WavefunctionTitle.Text = "Wavefunction";
            // 
            // GraphPositionSpace
            // 
            GraphPositionSpace.AutoSize = true;
            GraphPositionSpace.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            GraphPositionSpace.Location = new Point(240, 207);
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
            GraphMomentumSpace.Location = new Point(223, 275);
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
            MeasurementsTitle.Location = new Point(269, 358);
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
            MeasurePosition.Location = new Point(288, 401);
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
            MeasureMomentum.Location = new Point(257, 446);
            MeasureMomentum.Name = "MeasureMomentum";
            MeasureMomentum.Size = new Size(160, 36);
            MeasureMomentum.TabIndex = 33;
            MeasureMomentum.Text = "Momentum";
            MeasureMomentum.UseVisualStyleBackColor = false;
            // 
            // PositionMeasurement
            // 
            PositionMeasurement.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            PositionMeasurement.Location = new Point(411, 399);
            PositionMeasurement.Name = "PositionMeasurement";
            PositionMeasurement.Size = new Size(60, 39);
            PositionMeasurement.TabIndex = 34;
            // 
            // MomentumMeasurement
            // 
            MomentumMeasurement.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MomentumMeasurement.Location = new Point(425, 445);
            MomentumMeasurement.Name = "MomentumMeasurement";
            MomentumMeasurement.Size = new Size(60, 39);
            MomentumMeasurement.TabIndex = 35;
            // 
            // MeasureAngularMomentum
            // 
            MeasureAngularMomentum.AutoSize = true;
            MeasureAngularMomentum.BackColor = SystemColors.Window;
            MeasureAngularMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasureAngularMomentum.Location = new Point(213, 490);
            MeasureAngularMomentum.Name = "MeasureAngularMomentum";
            MeasureAngularMomentum.Size = new Size(250, 36);
            MeasureAngularMomentum.TabIndex = 36;
            MeasureAngularMomentum.Text = "Angular Momentum";
            MeasureAngularMomentum.UseVisualStyleBackColor = false;
            // 
            // AngularMomentumMeasurement
            // 
            AngularMomentumMeasurement.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            AngularMomentumMeasurement.Location = new Point(469, 489);
            AngularMomentumMeasurement.Name = "AngularMomentumMeasurement";
            AngularMomentumMeasurement.Size = new Size(60, 39);
            AngularMomentumMeasurement.TabIndex = 37;
            // 
            // MeasureEnergy
            // 
            MeasureEnergy.AutoSize = true;
            MeasureEnergy.BackColor = SystemColors.Window;
            MeasureEnergy.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            MeasureEnergy.Location = new Point(299, 537);
            MeasureEnergy.Name = "MeasureEnergy";
            MeasureEnergy.Size = new Size(106, 36);
            MeasureEnergy.TabIndex = 38;
            MeasureEnergy.Text = "Energy";
            MeasureEnergy.UseVisualStyleBackColor = false;
            // 
            // EnergyMeasurement
            // 
            EnergyMeasurement.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            EnergyMeasurement.Location = new Point(411, 536);
            EnergyMeasurement.Name = "EnergyMeasurement";
            EnergyMeasurement.Size = new Size(60, 39);
            EnergyMeasurement.TabIndex = 39;
            // 
            // CalculationsTitle
            // 
            CalculationsTitle.AutoSize = true;
            CalculationsTitle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CalculationsTitle.Location = new Point(282, 596);
            CalculationsTitle.Name = "CalculationsTitle";
            CalculationsTitle.Size = new Size(142, 32);
            CalculationsTitle.TabIndex = 42;
            CalculationsTitle.Text = "Calculations";
            // 
            // ExpectedPosition
            // 
            ExpectedPosition.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ExpectedPosition.Location = new Point(434, 634);
            ExpectedPosition.Name = "ExpectedPosition";
            ExpectedPosition.Size = new Size(60, 39);
            ExpectedPosition.TabIndex = 44;
            // 
            // CalculateExpectedPosition
            // 
            CalculateExpectedPosition.AutoSize = true;
            CalculateExpectedPosition.BackColor = SystemColors.Window;
            CalculateExpectedPosition.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CalculateExpectedPosition.Location = new Point(211, 637);
            CalculateExpectedPosition.Name = "CalculateExpectedPosition";
            CalculateExpectedPosition.Size = new Size(220, 36);
            CalculateExpectedPosition.TabIndex = 43;
            CalculateExpectedPosition.Text = "Expected Position";
            CalculateExpectedPosition.UseVisualStyleBackColor = false;
            // 
            // ExpectedMomentum
            // 
            ExpectedMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            ExpectedMomentum.Location = new Point(461, 683);
            ExpectedMomentum.Name = "ExpectedMomentum";
            ExpectedMomentum.Size = new Size(60, 39);
            ExpectedMomentum.TabIndex = 46;
            // 
            // CalculateExpectedMomentum
            // 
            CalculateExpectedMomentum.AutoSize = true;
            CalculateExpectedMomentum.BackColor = SystemColors.Window;
            CalculateExpectedMomentum.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CalculateExpectedMomentum.Location = new Point(186, 686);
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
            RevealParticle.Location = new Point(273, 752);
            RevealParticle.Name = "RevealParticle";
            RevealParticle.Size = new Size(176, 63);
            RevealParticle.TabIndex = 47;
            RevealParticle.Text = "Reveal Particle";
            RevealParticle.UseVisualStyleBackColor = true;
            // 
            // Back
            // 
            Back.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            Back.Location = new Point(67, 105);
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
            LoadingScreen.Location = new Point(686, 290);
            LoadingScreen.Name = "LoadingScreen";
            LoadingScreen.Size = new Size(540, 267);
            LoadingScreen.TabIndex = 51;
            LoadingScreen.UseCompatibleStateImageBehavior = false;
            // 
            // LoadingTitle
            // 
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
            CancelLoadingButton.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            CancelLoadingButton.Location = new Point(900, 482);
            CancelLoadingButton.Name = "CancelLoadingButton";
            CancelLoadingButton.Size = new Size(117, 51);
            CancelLoadingButton.TabIndex = 54;
            CancelLoadingButton.Text = "Cancel";
            CancelLoadingButton.UseVisualStyleBackColor = true;
            CancelLoadingButton.Click += CancelLoadingButton_Click;
            // 
            // Sandbox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(CancelLoadingButton);
            Controls.Add(LoadingMessage);
            Controls.Add(LoadingTitle);
            Controls.Add(LoadingProgressBar);
            Controls.Add(LoadingScreen);
            Controls.Add(Back);
            Controls.Add(RevealParticle);
            Controls.Add(ExpectedMomentum);
            Controls.Add(CalculateExpectedMomentum);
            Controls.Add(ExpectedPosition);
            Controls.Add(CalculateExpectedPosition);
            Controls.Add(CalculationsTitle);
            Controls.Add(EnergyMeasurement);
            Controls.Add(MeasureEnergy);
            Controls.Add(AngularMomentumMeasurement);
            Controls.Add(MeasureAngularMomentum);
            Controls.Add(MomentumMeasurement);
            Controls.Add(PositionMeasurement);
            Controls.Add(MeasureMomentum);
            Controls.Add(MeasurePosition);
            Controls.Add(MeasurementsTitle);
            Controls.Add(GraphMomentumSpace);
            Controls.Add(GraphPositionSpace);
            Controls.Add(WavefunctionTitle);
            Controls.Add(ToolsTitle);
            Controls.Add(ToolsMenu);
            Controls.Add(ErrorMessage);
            Controls.Add(Simulate);
            Controls.Add(AzimuthalLevel);
            Controls.Add(AzimuthalLevelTitle);
            Controls.Add(MaxY);
            Controls.Add(MinY);
            Controls.Add(MaxX);
            Controls.Add(MinX);
            Controls.Add(Direction2Title);
            Controls.Add(Direction1Title);
            Controls.Add(LaboratorySizeTitle);
            Controls.Add(EnergyLevel);
            Controls.Add(EnergyLevelTitle);
            Controls.Add(ParticleTitle);
            Controls.Add(EnvironmentTitle);
            Controls.Add(PotentialType);
            Controls.Add(PotentialTypeTitle);
            Controls.Add(MovementConstraints);
            Controls.Add(MovementConstraintsTitle);
            Controls.Add(CoordinateSystemTitle);
            Controls.Add(CoordinateSystem);
            Controls.Add(ParametersTitle);
            Controls.Add(MainGraph);
            Controls.Add(Parameters);
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

        private ScottPlot.FormsPlot MainGraph;
        private System.Windows.Forms.Timer LoadingTimer;
        private ListView Parameters;
        private Label ParametersTitle;
        private ComboBox CoordinateSystem;
        private Label CoordinateSystemTitle;
        private Label MovementConstraintsTitle;
        private ComboBox MovementConstraints;
        private Label PotentialTypeTitle;
        private ComboBox PotentialType;
        private Label EnvironmentTitle;
        private Label ParticleTitle;
        private ComboBox ParticleType;
        private ComboBox EnergyLevel;
        private Label EnergyLevelTitle;
        private Label LaboratorySizeTitle;
        private Label Direction1Title;
        private Label Direction2Title;
        private TextBox MinX;
        private TextBox MaxX;
        private TextBox MinY;
        private TextBox MaxY;
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
        private TextBox PositionMeasurement;
        private TextBox MomentumMeasurement;
        private CheckBox MeasureAngularMomentum;
        private TextBox AngularMomentumMeasurement;
        private CheckBox MeasureEnergy;
        private TextBox EnergyMeasurement;
        private Label CalculationsTitle;
        private TextBox ExpectedPosition;
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
    }
}