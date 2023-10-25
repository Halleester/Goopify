using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify.Forms.ToolForms
{
    public partial class SnapSettingsSubform : Form
    {
        private EditorWindow editor;
        public SnapSettingsSubform(EditorWindow gottenEditor)
        {
            InitializeComponent();
            editor = gottenEditor;

            UpdateSettingsVisuals();
        }

        public void UpdateSettingsVisuals()
        {
            edgeSnappingCheckBox.Checked = editor.snapSettings.snapToRegionEdge;
            cornerSnappingCheckBox.Checked = editor.snapSettings.snapToRegionCorner;

            snapToGridCheckBox.Checked = editor.snapSettings.snapToGrid;
            gridUnitSizeNumericUpDown.Enabled = snapToGridCheckBox.Checked;
            gridUnitSizeNumericUpDown.Value = editor.snapSettings.snapInterval;
        }

        // Grid interval size events
        private void gridUnitSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            editor.snapSettings.snapInterval = Convert.ToInt32(gridUnitSizeNumericUpDown.Value);
            editor.snapSettings.SettingsChanged();
        }

        private void gridUnitSizeNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                gridUnitSizeNumericUpDown.Validate();
            }
        }

        private void gridUnitSizeNumericUpDown_Validated(object sender, EventArgs e)
        {
            editor.snapSettings.snapInterval = Convert.ToInt32(gridUnitSizeNumericUpDown.Value);
            editor.snapSettings.SettingsChanged();
        }

        private void edgeSnappingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            editor.snapSettings.snapToRegionEdge = edgeSnappingCheckBox.Checked;
            editor.snapSettings.SettingsChanged();
        }

        private void cornerSnappingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            editor.snapSettings.snapToRegionCorner = cornerSnappingCheckBox.Checked;
            editor.snapSettings.SettingsChanged();
        }

        private void snapToGridCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            editor.snapSettings.snapToGrid = snapToGridCheckBox.Checked;
            gridUnitSizeNumericUpDown.Enabled = snapToGridCheckBox.Checked;
            editor.snapSettings.SettingsChanged();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
