namespace EasyTemplate.Desktop.Wpf.Models;

/// <summary>
/// 
/// </summary>
public enum EStatus
{
    下载完成,
    下载错误,
    下载中
}

public enum ETagType
{
    Id,
    ClassName,
    LinkText,
    TagName,
    Name,
    PartialLinkText,
    XPath,
    CssSelector,
}

public enum EAction
{
    SendKeys,
    Submit,
    Click,
    Clear,
    Text,
}

public enum ECrawType
{
    Click,
    Cartoon,
}

public enum ColorScheme
{
    Primary,
    Secondary,
    PrimaryForeground,
    SecondaryForeground
}
