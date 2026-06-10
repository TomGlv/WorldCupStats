using DataLayer.Services;
using WorldCupStats.WPF.Views;
using System.Windows;

namespace WorldCupStats.WPF
{
    public partial class App : Application
    {
        public static FileService FileService { get; private set; } = null!;
        public static ApiService ApiService { get; private set; } = null!;
        public static bool IsMen { get; set; }
        public static string Language { get; set; } = "en";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            FileService = new FileService(basePath);
            ApiService = new ApiService();

            var settings = FileService.LoadSettings();
            if (settings != null)
            {
                Language = settings.Value.language;
                IsMen = settings.Value.isMen;
            }

            var windowSettings = new WindowSettings(isFirstTime: true);
            windowSettings.Show();
        }
    }
}