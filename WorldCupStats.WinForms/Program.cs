using DataLayer.Services;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using WorldCupStats.WinForms.Forms;

namespace WorldCupStats.WinForms
{
    internal static class Program
    {
        public static FileService FileService { get; private set; }
        public static ApiService ApiService { get; private set; }
        public static bool IsMen { get; set; }
        public static string Language { get; set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            FileService = new FileService(basePath);
            ApiService = new ApiService();

            var settings = FileService.LoadSettings();

            if (settings == null)
            {
                // Première fois → on demande les settings
                using var formSettings = new FormSettings(isFirstTime: true);
                if (formSettings.ShowDialog() != DialogResult.OK)
                    return;
            }
            else
            {
                Language = settings.Value.language;
                IsMen = settings.Value.isMen;
            }

            Application.Run(new FormMain());
        }
    }
}