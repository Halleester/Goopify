
namespace Goopify.Forms.ToolForms
{
    partial class SnapSettingsSubform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapSettingsSubform));
            this.snapToGridCheckBox = new System.Windows.Forms.CheckBox();
            this.paintUndoLabel = new System.Windows.Forms.Label();
            this.edgeSnappingCheckBox = new System.Windows.Forms.CheckBox();
            this.regionSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.cornerSnappingCheckBox = new System.Windows.Forms.CheckBox();
            this.gridSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.gridUnitSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.confirmButton = new System.Windows.Forms.Button();
            this.regionSettingsGroupBox.SuspendLayout();
            this.gridSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnitSizeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // snapToGridCheckBox
            // 
            this.snapToGridCheckBox.AutoSize = true;
            this.snapToGridCheckBox.Checked = true;
            this.snapToGridCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.snapToGridCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snapToGridCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.snapToGridCheckBox.Location = new System.Drawing.Point(125, 39);
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
            this.paintUndoLabel.Location = new System.Drawing.Point(60, 79);
            this.paintUndoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.paintUndoLabel.Name = "paintUndoLabel";
            this.paintUndoLabel.Size = new System.Drawing.Size(111, 20);
            this.paintUndoLabel.TabIndex = 35;
            this.paintUndoLabel.Text = "Grid Unit Size:";
            // 
            // edgeSnappingCheckBox
            // 
            this.edgeSnappingCheckBox.AutoSize = true;
            this.edgeSnappingCheckBox.Checked = true;
            this.edgeSnappingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.edgeSnappingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edgeSnappingCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.edgeSnappingCheckBox.Location = new System.Drawing.Point(109, 42);
            this.edgeSnappingCheckBox.Name = "edgeSnappingCheckBox";
            this.edgeSnappingCheckBox.Size = new System.Drawing.Size(145, 24);
            this.edgeSnappingCheckBox.TabIndex = 37;
            this.edgeSnappingCheckBox.Text = "Edge Snapping";
            this.edgeSnappingCheckBox.UseVisualStyleBackColor = true;
            this.edgeSnappingCheckBox.CheckedChanged += new System.EventHandler(this.edgeSnappingCheckBox_CheckedChanged);
            // 
            // regionSettingsGroupBox
            // 
            this.regionSettingsGroupBox.Controls.Add(this.cornerSnappingCheckBox);
            this.regionSettingsGroupBox.Controls.Add(this.edgeSnappingCheckBox);
            this.regionSettingsGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.regionSettingsGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.regionSettingsGroupBox.Location = new System.Drawing.Point(34, 26);
            this.regionSettingsGroupBox.Name = "regionSettingsGroupBox";
            this.regionSettingsGroupBox.Size = new System.Drawing.Size(390, 117);
            this.regionSettingsGroupBox.TabIndex = 38;
            this.regionSettingsGroupBox.TabStop = false;
            this.regionSettingsGroupBox.Text = "Region Snapping";
            // 
            // cornerSnappingCheckBox
            // 
            this.cornerSnappingCheckBox.AutoSize = true;
            this.cornerSnappingCheckBox.Checked = true;
            this.cornerSnappingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cornerSnappingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cornerSnappingCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.cornerSnappingCheckBox.Location = new System.Drawing.Point(109, 72);
            this.cornerSnappingCheckBox.Name = "cornerSnappingCheckBox";
            this.cornerSnappingCheckBox.Size = new System.Drawing.Size(155, 24);
            this.cornerSnappingCheckBox.TabIndex = 38;
            this.cornerSnappingCheckBox.Text = "Corner Snapping";
            this.cornerSnappingCheckBox.UseVisualStyleBackColor = true;
            this.cornerSnappingCheckBox.CheckedChanged += new System.EventHandler(this.cornerSnappingCheckBox_CheckedChanged);
            // 
            // gridSettingsGroupBox
            // 
            this.gridSettingsGroupBox.Controls.Add(this.gridUnitSizeNumericUpDown);
            this.gridSettingsGroupBox.Controls.Add(this.snapToGridCheckBox);
            this.gridSettingsGroupBox.Controls.Add(this.paintUndoLabel);
            this.gridSettingsGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridSettingsGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.gridSettingsGroupBox.Location = new System.Drawing.Point(34, 162);
            this.gridSettingsGroupBox.Name = "gridSettingsGroupBox";
            this.gridSettingsGroupBox.Size = new System.Drawing.Size(390, 117);
            this.gridSettingsGroupBox.TabIndex = 39;
            this.gridSettingsGroupBox.TabStop = false;
            this.gridSettingsGroupBox.Text = "Grid Snapping";
            // 
            // gridUnitSizeNumericUpDown
            // 
            this.gridUnitSizeNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridUnitSizeNumericUpDown.Location = new System.Drawing.Point(206, 77);
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
            // confirmButton
            // 
            this.confirmButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.confirmButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(143)))), ((int)(((byte)(179)))));
            this.confirmButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(166)))), ((int)(((byte)(195)))));
            this.confirmButton.FlatAppearance.BorderSize = 2;
            this.confirmButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.confirmButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.confirmButton.ForeColor = System.Drawing.Color.Black;
            this.confirmButton.Location = new System.Drawing.Point(171, 296);
            this.confirmButton.Margin = new System.Windows.Forms.Padding(0);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(92, 38);
            this.confirmButton.TabIndex = 40;
            this.confirmButton.Text = "Ok";
            this.confirmButton.UseVisualStyleBackColor = false;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // SnapSettingsSubform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(458, 350);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.gridSettingsGroupBox);
            this.Controls.Add(this.regionSettingsGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SnapSettingsSubform";
            this.Text = "SnapSettingsSubform";
            this.regionSettingsGroupBox.ResumeLayout(false);
            this.regionSettingsGroupBox.PerformLayout();
            this.gridSettingsGroupBox.ResumeLayout(false);
            this.gridSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnitSizeNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox snapToGridCheckBox;
        private System.Windows.Forms.Label paintUndoLabel;
        private System.Windows.Forms.CheckBox edgeSnappingCheckBox;
        private System.Windows.Forms.GroupBox regionSettingsGroupBox;
        private System.Windows.Forms.CheckBox cornerSnappingCheckBox;
        private System.Windows.Forms.GroupBox gridSettingsGroupBox;
        private System.Windows.Forms.NumericUpDown gridUnitSizeNumericUpDown;
        private System.Windows.Forms.Button confirmButton;
    }
}