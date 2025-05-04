using Goopify.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify
{
    public partial class StartingWindow : Form
    {

        private const bool testEditWithoutMap = false;
        private const bool instantEditor = false;

        public StartingWindow(string instantLoad = "")
        {
            InitializeComponent();

            //ResetSettings();

            SetupInfoPathsIfNeeded();

            if(instantLoad != "")
            {
                // Open and setup the window
                EditorWindow editorWindow = new EditorWindow();
                editorWindow.Show();
                this.Hide();

                editorWindow.LoadGoopMap(instantLoad);
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
                NewGoopMap();
            }
        }

        public void NewGoopMap()
        {
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

                editorWindow.NewGoopMap(fileDialog.FileName);
            }
        }

        private void SetupInfoPathsIfNeeded()
        {
            // Setup SuperBMD
            string defaultSuperBmdPath = Directory.GetCurrentDirectory() + @"\SuperBMD_2.4.7.1\SuperBMD.exe";
            if(!File.Exists(defaultSuperBmdPath))
            {
                if (Properties.Settings.Default.superBmdPath == "null" || !File.Exists(Properties.Settings.Default.superBmdPath))
                {
                    // Try to get a local version of SuperBMD from the build first
                    string superBmdPath = Directory.GetCurrentDirectory() + Properties.Settings.Default.superBmdPath;
                    if(File.Exists(Properties.Settings.Default.superBmdPath))
                    {
                        Properties.Settings.Default.superBmdPath = superBmdPath;
                        Properties.Settings.Default.Save();
                    } else
                    {
                        MessageBox.Show("SuperBMD missing from the build! Select a SuperBMD path for model exporting!", "Initial Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                }
            } else
            {
                Properties.Settings.Default.superBmdPath = defaultSuperBmdPath;
                Properties.Settings.Default.Save();
            }

            // Setup goop resources
            if (!GoopResources.AreResourcesGotten())
            {
                // Warning that there's no defined goop files
                DialogResult dialogResult = MessageBox.Show("No \"GoopResources\" folder has been found. " + 
                    "Would you like to download it?", "Goopify Setup", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (WebClient wc = new WebClient())
                    {
                        string zipUrl = "https://drive.google.com/uc?export=download&id=1UhCAdBIxxPWgdwRRRPq1VHCL0VCqBS7K";
                        string zipSaveLoc = Directory.GetCurrentDirectory() + @"/GoopResources.zip";
                        wc.DownloadFileAsync(new Uri(zipUrl), zipSaveLoc);
                        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    }
                }

                /*MessageBox.Show("Please extract your base SMS iso and select your \"scene\" folder.", "Goopify Setup");
                // Get the scene folder to extract resources
                CommonOpenFileDialog folderDialogue = new CommonOpenFileDialog();
                folderDialogue.IsFolderPicker = true;
                folderDialogue.Title = "Select the extracted iso \"scene\" folder";
                if (folderDialogue.ShowDialog() == CommonFileDialogResult.Ok) // If the Ok button is hit
                {
                    GoopResources.GetGoopResources(folderDialogue.FileName);
                }*/
            }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(e.Error == null) {
                ZipFile.ExtractToDirectory(Directory.GetCurrentDirectory() + @"/GoopResources.zip", Directory.GetCurrentDirectory());
                MessageBox.Show("GoopResources successfully downloaded!", "Goopify Setup");
            } else {
                MessageBox.Show("Error while downloading GoopResources: " + e.Error.ToString(), "Goopify Setup");
            }
            
        }

        private void ResetSettings()
        {
            Properties.Settings.Default.Reset();
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        private void editGoopmapButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "GoopMap File (*.goo)|*.goo";
            fileDialog.InitialDirectory = Properties.Settings.Default.loadGoopDialogueRestore;
            if (fileDialog.ShowDialog() == DialogResult.OK) // If the Ok button is hit
            {
                // Save the directory for next time
                Properties.Settings.Default.loadGoopDialogueRestore = Path.GetDirectoryName(fileDialog.FileName);
                Properties.Settings.Default.Save();
                // Open and setup the window
                EditorWindow editorWindow = new EditorWindow();
                editorWindow.Show();
                this.Hide();

                editorWindow.LoadGoopMap(fileDialog.FileName);
            }
        }
    }
}
