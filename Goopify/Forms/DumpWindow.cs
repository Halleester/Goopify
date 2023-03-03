using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Goopify.Forms
{
    public partial class DumpWindow : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Program.startingForm.Close();
            base.OnFormClosing(e);
        }

        private string ympPath;
        private string savePath;

        public DumpWindow()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Program.startingForm.Location;
        }

        private void ympLocButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Pollution Map File (*.ymp)|*.ymp";
            fileDialog.InitialDirectory = Properties.Settings.Default.dumpYmpDialogRestore;
            if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.dumpYmpDialogRestore = fileDialog.FileName;
                Properties.Settings.Default.Save();
                //Get the path of specified file
                ympPath = fileDialog.FileName;
                ympLocFileBox.Text = ympPath;
            }
        }

        private void saveLocButton_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog folderDialogue = new CommonOpenFileDialog();
            folderDialogue.IsFolderPicker = true;
            folderDialogue.InitialDirectory = Properties.Settings.Default.dumpSaveDialogRestore;
            if (folderDialogue.ShowDialog() == CommonFileDialogResult.Ok) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.dumpSaveDialogRestore = folderDialogue.FileName;
                Properties.Settings.Default.Save();
                //Get the path of specified file
                savePath = folderDialogue.FileName;
                saveLocFileBox.Text = savePath;
            }
            this.TopMost = true;
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            // Error checking for ymp path
            if (ympPath == "") {
                MessageBox.Show(this, "Enter a ymp path first!");
                return;
            }
            try {
                FileInfo ympFileInfo = new FileInfo(ympPath);
                if(ympPath == "" || !ympFileInfo.Exists || ympFileInfo.Extension != ".ymp") {
                    MessageBox.Show(this, "Path to ymp file is invalid!");
                    return;
                }
            } catch {
                MessageBox.Show(this, "Path to ymp file is invalid!");
                return;
            }
            // Error checking for save path
            if(savePath == "") {
                MessageBox.Show(this, "Enter a save path first!");
                return;
            }
            try {
                FileInfo saveFileInfo = new FileInfo(savePath);
                if (saveFileInfo.Exists || saveFileInfo.Extension != "") {
                    MessageBox.Show(this, "Save path is invalid!");
                    return;
                }
            } catch {
                MessageBox.Show(this, "Save path is invalid!");
                return;
            }


            // Read the contents of the file into a stream
            Stream fileStream = File.Open(ympPath, FileMode.Open);

            string ympName = Path.GetFileNameWithoutExtension(ympPath);

            PollutionMap createdPollutionMap = new PollutionMap(fileStream); // Create the pollution map
            fileStream.Close();
            if (heightmapCheckbox.Checked) { createdPollutionMap.ExportHeightmaps(savePath, ympName); }
            if(infoCheckbox.Checked) { createdPollutionMap.ExportInfo(savePath, ympName); }

            //Opens an explorer window at that location
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = savePath,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void ympLocFileBox_TextChanged(object sender, EventArgs e)
        {
            ympPath = ympLocFileBox.Text;
        }

        private void saveLocFileBox_TextChanged(object sender, EventArgs e)
        {
            savePath = saveLocFileBox.Text;
        }
    }
}
