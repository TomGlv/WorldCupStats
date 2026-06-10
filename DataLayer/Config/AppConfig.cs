using System;
using System.Collections.Generic;
using System.Text;
namespace DataLayer.Config
{
    public static class AppConfig
    {
        public static bool UseApi { get; set; } = true;

        // http au lieu de https !
        public static string BaseApiUrl { get; set; }
            = "http://worldcup-vua.nullbit.hr";

        public static string JsonFilesPath { get; set; }
            = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "JsonData");
    }
}