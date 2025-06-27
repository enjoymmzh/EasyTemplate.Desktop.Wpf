using System;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Desktop.Wpf.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace EasyTemplate.Desktop.Wpf.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region 命令

    [RelayCommand]
    private void Exit()
    {
        Environment.Exit(0);
    }

    [RelayCommand]
    private void OpenProduct()=> Process.Start("Explorer.exe", $"{AppDomain.CurrentDomain.BaseDirectory}products");

    [RelayCommand]
    private void OpenExport()=> Process.Start("Explorer.exe", $"{AppDomain.CurrentDomain.BaseDirectory}export");
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] public string version;
}

public class aaaMessage
{
    public string Message { get; set; }
}
