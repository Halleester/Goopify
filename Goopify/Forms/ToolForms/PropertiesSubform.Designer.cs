
namespace Goopify
{
    partial class PropertiesSubform
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Snapping");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Model Creation");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesSubform));
            this.goopModelGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.collisionTypesTextbox = new System.Windows.Forms.TextBox();
            this.ignoreCollisionCheckBox = new System.Windows.Forms.CheckBox();
            this.goopAngleNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.goopAngleLabel = new System.Windows.Forms.Label();
            this.floorOffsetNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.goopOffsetLabel = new System.Windows.Forms.Label();
            this.settingTreeView = new System.Windows.Forms.TreeView();
            this.modelPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.snapPanel = new System.Windows.Forms.Panel();
            this.gridSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.gridUnitSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.snapToGridCheckBox = new System.Windows.Forms.CheckBox();
            this.paintUndoLabel = new System.Windows.Forms.Label();
            this.regionSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.snapToPowersCheckBox = new System.Windows.Forms.CheckBox();
            this.cornerSnappingCheckBox = new System.Windows.Forms.CheckBox();
            this.edgeSnappingCheckBox = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.generalPanel = new System.Windows.Forms.Panel();
            this.devModeCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.undoStepsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.regionMaxNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.regionMinNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cameraSpeedNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.goopModelGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.goopAngleNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.floorOffsetNumericUpDown)).BeginInit();
            this.modelPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.snapPanel.SuspendLayout();
            this.gridSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnitSizeNumericUpDown)).BeginInit();
            this.regionSettingsGroupBox.SuspendLayout();
            this.panel3.SuspendLayout();
            this.generalPanel.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.undoStepsNumericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.regionMaxNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.regionMinNumericUpDown)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cameraSpeedNumericUpDown)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // goopModelGroupBox
            // 
            this.goopModelGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.goopModelGroupBox.Controls.Add(this.label1);
            this.goopModelGroupBox.Controls.Add(this.collisionTypesTextbox);
            this.goopModelGroupBox.Controls.Add(this.ignoreCollisionCheckBox);
            this.goopModelGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goopModelGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.goopModelGroupBox.Location = new System.Drawing.Point(12, 55);
            this.goopModelGroupBox.Name = "goopModelGroupBox";
            this.goopModelGroupBox.Size = new System.Drawing.Size(547, 206);
            this.goopModelGroupBox.TabIndex = 28;
            this.goopModelGroupBox.TabStop = false;
            this.goopModelGroupBox.Text = "Collision";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Location = new System.Drawing.Point(7, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 20);
            this.label1.TabIndex = 38;
            this.label1.Text = "Collisions To Ignore:";
            // 
            // collisionTypesTextbox
            // 
            this.collisionTypesTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(68)))), ((int)(((byte)(97)))));
            this.collisionTypesTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.collisionTypesTextbox.ForeColor = System.Drawing.Color.LightGray;
            this.collisionTypesTextbox.Location = new System.Drawing.Point(6, 60);
            this.collisionTypesTextbox.Multiline = true;
            this.collisionTypesTextbox.Name = "collisionTypesTextbox";
            this.collisionTypesTextbox.Size = new System.Drawing.Size(535, 107);
            this.collisionTypesTextbox.TabIndex = 34;
            this.collisionTypesTextbox.Text = "1, 2, 3, 4, 5, 6, 7, 8, 8";
            this.collisionTypesTextbox.Validated += new System.EventHandler(this.collisionTypesTextbox_Validated);
            // 
            // ignoreCollisionCheckBox
            // 
            this.ignoreCollisionCheckBox.AutoSize = true;
            this.ignoreCollisionCheckBox.Checked = true;
            this.ignoreCollisionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreCollisionCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ignoreCollisionCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.ignoreCollisionCheckBox.Location = new System.Drawing.Point(6, 173);
            this.ignoreCollisionCheckBox.Name = "ignoreCollisionCheckBox";
            this.ignoreCollisionCheckBox.Size = new System.Drawing.Size(182, 24);
            this.ignoreCollisionCheckBox.TabIndex = 33;
            this.ignoreCollisionCheckBox.Text = "Ignore These Values";
            this.ignoreCollisionCheckBox.UseVisualStyleBackColor = true;
            this.ignoreCollisionCheckBox.CheckedChanged += new System.EventHandler(this.ignoreCollisionCheckBox_CheckedChanged);
            // 
            // goopAngleNumericUpDown
            // 
            this.goopAngleNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goopAngleNumericUpDown.Location = new System.Drawing.Point(201, 30);
            this.goopAngleNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.goopAngleNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.goopAngleNumericUpDown.Name = "goopAngleNumericUpDown";
            this.goopAngleNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.goopAngleNumericUpDown.TabIndex = 38;
            this.goopAngleNumericUpDown.Value = new decimal(new int[] {
            85,
            0,
            0,
            0});
            this.goopAngleNumericUpDown.ValueChanged += new System.EventHandler(this.goopAngleNumericUpDown_ValueChanged);
            this.goopAngleNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.goopAngleNumericUpDown_KeyDown);
            this.goopAngleNumericUpDown.Validated += new System.EventHandler(this.goopAngleNumericUpDown_Validated);
            // 
            // goopAngleLabel
            // 
            this.goopAngleLabel.AutoSize = true;
            this.goopAngleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goopAngleLabel.ForeColor = System.Drawing.Color.LightGray;
            this.goopAngleLabel.Location = new System.Drawing.Point(7, 32);
            this.goopAngleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.goopAngleLabel.Name = "goopAngleLabel";
            this.goopAngleLabel.Size = new System.Drawing.Size(128, 20);
            this.goopAngleLabel.TabIndex = 37;
            this.goopAngleLabel.Text = "Angle Tolerance:";
            // 
            // floorOffsetNumericUpDown
            // 
            this.floorOffsetNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.floorOffsetNumericUpDown.Location = new System.Drawing.Point(201, 65);
            this.floorOffsetNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.floorOffsetNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.floorOffsetNumericUpDown.Name = "floorOffsetNumericUpDown";
            this.floorOffsetNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.floorOffsetNumericUpDown.TabIndex = 36;
            this.floorOffsetNumericUpDown.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.floorOffsetNumericUpDown.ValueChanged += new System.EventHandler(this.floorOffsetNumericUpDown_ValueChanged);
            this.floorOffsetNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.floorOffsetNumericUpDown_KeyDown);
            this.floorOffsetNumericUpDown.Validated += new System.EventHandler(this.floorOffsetNumericUpDown_Validated);
            // 
            // goopOffsetLabel
            // 
            this.goopOffsetLabel.AutoSize = true;
            this.goopOffsetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goopOffsetLabel.ForeColor = System.Drawing.Color.LightGray;
            this.goopOffsetLabel.Location = new System.Drawing.Point(7, 67);
            this.goopOffsetLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.goopOffsetLabel.Name = "goopOffsetLabel";
            this.goopOffsetLabel.Size = new System.Drawing.Size(133, 20);
            this.goopOffsetLabel.TabIndex = 35;
            this.goopOffsetLabel.Text = "Offset from Floor:";
            // 
            // settingTreeView
            // 
            this.settingTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(68)))), ((int)(((byte)(97)))));
            this.settingTreeView.ForeColor = System.Drawing.Color.LightGray;
            this.settingTreeView.Location = new System.Drawing.Point(12, 12);
            this.settingTreeView.Name = "settingTreeView";
            treeNode1.Name = "Node0";
            treeNode1.Text = "General";
            treeNode2.Name = "Node3";
            treeNode2.Text = "Snapping";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Model Creation";
            this.settingTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.settingTreeView.Size = new System.Drawing.Size(314, 550);
            this.settingTreeView.TabIndex = 31;
            this.settingTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.settingTreeView_AfterSelect);
            // 
            // modelPanel
            // 
            this.modelPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.modelPanel.Controls.Add(this.panel1);
            this.modelPanel.Controls.Add(this.groupBox1);
            this.modelPanel.Controls.Add(this.goopModelGroupBox);
            this.modelPanel.Location = new System.Drawing.Point(332, 13);
            this.modelPanel.Name = "modelPanel";
            this.modelPanel.Size = new System.Drawing.Size(570, 549);
            this.modelPanel.TabIndex = 32;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(41)))), ((int)(((byte)(58)))));
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 44);
            this.panel1.TabIndex = 40;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.LightGray;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(570, 44);
            this.label2.TabIndex = 39;
            this.label2.Text = "Model Creation";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.groupBox1.Controls.Add(this.goopAngleNumericUpDown);
            this.groupBox1.Controls.Add(this.goopAngleLabel);
            this.groupBox1.Controls.Add(this.goopOffsetLabel);
            this.groupBox1.Controls.Add(this.floorOffsetNumericUpDown);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox1.Location = new System.Drawing.Point(12, 277);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(547, 105);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Model Cutting";
            // 
            // snapPanel
            // 
            this.snapPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.snapPanel.Controls.Add(this.gridSettingsGroupBox);
            this.snapPanel.Controls.Add(this.regionSettingsGroupBox);
            this.snapPanel.Controls.Add(this.panel3);
            this.snapPanel.Location = new System.Drawing.Point(332, 13);
            this.snapPanel.Name = "snapPanel";
            this.snapPanel.Size = new System.Drawing.Size(570, 549);
            this.snapPanel.TabIndex = 33;
            this.snapPanel.Visible = false;
            // 
            // gridSettingsGroupBox
            // 
            this.gridSettingsGroupBox.Controls.Add(this.gridUnitSizeNumericUpDown);
            this.gridSettingsGroupBox.Controls.Add(this.snapToGridCheckBox);
            this.gridSettingsGroupBox.Controls.Add(this.paintUndoLabel);
            this.gridSettingsGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridSettingsGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.gridSettingsGroupBox.Location = new System.Drawing.Point(12, 201);
            this.gridSettingsGroupBox.Name = "gridSettingsGroupBox";
            this.gridSettingsGroupBox.Size = new System.Drawing.Size(547, 103);
            this.gridSettingsGroupBox.TabIndex = 42;
            this.gridSettingsGroupBox.TabStop = false;
            this.gridSettingsGroupBox.Text = "Grid Snapping";
            // 
            // gridUnitSizeNumericUpDown
            // 
            this.gridUnitSizeNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridUnitSizeNumericUpDown.Location = new System.Drawing.Point(161, 66);
            this.gridUnitSizeNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gridUnitSizeNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.gridUnitSizeNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gridUnitSizeNumericUpDown.Name = "gridUnitSizeNumericUpDown";
            this.gridUnitSizeNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.gridUnitSizeNumericUpDown.TabIndex = 36;
            this.gridUnitSizeNumericUpDown.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.gridUnitSizeNumericUpDown.ValueChanged += new System.EventHandler(this.gridUnitSizeNumericUpDown_ValueChanged);
            this.gridUnitSizeNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridUnitSizeNumericUpDown_KeyDown);
            this.gridUnitSizeNumericUpDown.Validated += new System.EventHandler(this.gridUnitSizeNumericUpDown_Validated);
            // 
            // snapToGridCheckBox
            // 
            this.snapToGridCheckBox.AutoSize = true;
            this.snapToGridCheckBox.Checked = true;
            this.snapToGridCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.snapToGridCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snapToGridCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.snapToGridCheckBox.Location = new System.Drawing.Point(11, 35);
            this.snapToGridCheckBox.Name = "snapToGridCheckBox";
            this.snapToGridCheckBox.Size = new System.Drawing.Size(129, 24);
            this.snapToGridCheckBox.TabIndex = 34;
            this.snapToGridCheckBox.Text = "Snap To Grid";
            this.snapToGridCheckBox.UseVisualStyleBackColor = true;
            this.snapToGridCheckBox.CheckedChanged += new System.EventHandler(this.snapToGridCheckBox_CheckedChanged);
            // 
            // paintUndoLabel
            // 
            this.paintUndoLabel.AutoSize = true;
            this.paintUndoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paintUndoLabel.ForeColor = System.Drawing.Color.LightGray;
            this.paintUndoLabel.Location = new System.Drawing.Point(15, 68);
            this.paintUndoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paintUndoLabel.Name = "paintUndoLabel";
            this.paintUndoLabel.Size = new System.Drawing.Size(111, 20);
            this.paintUndoLabel.TabIndex = 35;
            this.paintUndoLabel.Text = "Grid Unit Size:";
            // 
            // regionSettingsGroupBox
            // 
            this.regionSettingsGroupBox.Controls.Add(this.snapToPowersCheckBox);
            this.regionSettingsGroupBox.Controls.Add(this.cornerSnappingCheckBox);
            this.regionSettingsGroupBox.Controls.Add(this.edgeSnappingCheckBox);
            this.regionSettingsGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regionSettingsGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.regionSettingsGroupBox.Location = new System.Drawing.Point(12, 54);
            this.regionSettingsGroupBox.Name = "regionSettingsGroupBox";
            this.regionSettingsGroupBox.Size = new System.Drawing.Size(547, 124);
            this.regionSettingsGroupBox.TabIndex = 41;
            this.regionSettingsGroupBox.TabStop = false;
            this.regionSettingsGroupBox.Text = "Region Snapping";
            // 
            // snapToPowersCheckBox
            // 
            this.snapToPowersCheckBox.AutoSize = true;
            this.snapToPowersCheckBox.Checked = true;
            this.snapToPowersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.snapToPowersCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snapToPowersCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.snapToPowersCheckBox.Location = new System.Drawing.Point(11, 92);
            this.snapToPowersCheckBox.Name = "snapToPowersCheckBox";
            this.snapToPowersCheckBox.Size = new System.Drawing.Size(180, 24);
            this.snapToPowersCheckBox.TabIndex = 39;
            this.snapToPowersCheckBox.Text = "Scale to Powers of 2";
            this.snapToPowersCheckBox.UseVisualStyleBackColor = true;
            this.snapToPowersCheckBox.CheckedChanged += new System.EventHandler(this.snapToPowersCheckBox_CheckedChanged);
            // 
            // cornerSnappingCheckBox
            // 
            this.cornerSnappingCheckBox.AutoSize = true;
            this.cornerSnappingCheckBox.Checked = true;
            this.cornerSnappingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cornerSnappingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cornerSnappingCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.cornerSnappingCheckBox.Location = new System.Drawing.Point(11, 62);
            this.cornerSnappingCheckBox.Name = "cornerSnappingCheckBox";
            this.cornerSnappingCheckBox.Size = new System.Drawing.Size(155, 24);
            this.cornerSnappingCheckBox.TabIndex = 38;
            this.cornerSnappingCheckBox.Text = "Corner Snapping";
            this.cornerSnappingCheckBox.UseVisualStyleBackColor = true;
            this.cornerSnappingCheckBox.CheckedChanged += new System.EventHandler(this.cornerSnappingCheckBox_CheckedChanged);
            // 
            // edgeSnappingCheckBox
            // 
            this.edgeSnappingCheckBox.AutoSize = true;
            this.edgeSnappingCheckBox.Checked = true;
            this.edgeSnappingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.edgeSnappingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edgeSnappingCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.edgeSnappingCheckBox.Location = new System.Drawing.Point(11, 32);
            this.edgeSnappingCheckBox.Name = "edgeSnappingCheckBox";
            this.edgeSnappingCheckBox.Size = new System.Drawing.Size(145, 24);
            this.edgeSnappingCheckBox.TabIndex = 37;
            this.edgeSnappingCheckBox.Text = "Edge Snapping";
            this.edgeSnappingCheckBox.UseVisualStyleBackColor = true;
            this.edgeSnappingCheckBox.CheckedChanged += new System.EventHandler(this.edgeSnappingCheckBox_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(41)))), ((int)(((byte)(58)))));
            this.panel3.Controls.Add(this.label3);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(570, 44);
            this.panel3.TabIndex = 40;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.LightGray;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(570, 44);
            this.label3.TabIndex = 39;
            this.label3.Text = "Snapping";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // generalPanel
            // 
            this.generalPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.generalPanel.Controls.Add(this.devModeCheckBox);
            this.generalPanel.Controls.Add(this.groupBox4);
            this.generalPanel.Controls.Add(this.groupBox2);
            this.generalPanel.Controls.Add(this.groupBox3);
            this.generalPanel.Controls.Add(this.panel4);
            this.generalPanel.Location = new System.Drawing.Point(332, 13);
            this.generalPanel.Name = "generalPanel";
            this.generalPanel.Size = new System.Drawing.Size(570, 549);
            this.generalPanel.TabIndex = 34;
            this.generalPanel.Visible = false;
            // 
            // devModeCheckBox
            // 
            this.devModeCheckBox.AutoSize = true;
            this.devModeCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devModeCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.devModeCheckBox.Location = new System.Drawing.Point(14, 309);
            this.devModeCheckBox.Name = "devModeCheckBox";
            this.devModeCheckBox.Size = new System.Drawing.Size(151, 24);
            this.devModeCheckBox.TabIndex = 46;
            this.devModeCheckBox.Text = "Developer Mode";
            this.devModeCheckBox.UseVisualStyleBackColor = true;
            this.devModeCheckBox.CheckedChanged += new System.EventHandler(this.devModeCheckBox_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.undoStepsNumericUpDown);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox4.Location = new System.Drawing.Point(12, 232);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(547, 67);
            this.groupBox4.TabIndex = 45;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Undo/Redo";
            // 
            // undoStepsNumericUpDown
            // 
            this.undoStepsNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.undoStepsNumericUpDown.Location = new System.Drawing.Point(161, 25);
            this.undoStepsNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.undoStepsNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.undoStepsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.undoStepsNumericUpDown.Name = "undoStepsNumericUpDown";
            this.undoStepsNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.undoStepsNumericUpDown.TabIndex = 44;
            this.undoStepsNumericUpDown.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.undoStepsNumericUpDown.ValueChanged += new System.EventHandler(this.undoStepsNumericUpDown_ValueChanged);
            this.undoStepsNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.undoStepsNumericUpDown_KeyDown);
            this.undoStepsNumericUpDown.Validated += new System.EventHandler(this.undoStepsNumericUpDown_Validated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.LightGray;
            this.label9.Location = new System.Drawing.Point(15, 27);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 20);
            this.label9.TabIndex = 43;
            this.label9.Text = "Max Steps:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.regionMaxNumericUpDown);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.regionMinNumericUpDown);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox2.Location = new System.Drawing.Point(12, 126);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(547, 103);
            this.groupBox2.TabIndex = 42;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Limits";
            // 
            // regionMaxNumericUpDown
            // 
            this.regionMaxNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regionMaxNumericUpDown.Location = new System.Drawing.Point(161, 60);
            this.regionMaxNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.regionMaxNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.regionMaxNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.regionMaxNumericUpDown.Name = "regionMaxNumericUpDown";
            this.regionMaxNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.regionMaxNumericUpDown.TabIndex = 46;
            this.regionMaxNumericUpDown.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.regionMaxNumericUpDown.ValueChanged += new System.EventHandler(this.regionMaxNumericUpDown_ValueChanged);
            this.regionMaxNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.regionMaxNumericUpDown_KeyDown);
            this.regionMaxNumericUpDown.Validated += new System.EventHandler(this.regionMaxNumericUpDown_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.LightGray;
            this.label8.Location = new System.Drawing.Point(15, 62);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(132, 20);
            this.label8.TabIndex = 45;
            this.label8.Text = "Region Max Size:";
            // 
            // regionMinNumericUpDown
            // 
            this.regionMinNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regionMinNumericUpDown.Location = new System.Drawing.Point(161, 29);
            this.regionMinNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.regionMinNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.regionMinNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.regionMinNumericUpDown.Name = "regionMinNumericUpDown";
            this.regionMinNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.regionMinNumericUpDown.TabIndex = 44;
            this.regionMinNumericUpDown.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.regionMinNumericUpDown.ValueChanged += new System.EventHandler(this.regionMinNumericUpDown_ValueChanged);
            this.regionMinNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.regionMinNumericUpDown_KeyDown);
            this.regionMinNumericUpDown.Validated += new System.EventHandler(this.regionMinNumericUpDown_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.LightGray;
            this.label7.Location = new System.Drawing.Point(15, 31);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 20);
            this.label7.TabIndex = 43;
            this.label7.Text = "Region Min Size:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cameraSpeedNumericUpDown);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox3.Location = new System.Drawing.Point(12, 54);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(547, 67);
            this.groupBox3.TabIndex = 41;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Camera";
            // 
            // cameraSpeedNumericUpDown
            // 
            this.cameraSpeedNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cameraSpeedNumericUpDown.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.cameraSpeedNumericUpDown.Location = new System.Drawing.Point(161, 25);
            this.cameraSpeedNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cameraSpeedNumericUpDown.Maximum = new decimal(new int[] {
            1215752192,
            23,
            0,
            0});
            this.cameraSpeedNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.cameraSpeedNumericUpDown.Name = "cameraSpeedNumericUpDown";
            this.cameraSpeedNumericUpDown.Size = new System.Drawing.Size(101, 26);
            this.cameraSpeedNumericUpDown.TabIndex = 44;
            this.cameraSpeedNumericUpDown.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.cameraSpeedNumericUpDown.ValueChanged += new System.EventHandler(this.cameraSpeedNumericUpDown_ValueChanged);
            this.cameraSpeedNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cameraSpeedNumericUpDown_KeyDown);
            this.cameraSpeedNumericUpDown.Validated += new System.EventHandler(this.cameraSpeedNumericUpDown_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.LightGray;
            this.label6.Location = new System.Drawing.Point(15, 27);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 20);
            this.label6.TabIndex = 43;
            this.label6.Text = "Camera Speed:";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(41)))), ((int)(((byte)(58)))));
            this.panel4.Controls.Add(this.label5);
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(570, 44);
            this.panel4.TabIndex = 40;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.LightGray;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(570, 44);
            this.label5.TabIndex = 39;
            this.label5.Text = "General";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PropertiesSubform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(914, 574);
            this.Controls.Add(this.modelPanel);
            this.Controls.Add(this.snapPanel);
            this.Controls.Add(this.generalPanel);
            this.Controls.Add(this.settingTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesSubform";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Properties";
            this.TopMost = true;
            this.goopModelGroupBox.ResumeLayout(false);
            this.goopModelGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.goopAngleNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.floorOffsetNumericUpDown)).EndInit();
            this.modelPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.snapPanel.ResumeLayout(false);
            this.gridSettingsGroupBox.ResumeLayout(false);
            this.gridSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnitSizeNumericUpDown)).EndInit();
            this.regionSettingsGroupBox.ResumeLayout(false);
            this.regionSettingsGroupBox.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.generalPanel.ResumeLayout(false);
            this.generalPanel.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.undoStepsNumericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.regionMaxNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.regionMinNumericUpDown)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cameraSpeedNumericUpDown)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox goopModelGroupBox;
        private System.Windows.Forms.CheckBox ignoreCollisionCheckBox;
        private System.Windows.Forms.TextBox collisionTypesTextbox;
        private System.Windows.Forms.NumericUpDown floorOffsetNumericUpDown;
        private System.Windows.Forms.Label goopOffsetLabel;
        private System.Windows.Forms.NumericUpDown goopAngleNumericUpDown;
        private System.Windows.Forms.Label goopAngleLabel;
        private System.Windows.Forms.TreeView settingTreeView;
        private System.Windows.Forms.Panel modelPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel snapPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox gridSettingsGroupBox;
        private System.Windows.Forms.NumericUpDown gridUnitSizeNumericUpDown;
        private System.Windows.Forms.CheckBox snapToGridCheckBox;
        private System.Windows.Forms.Label paintUndoLabel;
        private System.Windows.Forms.GroupBox regionSettingsGroupBox;
        private System.Windows.Forms.CheckBox cornerSnappingCheckBox;
        private System.Windows.Forms.CheckBox edgeSnappingCheckBox;
        private System.Windows.Forms.Panel generalPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown cameraSpeedNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown regionMaxNumericUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown regionMinNumericUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox devModeCheckBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown undoStepsNumericUpDown;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox snapToPowersCheckBox;
    }
}