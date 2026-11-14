using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using System;

namespace TopNotch.Core.Infrastructure;

/// <summary>
/// Boolean to visibility converter
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is Visibility v && v == Visibility.Visible);
    }
}

/// <summary>
/// Inverse boolean to visibility converter
/// </summary>
public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && !b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is Visibility v && v == Visibility.Visible);
    }
}

/// <summary>
/// Time converter for displaying times
/// </summary>
public class TimeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            return dt.ToString("h:mm tt");
        }
        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && DateTime.TryParse(s, out var dt))
        {
            return dt;
        }
        return DateTime.Now;
    }
}

/// <summary>
/// Null to visibility converter
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value != null;
        return Invert ? !result : result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}