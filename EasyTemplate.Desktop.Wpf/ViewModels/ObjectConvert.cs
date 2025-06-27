using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace EasyTemplate.Desktop.Wpf.ViewModels;

/// <summary>
/// CommandParameter 多参数传递
/// </summary>
public partial class ObjectConvert : IMultiValueConverter
{
    #region IMultiValueConverter Members

    public static object ConverterObject;

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return values.ToArray();
    }

    public object[] ConvertBack(object value, Type[] targetTypes,
      object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}

[ValueConversion(typeof(Visibility), typeof(Visibility))]
public class InverseVisibilityConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
    {
        if (targetType != typeof(Visibility))
            throw new InvalidOperationException("The target must be a Visibility");

        var res = (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        return res;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    #endregion
}

[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBoolConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
         System.Globalization.CultureInfo culture)
    {
        if (targetType != typeof(bool))
            throw new InvalidOperationException("The target must be a bool");

        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    #endregion
}

