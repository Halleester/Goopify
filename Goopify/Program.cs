using Goopify.Forms.ToolForms;
using System;
using System.Collections.Generic;
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
        public static Form startingForm;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Intened for release
            startingForm = new StartingWindow();
            Application.Run(startingForm);

            // For testing OpenTK
            //OpenTKTesting test = new OpenTKTesting();
            //Application.Run(test);

            // For testing editor
            //EditorWindow editorWindow = new EditorWindow();
            //Application.Run(editorWindow);
        }
    }
}
