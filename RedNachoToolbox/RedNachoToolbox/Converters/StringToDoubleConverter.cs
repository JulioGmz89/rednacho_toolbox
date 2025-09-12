using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RedNachoToolbox.Converters
{
    // Converts between numeric Entry text (string) and double ViewModel properties.
    // - Empty or invalid text becomes 0
    // - Clamps to a reasonable range [0, 1000] by default
    public class StringToDoubleConverter : IValueConverter
    {
        public double Min { get; set; } = 0d;
        public double Max { get; set; } = 1000d;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                // Render with no trailing decimals if integer, else with up to 2 decimals
                if (Math.Abs(d % 1) < 0.0000001)
                    return ((int)Math.Round(d)).ToString(culture);
                return d.ToString("0.##", culture);
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var s = value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(s))
                    return Min;

                // Allow leading '#', spaces, etc. not expected here; just parse number
                if (double.TryParse(s, NumberStyles.Float, culture, out var d))
                {
                    if (double.IsNaN(d) || double.IsInfinity(d)) return Min;
                    if (d < Min) d = Min;
                    if (d > Max) d = Max;
                    return d;
                }
                return Min;
            }
            catch
            {
                return Min;
            }
        }
    }
}
