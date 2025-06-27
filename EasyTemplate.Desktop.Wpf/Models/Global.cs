namespace EasyTemplate.Desktop.Wpf.Models;

public class Global
{
    public static Config config { get; set; }
    public static string productPath { get; set; }
}

public class Config
{
    public string GeneralUrl { get; set; }
    public bool IsDarkTheme { get; set; }
    public bool IsColorAdjusted { get; set; }
    public bool IsMaxWindow { get; set; }
    public bool IsCloseToTray { get; set; }
    public bool IsAutoRun { get; set; }
}

public class TheMessage
{
    public string title { get; set; } = "提示";
    public string message { get; set; }
}


