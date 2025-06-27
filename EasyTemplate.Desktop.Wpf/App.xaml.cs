using System;
using System.IO;
using System.Windows;
using EasyTemplate.Desktop.Wpf.Common;
using EasyTemplate.Desktop.Wpf.Models;

namespace EasyTemplate.Desktop.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}configuration\\config.json"))
            {
                var info = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}configuration\\config.json");
                Global.config = info.ToModel<Config>();
            }
            else
            {
                Global.config = new Config() { 
                    GeneralUrl = "https://www.bing.com", 
                    IsDarkTheme = true, 
                    IsColorAdjusted = false, 
                    IsMaxWindow = false,
                    IsAutoRun = false,
                    IsCloseToTray = false,
                };
                File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}configuration\\config.json", Global.config.ToJson());
            }

        }
    }
}
