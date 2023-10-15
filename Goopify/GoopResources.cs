using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Goopify
{
    static class GoopResources
    {
        public const string resourcePath = "GoopResources";
        public const string matDumpFolder = "BMDDump";

        // Goop needs a model, texture bti, particles, bmp, and btk animation

        // Add custom goop as an example for adding your own goop types
        public enum PredefinedGoopColor { Brown, Black, Pink, Electric, Fire }

        public static readonly string[] goopColorLevels = { "bianco0", "ricco0", "mare0", "sirena0", "monte0"};
        public static readonly string[] goopFolderNames = { "BrownGoop", "BlackGoop", "PinkGoop", "ElectricGoop", "FireGoop" };
        public const string pollutionFolderPath = "\\scene\\map\\pollution";
        private const string bmpFileName = "bmpExample.bmp";
        private const string btkFileName = "textureAnim.btk";
        private const string matJsonName = "goop_materials.json";
        private const string texHeadJsonName = "goop_texheaders.json";

        // bianco0 for brown, mare0 for pink, monte0 for fire, ricco0 for black, sirena0 for electric

        // Particles (always the same name, the files are just different)
        private const string flowerParticle = "ms_m_ashios.jpa"; // Ground splat effect?
        private const string secondParticle = "ms_m_spinos.jpa"; // Goop spawned effect (Gooper blooper vomit move is example, cleaning piantas?)
        private const string thirdParticle = "ms_m_tokeos.jpa"; // Slower bigger version of splatter (rise?)

        private const string fireParticleOne = "ms_newfire_a.jpa";
        private const string fireParticleTwo = "ms_newfire_b.jpa";

        private const string electricParticle = "ms_thunder_s.jpa";

        // Textures (always the same, file is just different, not in test 11, seems you can specify textures like pollute00.bti according to pinnaBeach0)
        private const string goopTextureName = "h_ma_rak.bti";

        private const string goopModelName = "pollution00.bmd";

        public static string[] GetParticleStrings(PredefinedGoopColor goopColor)
        {
            List<string> particleStrings = new List<string>();
            particleStrings.Add(flowerParticle);
            particleStrings.Add(secondParticle);
            particleStrings.Add(thirdParticle);

            switch (goopColor)
            {
                case PredefinedGoopColor.Electric:
                    particleStrings.Add(electricParticle);
                    break;
                case PredefinedGoopColor.Fire:
                    particleStrings.Add(fireParticleOne);
                    particleStrings.Add(fireParticleTwo);
                    break;
            }

            return particleStrings.ToArray();
        }

        public static string GetBtkResourceString(PredefinedGoopColor goopColor)
        {
            switch(goopColor)
            {
                case PredefinedGoopColor.Brown:
                    return "Resources\\btks\\brownGoop.btk";
                case PredefinedGoopColor.Black:
                    return "Resources\\btks\\blackGoop.btk";
                case PredefinedGoopColor.Electric:
                    return "Resources\\btks\\electricGoop.btk";
                case PredefinedGoopColor.Fire:
                    return "Resources\\btks\\fireGoop.btk";
                case PredefinedGoopColor.Pink:
                    return "Resources\\btks\\pinkGoop.btk";
            }
            return "";
        }

        public static string[] GetBmdJsonStrings(PredefinedGoopColor goopColor)
        {
            string[] jsonStrings = new string[2];
            string prefexString = "";
            switch(goopColor)
            {
                case PredefinedGoopColor.Brown:
                    prefexString = "brownGoop";
                    break;
                case PredefinedGoopColor.Black:
                    prefexString = "blackGoop";
                    break;
                case PredefinedGoopColor.Fire:
                    prefexString = "fireGoop";
                    break;
                case PredefinedGoopColor.Electric:
                    prefexString = "electricGoop";
                    break;
                case PredefinedGoopColor.Pink:
                    prefexString = "pinkGoop";
                    break;
            }
            jsonStrings[0] = "Resources\\modelJsons\\" + prefexString + "_materials.json";
            jsonStrings[1] = "Resources\\modelJsons\\" + prefexString + "_texheaders.json";
            return jsonStrings;
        }

        public static void GetGoopResources(string scenePath)
        {
            List<string> importErrors = new List<string>();

            if (!Directory.Exists(resourcePath)) {
                Directory.CreateDirectory(resourcePath);
            }

            string szsExtractPath = "";
            for (int i = 0; i < goopColorLevels.Length; i++)
            {
                if(szsExtractPath != "") {
                    Directory.Delete(szsExtractPath, true);
                    szsExtractPath = "";
                }

                string levelPath = scenePath + "\\" + goopColorLevels[i];
                // Check if the level is already extracted, otherwise check for szs and unzip it
                if(!Directory.Exists(levelPath))
                {
                    // Check if szs exists
                    string szsPath = levelPath + ".szs";
                    if (!File.Exists(szsPath)) {
                        importErrors.Add(goopColorLevels[i] + ": No scene folder or SZS found, import skipped");
                        continue;
                    }
                    // Extract the szs
                    szsExtractPath = ExtractSzs(levelPath);
                }

                string pollutionPath = levelPath + pollutionFolderPath;
                string[] particleStrings = GetParticleStrings((PredefinedGoopColor)i);

                if(!Directory.Exists(pollutionPath))
                {
                    importErrors.Add(goopColorLevels[i] + ": Scene folder not found, import skipped");
                    continue;
                }

                string goopResourcePath = resourcePath + "\\" + goopFolderNames[i];
                if (!Directory.Exists(goopResourcePath))
                {
                    Directory.CreateDirectory(goopResourcePath);
                }

                // Copy the particles
                foreach (string particleString in particleStrings)
                {
                    string particlePath = pollutionPath + "\\" + particleString;
                    string savePath = goopResourcePath + "\\" + particleString;
                    if (File.Exists(particlePath))
                    {
                        File.Copy(particlePath, savePath);
                    } else {
                        importErrors.Add(goopColorLevels[i] + ": Particle Effect \"" + particleString + "\" not found");
                    }
                }
                // Copy the bianco0 bmp to use for formatting
                if(i == 0) {
                    string bmpPath = pollutionPath + "\\pollution00.bmp";
                    string savePath = resourcePath + "\\" + bmpFileName;
                    if (File.Exists(bmpPath))
                    {
                        File.Copy(bmpPath, savePath);
                    } else {
                        importErrors.Add(goopColorLevels[i] + ": pollution00.bmp not found");
                    }
                }
                // Copy the bti
                string btiPath = pollutionPath + "\\" + goopTextureName;
                if (File.Exists(btiPath)) {
                    File.Copy(btiPath, goopResourcePath + "\\" + goopTextureName);
                } else {
                    importErrors.Add(goopColorLevels[i] + ": Bti not found");
                }
                // Copy the model to export the textures
                string bmdPath = pollutionPath + "\\" + goopModelName;
                if (File.Exists(bmdPath)) {
                    string superBMDSavePath = goopResourcePath + "\\" + matDumpFolder;

                    if (!Directory.Exists(superBMDSavePath)) {
                        Directory.CreateDirectory(superBMDSavePath);
                    }

                    string superBmdExportPath = superBMDSavePath + "\\" + goopColorLevels[i] + "Pollution";

                    Process p = new Process();
                    p.StartInfo.FileName = Properties.Settings.Default.superBmdPath;
                    p.StartInfo.Arguments = "\"" + bmdPath + "\"" + " " + "\"" + superBmdExportPath + "\"";
                    p.Start();
                    // Don't continue until superbmd has exported the model info
                    p.WaitForExit();

                    // Copy over the modified json files from the resources folder
                    string[] jsonResourceStrings = GetBmdJsonStrings((PredefinedGoopColor)i);
                    File.Copy(jsonResourceStrings[0], superBMDSavePath + "\\" + matJsonName);
                    File.Copy(jsonResourceStrings[1], superBMDSavePath + "\\" + texHeadJsonName);
                } else {
                    importErrors.Add(goopColorLevels[i] + ": pollution00.bmd not found, not extracting model data");
                }
                // Copy the btk animations from the resources folder to the correct goop folder
                string btkResourcePath = GetBtkResourceString((PredefinedGoopColor)i);
                if(File.Exists(btkResourcePath))
                {
                    File.Copy(btkResourcePath, goopResourcePath + "\\" + btkFileName);
                } else {
                    importErrors.Add(goopColorLevels[i] + ": matching btk file not found in the Goopify resource folder");
                }
            }
            if(szsExtractPath != "") {
                Directory.Delete(szsExtractPath, true);
            }
            // Show if the import was fully sucessful or not
            if(importErrors.Count > 0) {
                string errorMessage = "";
                for(int i = 0; i < importErrors.Count; i++)
                {
                    if(i > 0) { errorMessage += "\n"; }
                    errorMessage += importErrors[i];
                }
                MessageBox.Show("Errors Found:\n" + errorMessage, "Import Finished", MessageBoxButton.OK);
            } else {
                MessageBox.Show("Assets imported successfully!", "Import Finished", MessageBoxButton.OK);
            }
            
        }

        // Only need bti and goop model for the pollution to not crash the game
        public static bool IsValidGoopFolder(string folderName)
        {
            string goopFolderPath = resourcePath + "\\" + folderName;

            string btiPath = goopFolderPath + "\\" + goopTextureName;

            return File.Exists(btiPath);
        }

        public static bool HasPredefinedResources(PredefinedGoopColor goopColor)
        {
            int missingResources = 0;

            // Check for all particles
            string[] particleStrings = GetParticleStrings(goopColor);
            foreach (string particleString in particleStrings)
            {
                string particlePath = resourcePath + "\\" + particleString;
                if (!File.Exists(particlePath)) {
                    missingResources++;
                }
            }
            // Check for bti
            if(!File.Exists(resourcePath + "\\" + goopTextureName)) { missingResources++; }
            // Check for bmd resources (Just checks if folder exists)
            if(!File.Exists(resourcePath + "\\" + matDumpFolder)) { missingResources++; }

            return missingResources == 0;
        }

        public static bool AreResourcesGotten()
        {
            return Directory.Exists(resourcePath);
        }

        public static void CopyResourcesToPath(string pollutionPath, string goopTypeFolderName)
        {
            if(!IsValidGoopFolder(goopTypeFolderName)) { return; }

            // Copies all files to the pollutionPath
            DirectoryInfo d = new DirectoryInfo(resourcePath + "\\" + goopTypeFolderName);
            foreach(FileInfo file in d.GetFiles())
            {
                if(file.Extension == "jpa" || file.Extension == "bti")
                    File.Copy(file.DirectoryName, pollutionPath + file.Name);
            }
        }

        public static string GetBtkPath(string goopTypeFolderName)
        {
            string btkPath = resourcePath + "\\" + goopTypeFolderName + "\\" + btkFileName;
            return File.Exists(btkPath) ? btkPath : "";
        }

        public static string[] GetParticlePaths(string goopTypeFolderName)
        {
            string goopFolderPath = resourcePath + "\\" + goopTypeFolderName;
            string[] particlePaths = Directory.GetFiles(goopFolderPath, "*.jpa");
            return particlePaths;
        }

        public static string GetBtiPath(string goopTypeFolderName)
        {
            string btiPath = resourcePath + "\\" + goopTypeFolderName + "\\" + goopTextureName;
            return File.Exists(btiPath) ? btiPath : "";
        }

        public static string GetMatPath(string goopTypeFolderName) {
            string matPath = resourcePath + "\\" + goopTypeFolderName + "\\" + matDumpFolder + "\\" + matJsonName;
            return File.Exists(matPath) ? matPath : "";
        }

        public static string GetTexHeaderPath(string goopTypeFolderName) {
            string texHeaderPath = resourcePath + "\\" + goopTypeFolderName + "\\" + matDumpFolder + "\\" + texHeadJsonName;
            return File.Exists(texHeaderPath) ? texHeaderPath : "";
        }

        public static string GetGlobalResourcesPath(string goopTypeFolderName)
        {
            string resourcesPath = Directory.GetCurrentDirectory() + "\\" + resourcePath + "\\" + goopTypeFolderName + "\\" + matDumpFolder;
            return Directory.Exists(resourcesPath) ? resourcesPath : "";
        }

        public static string GetExampleBmpPath()
        {
            string path = resourcePath + "\\" + bmpFileName;
            return File.Exists(path) ? path : "";
        }

        public static bool CopyPollutionBtk(string path, string goopType)
        {
            string goopPath = GetBtkPath(goopType);
            if(goopPath == "") {
                MessageBox.Show("Btk path is invalid!", "Btk Error");
                return false;
            }
            File.Copy(goopPath, path);
            return true;
        }

        public static string ExtractSzs(string levelPath)
        {
            Process p = new Process();
            p.StartInfo.FileName = Properties.Settings.Default.rarcPath;
            p.StartInfo.Arguments = "\"" + levelPath + ".szs\" \"" + levelPath + "\"";
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            /*using (StreamReader reader = p.StandardOutput) {
                string result = reader.ReadToEnd();
            }*/
            p.WaitForExit();
            return levelPath;
        }
    }
}
