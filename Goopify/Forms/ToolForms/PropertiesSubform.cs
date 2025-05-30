using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify
{
    public partial class PropertiesSubform : Form
    {
        private Panel[] panels;

        public PropertiesSubform()
        {
            InitializeComponent();
            // Hide all panels except start one
            panels = new Panel[] { generalPanel, snapPanel, modelPanel };
            ShowPanel(0);

            // Set values

            //General
            cameraSpeedNumericUpDown.Value = (decimal)Properties.Settings.Default.cameraSpeed;
            regionMinNumericUpDown.Value = Properties.Settings.Default.regionMinSize;
            regionMaxNumericUpDown.Value = Properties.Settings.Default.regionMaxSize;
            undoStepsNumericUpDown.Value = Properties.Settings.Default.undoSteps;
            devModeCheckBox.Checked = Properties.Settings.Default.devMode;
            //Snapping
            edgeSnappingCheckBox.Checked = Properties.Settings.Default.snapToRegionEdge;
            cornerSnappingCheckBox.Checked = Properties.Settings.Default.snapToRegionCorner;
            snapToPowersCheckBox.Checked = Properties.Settings.Default.snapToPowers;
            snapToGridCheckBox.Checked = Properties.Settings.Default.snapToGrid;
            gridUnitSizeNumericUpDown.Enabled = snapToGridCheckBox.Checked;
            gridUnitSizeNumericUpDown.Value = Properties.Settings.Default.snapInterval;
            //Model
            collisionTypesTextbox.Text = string.Join(", ", Properties.Settings.Default.colTypesToRemove);
            ignoreCollisionCheckBox.Checked = Properties.Settings.Default.ignoreColTypes;
            goopAngleNumericUpDown.Value = (decimal)Properties.Settings.Default.goopAngleTolerance;
            floorOffsetNumericUpDown.Value = (decimal)Properties.Settings.Default.regionFloorOffset;
        }

        private void settingTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowPanel(e.Node.Index);
        }

        private void ShowPanel(int index)
        {
            for(int i = 0; i < panels.Length; i++)
            {
                panels[i].Visible = i == index;
            }
        }

        #region General Panel

        private void cameraSpeedNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.cameraSpeed = (float)cameraSpeedNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void cameraSpeedNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                cameraSpeedNumericUpDown.Validate();
            }
        }

        private void cameraSpeedNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.cameraSpeed = (float)cameraSpeedNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void regionMinNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.regionMinSize = (int)regionMinNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void regionMinNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                regionMinNumericUpDown.Validate();
            }
        }

        private void regionMinNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.regionMinSize = (int)regionMinNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void regionMaxNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.regionMaxSize = (int)regionMaxNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void regionMaxNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                regionMaxNumericUpDown.Validate();
            }
        }

        private void regionMaxNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.regionMaxSize = (int)regionMaxNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void undoStepsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.undoSteps = (int)undoStepsNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void undoStepsNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                undoStepsNumericUpDown.Validate();
            }
        }

        private void undoStepsNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.undoSteps = (int)undoStepsNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void devModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.devMode = devModeCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Snapping Panel

        private void edgeSnappingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.snapToRegionEdge = edgeSnappingCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void cornerSnappingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.snapToRegionCorner = cornerSnappingCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void snapToPowersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.snapToPowers = snapToPowersCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void snapToGridCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.snapToGrid = snapToGridCheckBox.Checked;
            gridUnitSizeNumericUpDown.Enabled = snapToGridCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void gridUnitSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.snapInterval = Convert.ToInt32(gridUnitSizeNumericUpDown.Value);
            Properties.Settings.Default.Save();
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
            Properties.Settings.Default.snapInterval = Convert.ToInt32(gridUnitSizeNumericUpDown.Value);
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Model Panel

        private void collisionTypesTextbox_Validated(object sender, EventArgs e)
        {
            string trimmedString = collisionTypesTextbox.Text.Trim();
            Properties.Settings.Default.colTypesToRemove = Array.ConvertAll(trimmedString.Split(','), int.Parse);
            Properties.Settings.Default.Save();
        }

        private void ignoreCollisionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ignoreColTypes = ignoreCollisionCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void goopAngleNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.goopAngleTolerance = (float)goopAngleNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void goopAngleNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                goopAngleNumericUpDown.Validate();
            }
        }

        private void goopAngleNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.goopAngleTolerance = (float)goopAngleNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void floorOffsetNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.regionFloorOffset = (float)floorOffsetNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        private void floorOffsetNumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                floorOffsetNumericUpDown.Validate();
            }
        }

        private void floorOffsetNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.regionFloorOffset = (float)floorOffsetNumericUpDown.Value;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}
