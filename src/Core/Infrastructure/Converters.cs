using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TopNotch.Core.Infrastructure;

/// <summary>
/// Boolean to visibility converter (Avalonia uses bool for IsVisible)
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && b);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && b;
    }
}

/// <summary>
/// Inverse boolean to visibility converter
/// </summary>
public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !(value is bool b && b);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !(value is bool b && b);
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