using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjetG2AdminDev.Views;

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isPauseButton = System.Convert.ToBoolean(parameter);
        
        string? stringValue = value as string;
        
        if (stringValue is "Paused" or "Inactive")
        {
            return !isPauseButton ? Visibility.Visible : Visibility.Collapsed;
        }
        else if (stringValue == "Active")
        {
            return !isPauseButton ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
