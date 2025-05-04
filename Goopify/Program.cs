using Goopify.Forms.ToolForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goopify
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static StartingWindow startingForm;
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check if we're trying to open a file with this program
            string startupLoad = "";
            if (args.Length > 0 && File.Exists(args[0]) && Path.GetExtension(args[0]) == ".goo") {
                startupLoad = args[0];
            }

            startingForm = new StartingWindow(startupLoad);
            Application.Run(startingForm);
        }
    }
}
