using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.ComponentModel;

namespace RedNachoToolbox.Controls;

/// <summary>
/// A complete color picker control with visual picker, hex input, and presets.
/// </summary>
public partial class ColorPickerControl : ContentView, INotifyPropertyChanged
{
    public static readonly BindableProperty PickedColorProperty =
        BindableProperty.Create(
        nameof(PickedColor),
    typeof(Color),
  typeof(ColorPickerControl),
     Colors.Black,
   BindingMode.TwoWay,
         propertyChanged: OnPickedColorChanged);

    public Color PickedColor
    {
        get => (Color)GetValue(PickedColorProperty);
        set => SetValue(PickedColorProperty, value);
    }

    private string _hexColor = "#000000";
    public string HexColor
    {
  get => _hexColor;
        set
 {
            if (_hexColor != value)
  {
         _hexColor = value;
              OnPropertyChanged(nameof(HexColor));
    }
   }
    }

    private bool _isUpdatingFromColor;

    public ColorPickerControl()
    {
        InitializeComponent();
      UpdateHexFromColor(PickedColor);
    }

    private static void OnPickedColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ColorPickerControl control && newValue is Color color)
    {
            control.UpdateHexFromColor(color);
  }
    }

  private void UpdateHexFromColor(Color color)
    {
 if (_isUpdatingFromColor) return;

        _isUpdatingFromColor = true;
     try
        {
            HexColor = ColorToHex(color);
 }
        finally
   {
            _isUpdatingFromColor = false;
        }
    }

    private void OnHexEntryCompleted(object? sender, EventArgs e)
    {
        UpdateColorFromHex();
    }

    private void OnPresetColorTapped(object? sender, TappedEventArgs e)
    {
   if (e.Parameter is string hex)
{
    HexColor = hex;
      UpdateColorFromHex();
        }
    }

    private void UpdateColorFromHex()
    {
  if (_isUpdatingFromColor) return;

      try
 {
  var color = HexToColor(HexColor);
 if (color != Colors.Transparent) // Use a sentinel value check instead of nullable
    {
   _isUpdatingFromColor = true;
     PickedColor = color;
      }
        }
        catch
   {
 // Invalid hex, revert to current color
   UpdateHexFromColor(PickedColor);
   }
 finally
  {
    _isUpdatingFromColor = false;
        }
  }

    private static string ColorToHex(Color color)
    {
     int r = (int)(color.Red * 255);
        int g = (int)(color.Green * 255);
int b = (int)(color.Blue * 255);
     return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static Color HexToColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
   return Colors.Transparent; // Return sentinel value

        hex = hex.Trim();
  if (!hex.StartsWith("#"))
       hex = "#" + hex;

    if (hex.Length != 7)
       return Colors.Transparent; // Return sentinel value

   try
        {
       int r = Convert.ToInt32(hex.Substring(1, 2), 16);
   int g = Convert.ToInt32(hex.Substring(3, 2), 16);
            int b = Convert.ToInt32(hex.Substring(5, 2), 16);
 return Color.FromRgb(r, g, b);
        }
   catch
      {
        return Colors.Transparent; // Return sentinel value
        }
    }
}
