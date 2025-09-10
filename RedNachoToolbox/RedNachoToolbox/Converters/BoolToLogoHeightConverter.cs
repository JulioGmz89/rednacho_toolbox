using System.Globalization;

namespace RedNachoToolbox.Converters;

/// <summary>
/// Converts a boolean value to a height for the logo image.
/// True (collapsed) returns smaller height, False (expanded) returns normal height.
/// </summary>
public class BoolToLogoHeightConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isCollapsed)
        {
            // Collapsed sidebar: 48px height (compact logo)
            // Expanded sidebar: 64px height (smaller logo for wider sidebar)
            return isCollapsed ? 48 : 64;
        }
        
        // Default to smaller height
        return 64;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
