using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace NonogramAvalonia.Converters;
public sealed class TimeSpanToTimerStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return AvaloniaProperty.UnsetValue;

        if (!targetType.IsAssignableFrom(typeof(string)))
            throw new ArgumentException($"Cannot convert from {typeof(string)} to {targetType}", nameof(targetType));

        if (value is TimeSpan timeSpan)
        {
            string timeString;

            if (timeSpan.Days > 0)
                timeString = timeSpan.ToString(@"d\:hh\:mm\:ss");
            else if (timeSpan.Hours > 0)
                timeString = timeSpan.ToString(@"h\:mm\:ss");
            else
                timeString = timeSpan.ToString(@"m\:ss");

            return timeString;
        }
        else
            throw new ArgumentException($"Cannot convert from {value.GetType()} to {typeof(TimeSpan)}", nameof(value));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
