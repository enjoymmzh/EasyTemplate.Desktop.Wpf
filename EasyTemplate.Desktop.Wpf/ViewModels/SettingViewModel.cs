using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyTemplate.Desktop.Wpf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EasyTemplate.Desktop.Wpf.Common;
using System.IO;
using System;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace EasyTemplate.Desktop.Wpf.ViewModels;

public partial class SettingViewModel : ViewModelBase
{
    public SettingViewModel(Snackbar snackbar)
    {
        _snackbar = snackbar;

        PaletteHelper paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        GeneralUrl = Global.config.GeneralUrl;
        IsDarkTheme = Global.config.IsDarkTheme;
        ModifyTheme(theme => theme.SetBaseTheme(IsDarkTheme ? BaseTheme.Dark : BaseTheme.Light));
        IsColorAdjusted = Global.config.IsColorAdjusted;
        IsMaxWindow = Global.config.IsMaxWindow;
        IsAutoRun = Global.config.IsAutoRun;
        isDirectClose = Global.config.IsCloseToTray;

        if (theme is Theme internalTheme)
        {
            _isColorAdjusted = internalTheme.ColorAdjustment is not null;

            var colorAdjustment = internalTheme.ColorAdjustment ?? new ColorAdjustment();
            _desiredContrastRatio = colorAdjustment.DesiredContrastRatio;
            _contrastValue = colorAdjustment.Contrast;
            _colorSelectionValue = colorAdjustment.Colors;
        }

        if (paletteHelper.GetThemeManager() is { } themeManager)
        {
            themeManager.ThemeChanged += (_, e) =>
            {
                IsDarkTheme = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
            };
        }
    }

    private void Save()
    {
        try
        {
            File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}configuration\\config.json", Global.config.ToJson());
            if (_snackbar.MessageQueue is { } messageQueue)
            {
                var message = "保存成功";
                Task.Factory.StartNew(() => messageQueue.Enqueue(message));
            }
        }
        catch (Exception)
        {
            if (_snackbar.MessageQueue is { } messageQueue)
            {
                var message = "保存失敗";
                Task.Factory.StartNew(() => messageQueue.Enqueue(message));
            }
        }
    }

    private bool EnableAutoRun()
    {
        try
        {
            using var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            var path = AppDomain.CurrentDomain.BaseDirectory + System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
            registryKey?.SetValue("EasyTemplate.Desktop.Wpf", path);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private bool DisableAutoRun()
    {
        try
        {
            using var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            registryKey?.DeleteValue("EasyTemplate.Desktop.Wpf", false);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private void ModifyTheme(Action<Theme> modificationAction)
    {
        var paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();
        modificationAction?.Invoke(theme);
        paletteHelper.SetTheme(theme);
    }

    [RelayCommand]
    private void Clear()
    {
        try
        {
            var file1 = $"{AppDomain.CurrentDomain.BaseDirectory}products\\product.json";
            if (File.Exists(file1))
            {
                File.Delete(file1);
            }
        }
        catch { }

        try
        {
            var file2 = $"{AppDomain.CurrentDomain.BaseDirectory}products\\website.json";
            if (File.Exists(file2))
            {
                File.Delete(file2);
            }
        }
        catch { }

        if (_snackbar.MessageQueue is { } messageQueue)
        {
            var message = "清理完成，請重啟程序";
            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }
    }

    #region 字段

    private Snackbar _snackbar { get; set; }

    /// <summary>
    /// 
    /// </summary>
    private string generalUrl;
    public string GeneralUrl
    {
        get => generalUrl;
        set
        {
            OnPropertyChanged();
            if (SetProperty(ref generalUrl, value) && !Global.config.GeneralUrl.Equals(generalUrl))
            {
                Global.config.GeneralUrl = GeneralUrl;
                Save();
            }
        }
    }
    [ObservableProperty] private string generalToken;
    [ObservableProperty] private string dmtToken;
    /// <summary>
    /// 
    /// </summary>
    private bool isAutoRun;
    public bool IsAutoRun
    {
        get => isAutoRun;
        set
        {
            OnPropertyChanged();
            if (SetProperty(ref isAutoRun, value) && Global.config.IsAutoRun != isAutoRun)
            {
                Global.config.IsAutoRun = IsAutoRun;
                _ = IsAutoRun ? EnableAutoRun() : DisableAutoRun();
                Save();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private bool isDirectClose;
    public bool IsDirectClose
    {
        get => isDirectClose;
        set
        {
            OnPropertyChanged();
            if (SetProperty(ref isDirectClose, value) && Global.config.IsCloseToTray != isDirectClose)
            {
                Global.config.IsCloseToTray = IsDirectClose;
                Save();
            }
        }
    }
    private bool isDarkTheme;
    public bool IsDarkTheme
    {
        get => isDarkTheme;
        set
        {
            OnPropertyChanged();
            if (SetProperty(ref isDarkTheme, value) && Global.config.IsDarkTheme != IsDarkTheme)
            {
                Global.config.IsDarkTheme = IsDarkTheme;
                ModifyTheme(theme => theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light));
                Save();
            }
        }
    }
    private bool _isColorAdjusted;
    public bool IsColorAdjusted
    {
        get => _isColorAdjusted;
        set
        {
            if (SetProperty(ref _isColorAdjusted, value) && Global.config.IsColorAdjusted != IsColorAdjusted)
            {
                Global.config.IsColorAdjusted = IsColorAdjusted;
                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme)
                    {
                        internalTheme.ColorAdjustment = value
                            ? new ColorAdjustment
                            {
                                DesiredContrastRatio = DesiredContrastRatio,
                                Contrast = ContrastValue,
                                Colors = ColorSelectionValue
                            }
                            : null;
                    }
                });
                Save();
            }
        }
    }

    private float _desiredContrastRatio = 4.5f;
    public float DesiredContrastRatio
    {
        get => _desiredContrastRatio;
        set
        {
            if (SetProperty(ref _desiredContrastRatio, value))
            {
                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.DesiredContrastRatio = value;
                });
            }
        }
    }

    public IEnumerable<Contrast> ContrastValues => Enum.GetValues(typeof(Contrast)).Cast<Contrast>();

    private Contrast _contrastValue;
    public Contrast ContrastValue
    {
        get => _contrastValue;
        set
        {
            if (SetProperty(ref _contrastValue, value))
            {
                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.Contrast = value;
                });
            }
        }
    }

    public IEnumerable<ColorSelection> ColorSelectionValues => Enum.GetValues(typeof(ColorSelection)).Cast<ColorSelection>();

    private ColorSelection _colorSelectionValue;
    public ColorSelection ColorSelectionValue
    {
        get => _colorSelectionValue;
        set
        {
            if (SetProperty(ref _colorSelectionValue, value))
            {
                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.Colors = value;
                });
            }
        }
    }

    private bool isMaxWindow;
    public bool IsMaxWindow
    {
        get => isMaxWindow;
        set
        {
            if (SetProperty(ref isMaxWindow, value))
            {
                Save();
            }
        }
    }
    #endregion
}
