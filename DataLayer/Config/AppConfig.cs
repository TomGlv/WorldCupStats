using System;
using System.Collections.Generic;
using System.Text;
namespace DataLayer.Config
{
    public static class AppConfig
    {
        // true = API en ligne, false = fichiers JSON locaux
        public static bool UseApi { get; set; } = true;

        public static string BaseApiUrl { get; set; }
            = "https://worldcup-vua.nullbit.hr";

        public static string JsonFilesPath { get; set; }
            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData");
    }
}