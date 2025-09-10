using System.Globalization;

namespace RedNachoToolbox.Converters;

/// <summary>
/// Converts a boolean value to a Thickness for sidebar padding.
/// True (collapsed) returns smaller padding, False (expanded) returns normal padding.
/// </summary>
public class BoolToPaddingConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isCollapsed)
        {
            // Collapsed sidebar: 8px padding (minimal)
            // Expanded sidebar: 16px padding (normal)
            return new Thickness(isCollapsed ? 8 : 16);
        }
        
        // Default to normal padding
        return new Thickness(16);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
