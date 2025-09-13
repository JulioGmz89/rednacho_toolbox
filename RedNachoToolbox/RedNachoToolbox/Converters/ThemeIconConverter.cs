using System.Globalization;
using System.IO;

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
                values[0] is string baseOrPath && 
                values[1] is bool isDarkTheme &&
                !string.IsNullOrEmpty(baseOrPath))
            {
                // Determine if input is a full PNG path (contains extension) or a base name
                bool hasPngExtension = baseOrPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
                if (hasPngExtension)
                {
                    // If it's a PNG path, check for _black/_white suffix to theme-switch
                    var fileNameNoExt = Path.GetFileNameWithoutExtension(baseOrPath);
                    if (fileNameNoExt.EndsWith("_black", StringComparison.OrdinalIgnoreCase) ||
                        fileNameNoExt.EndsWith("_white", StringComparison.OrdinalIgnoreCase))
                    {
                        // Remove suffix and apply themed suffix
                        var baseName = fileNameNoExt
                            .Replace("_black", string.Empty, StringComparison.OrdinalIgnoreCase)
                            .Replace("_white", string.Empty, StringComparison.OrdinalIgnoreCase);
                        string themed = isDarkTheme ? $"{baseName}_white.png" : $"{baseName}_black.png";
                        System.Diagnostics.Debug.WriteLine($"ThemeIconConverter: from path '{baseOrPath}' themed -> '{themed}'");
                        return themed;
                    }
                    // No theme suffix present; return original path unchanged
                    System.Diagnostics.Debug.WriteLine($"ThemeIconConverter: using original PNG path '{baseOrPath}' (no theme suffix to swap)");
                    return baseOrPath;
                }

                // Treat as base icon name; append theme suffix and .png
                string iconName = isDarkTheme ? $"{baseOrPath}_white.png" : $"{baseOrPath}_black.png";
                System.Diagnostics.Debug.WriteLine($"ThemeIconConverter: baseIconName='{baseOrPath}', isDarkTheme={isDarkTheme}, result='{iconName}'");
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
