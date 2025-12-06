#nullable disable
using System;
using System.Windows.Forms;

namespace AWE.DesktopApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            // Launch the LoginForm first to handle authentication
            Application.Run(new LoginForm());
        }
    }
}
