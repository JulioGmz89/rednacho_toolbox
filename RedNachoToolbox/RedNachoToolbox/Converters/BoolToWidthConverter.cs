using System.Globalization;

namespace RedNachoToolbox.Converters;

/// <summary>
/// Converts a boolean value to a GridLength for sidebar width.
/// True (collapsed) returns 80, False (expanded) returns 250.
/// </summary>
public class BoolToWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isCollapsed)
        {
            // Collapsed sidebar: 80px width (just enough for logo)
            // Expanded sidebar: 250px width (full sidebar)
            return new GridLength(isCollapsed ? 80 : 250);
        }
        
        // Default to expanded width
        return new GridLength(250);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is GridLength gridLength)
        {
            // Consider collapsed if width is less than 150
            return gridLength.Value < 150;
        }
        
        return false;
    }
}
