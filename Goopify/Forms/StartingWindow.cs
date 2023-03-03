using Goopify.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify
{
    public partial class StartingWindow : Form
    {

        private const bool testEditWithoutMap = false;
        private const bool instantEditor = false;

        public StartingWindow()
        {
            InitializeComponent();

            SetupInfoPathsIfNeeded();

            if(instantEditor)
            {
                //bmp.Save("C:\\Users\\alexh\\Downloads\\pollutiontest.bmp");
                string path = "C:\\Users\\alexh\\Downloads\\casino1.szs_ext\\scene\\map\\map.col";
                // Open and setup the window
                EditorWindow editorWindow = new EditorWindow();
                editorWindow.Show();
                this.Hide();

                editorWindow.Setup(path);
            }
        }

        /// <summary>
        /// Shows the ymp extracting window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extractGoopmapButton_Click(object sender, EventArgs e)
        {
            DumpWindow dumpWindow = new DumpWindow();
            dumpWindow.Show();
            this.Hide();
        }

        /// <summary>
        /// Opens up a file dialogue for a col model and uses it to show the editor window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGoopmapButton_Click(object sender, EventArgs e)
        {
            if(testEditWithoutMap)
            {
                EditorWindow editorWindow = new EditorWindow();
                editorWindow.Show();
                this.Hide();
            } else {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Collision File (*.col)|*.col";
                fileDialog.InitialDirectory = Properties.Settings.Default.loadColDialogueRestore;
                if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
                {
                    // Save the directory for next time
                    Properties.Settings.Default.loadColDialogueRestore = Path.GetDirectoryName(fileDialog.FileName);
                    Properties.Settings.Default.Save();
                    // Open and setup the window
                    EditorWindow editorWindow = new EditorWindow();
                    editorWindow.Show();
                    this.Hide();

                    editorWindow.Setup(fileDialog.FileName);
                }
            }
        }

        private void SetupInfoPathsIfNeeded()
        {
            // Setup SuperBMD
            if (Properties.Settings.Default.superBmdPath == "null" || !File.Exists(Properties.Settings.Default.superBmdPath))
            {
                MessageBox.Show("Setup SuperBMD path for model exporting!", "Initial Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "SuperBMD Path";
                fileDialog.Filter = "SuperBMD (*.exe)|*.exe";
                fileDialog.InitialDirectory = "C:\\";
                if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
                {
                    // Save the directory for next time
                    Properties.Settings.Default.superBmdPath = fileDialog.FileName;
                    Properties.Settings.Default.Save();

                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Properties.Settings.Default.superBmdPath);
                    string versionString = versionInfo.FileVersion;
                }
            }

            // Setup goop resources
            if(!GoopResources.AreResourcesGotten())
            {
                // Get the scene folder to extract resources
                CommonOpenFileDialog folderDialogue = new CommonOpenFileDialog();
                folderDialogue.IsFolderPicker = true;
                folderDialogue.Title = "Select the extracted iso \"scene\" folder";
                if (folderDialogue.ShowDialog() == CommonFileDialogResult.Ok) // If the Ok button is hit
                {
                    GoopResources.GetGoopResources(folderDialogue.FileName);
                }
            }
        }
    }
}
