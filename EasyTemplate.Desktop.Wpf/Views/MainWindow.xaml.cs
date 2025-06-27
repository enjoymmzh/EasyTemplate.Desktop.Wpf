using MahApps.Metro.Controls;
using EasyTemplate.Desktop.Wpf.ViewModels;
using EasyTemplate.Desktop.Wpf.Models;
using AutoUpdaterDotNET;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System;
using System.Windows;
using System.IO;
using EasyTemplate.Desktop.Wpf.Common;

namespace EasyTemplate.Desktop.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow() 
        {
            InitializeComponent();

            var context = new MainViewModel();
            this.DataContext = context;

            Assembly assembly = Assembly.GetEntryAssembly();
            context.Version = $"當前版本: {assembly.GetName().Version}";

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 1;
            AutoUpdater.ReportErrors = true;


            this.WindowState = Global.config.IsMaxWindow ? WindowState.Maximized : WindowState.Normal;

            //notify1
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AutoUpdater.Start("http://xxx.my.com/update/AutoUpdaterStarter.xml");
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (Global.config.IsMaxWindow != this.WindowState is WindowState.Maximized)
                {
                    Global.config.IsMaxWindow = this.WindowState is WindowState.Maximized;
                    File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}configuration\\config.json", Global.config.ToJson());
                }
            }
            catch (Exception) { }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.Show();
            this.WindowState = Global.config.IsMaxWindow ? WindowState.Maximized : WindowState.Normal;
            this.Topmost = false;
        }

        private void notify1_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            MenuItem_Click_1(sender, e);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Global.config.IsCloseToTray)
            {
                e.Cancel = true;
                this.Hide();
                this.WindowState = WindowState.Minimized;
            }
        }
    }
}
