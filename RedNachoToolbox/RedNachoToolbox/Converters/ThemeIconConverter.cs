using System.Globalization;

namespace RedNachoToolbox.Converters;

/// <summary>
/// Converts a base icon name and theme state to the appropriate themed PNG icon path.
/// Uses "_white" suffix for dark theme icons and "_black" suffix for light theme icons.
/// </summary>
public class ThemeIconConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (values?.Length >= 2 && 
                values[0] is string baseIconName && 
                values[1] is bool isDarkTheme &&
                !string.IsNullOrEmpty(baseIconName))
            {
                // For PNG files, append theme suffix and include .png extension
                // Dark theme uses white icons, light theme uses black icons
                string iconName = isDarkTheme ? $"{baseIconName}_white.png" : $"{baseIconName}_black.png";
                
                System.Diagnostics.Debug.WriteLine($"ThemeIconConverter: baseIconName='{baseIconName}', isDarkTheme={isDarkTheme}, result='{iconName}'");
                return iconName;
            }
            
            System.Diagnostics.Debug.WriteLine($"ThemeIconConverter: Invalid parameters, using fallback 'dotnet_bot.png'");
            return "dotnet_bot.png"; // Fallback icon
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ThemeIconConverter error: {ex.Message}");
            return "dotnet_bot.png"; // Fallback on error
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
