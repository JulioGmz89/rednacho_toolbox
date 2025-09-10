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
            // Expanded sidebar: 280px width (wider for better spacing)
            return new GridLength(isCollapsed ? 80 : 280);
        }
        
        // Default to expanded width
        return new GridLength(280);
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
