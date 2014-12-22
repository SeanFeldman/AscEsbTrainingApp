using System;
using System.Windows.Forms;

namespace AscEsbTrainingSubUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainform = new SubscriberForm();

            Application.Run(mainform);
        }
    }
}
