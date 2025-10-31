using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace RedNachoToolbox.Controls;

/// <summary>
/// A custom color picker control using SkiaSharp for .NET 9 MAUI.
/// Provides HSV color selection with visual feedback.
/// </summary>
public class ColorPickerCanvas : SKCanvasView
{
    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(
            nameof(SelectedColor),
        typeof(Color),
        typeof(ColorPickerCanvas),
  Colors.Black,
  BindingMode.TwoWay,
      propertyChanged: OnSelectedColorChanged);

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    private float _hue;
    private float _saturation = 1f;
    private float _value = 1f;
    private SKPoint _pickerPosition;
    private SKPoint _huePosition;
    private const float PickerSize = 200f;
    private const float HueBarWidth = 30f;
    private const float Padding = 10f;

    public ColorPickerCanvas()
    {
        HeightRequest = PickerSize + Padding * 2;
        WidthRequest = PickerSize + HueBarWidth + Padding * 3;
        
    EnableTouchEvents = true;
        Touch += OnTouch;
        
// Initialize with current color
        UpdateFromColor(SelectedColor);
    }

    private static void OnSelectedColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
   if (bindable is ColorPickerCanvas picker && newValue is Color color)
     {
          picker.UpdateFromColor(color);
   picker.InvalidateSurface();
        }
    }

    private void UpdateFromColor(Color color)
    {
        // Convert RGBA to HSV
        var (h, s, v) = RgbToHsv(color.Red, color.Green, color.Blue);
   _hue = h;
        _saturation = s;
        _value = v;
        
        // Update picker positions
        _pickerPosition = new SKPoint(
            _saturation * PickerSize + Padding,
            (1 - _value) * PickerSize + Padding);
        
  _huePosition = new SKPoint(
         PickerSize + Padding * 2 + HueBarWidth / 2,
            _hue / 360f * PickerSize + Padding);
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var info = e.Info;
        float scale = info.Width / (float)Width;

        // Draw SV picker (left square)
        DrawSVPicker(canvas, scale);

     // Draw Hue bar (right)
        DrawHueBar(canvas, scale);

        // Draw selection indicators
      DrawSelectionIndicators(canvas, scale);
    }

    private void DrawSVPicker(SKCanvas canvas, float scale)
    {
   var pickerRect = new SKRect(
      Padding * scale,
            Padding * scale,
            (Padding + PickerSize) * scale,
(Padding + PickerSize) * scale);

        // Create HSV gradient shader
     var hueColor = SKColor.FromHsv(_hue, 100, 100);
        
        // Horizontal gradient (white to hue color) for saturation
        var saturationColors = new[] { SKColors.White, hueColor };
        var saturationShader = SKShader.CreateLinearGradient(
            new SKPoint(pickerRect.Left, pickerRect.Top),
        new SKPoint(pickerRect.Right, pickerRect.Top),
        saturationColors,
     SKShaderTileMode.Clamp);

        // Vertical gradient (transparent to black) for value
 var valueColors = new[] { SKColors.Transparent, SKColors.Black };
        var valueShader = SKShader.CreateLinearGradient(
     new SKPoint(pickerRect.Left, pickerRect.Top),
            new SKPoint(pickerRect.Left, pickerRect.Bottom),
            valueColors,
          SKShaderTileMode.Clamp);

        // Combine shaders
        var combinedShader = SKShader.CreateCompose(saturationShader, valueShader, SKBlendMode.Multiply);

        using var paint = new SKPaint
        {
            Shader = combinedShader,
            IsAntialias = true
        };

   canvas.DrawRoundRect(pickerRect, 8 * scale, 8 * scale, paint);

        // Draw border
        using var borderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Gray.WithAlpha(128),
      StrokeWidth = 1 * scale,
         IsAntialias = true
  };
 canvas.DrawRoundRect(pickerRect, 8 * scale, 8 * scale, borderPaint);
    }

    private void DrawHueBar(SKCanvas canvas, float scale)
    {
        var hueRect = new SKRect(
    (PickerSize + Padding * 2) * scale,
       Padding * scale,
            (PickerSize + Padding * 2 + HueBarWidth) * scale,
     (Padding + PickerSize) * scale);

   // Create hue gradient (rainbow)
        var hueColors = new SKColor[7];
        for (int i = 0; i < 7; i++)
  {
     hueColors[i] = SKColor.FromHsv(i * 60f, 100, 100);
        }

   var hueShader = SKShader.CreateLinearGradient(
 new SKPoint(hueRect.Left, hueRect.Top),
   new SKPoint(hueRect.Left, hueRect.Bottom),
            hueColors,
            SKShaderTileMode.Clamp);

        using var paint = new SKPaint
        {
     Shader = hueShader,
        IsAntialias = true
        };

     canvas.DrawRoundRect(hueRect, 8 * scale, 8 * scale, paint);

     // Draw border
   using var borderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
   Color = SKColors.Gray.WithAlpha(128),
            StrokeWidth = 1 * scale,
      IsAntialias = true
        };
  canvas.DrawRoundRect(hueRect, 8 * scale, 8 * scale, borderPaint);
  }

    private void DrawSelectionIndicators(SKCanvas canvas, float scale)
    {
      // Draw SV picker indicator
        var pickerX = _pickerPosition.X * scale;
        var pickerY = _pickerPosition.Y * scale;

        using (var paint = new SKPaint
  {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2 * scale,
          IsAntialias = true
        })
        {
            canvas.DrawCircle(pickerX, pickerY, 8 * scale, paint);
        }

        using (var paint = new SKPaint
        {
        Style = SKPaintStyle.Stroke,
   Color = SKColors.Black,
   StrokeWidth = 1 * scale,
            IsAntialias = true
    })
        {
        canvas.DrawCircle(pickerX, pickerY, 9 * scale, paint);
      }

        // Draw hue bar indicator
        var hueX = _huePosition.X * scale;
        var hueY = _huePosition.Y * scale;
        var hueLeft = (PickerSize + Padding * 2) * scale;
        var hueRight = (PickerSize + Padding * 2 + HueBarWidth) * scale;

 using (var paint = new SKPaint
        {
       Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2 * scale,
        IsAntialias = true
        })
        {
            canvas.DrawLine(hueLeft - 2 * scale, hueY, hueRight + 2 * scale, hueY, paint);
     }

        using (var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
     Color = SKColors.Black,
 StrokeWidth = 1 * scale,
  IsAntialias = true
  })
     {
     canvas.DrawLine(hueLeft - 3 * scale, hueY, hueRight + 3 * scale, hueY, paint);
        }
 }

    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
     var info = (sender as SKCanvasView)?.CanvasSize;
    if (info == null) return;

        float scale = info.Value.Width / (float)Width;
        var point = e.Location;

        var pickerRect = new SKRect(
            Padding * scale,
            Padding * scale,
  (Padding + PickerSize) * scale,
  (Padding + PickerSize) * scale);

        var hueRect = new SKRect(
      (PickerSize + Padding * 2) * scale,
   Padding * scale,
        (PickerSize + Padding * 2 + HueBarWidth) * scale,
            (Padding + PickerSize) * scale);

        switch (e.ActionType)
{
         case SKTouchAction.Pressed:
            case SKTouchAction.Moved:
              if (pickerRect.Contains(point))
      {
   // Update SV picker
        var s = (point.X - pickerRect.Left) / pickerRect.Width;
   var v = 1 - (point.Y - pickerRect.Top) / pickerRect.Height;
          
      _saturation = Math.Clamp(s, 0, 1);
          _value = Math.Clamp(v, 0, 1);
  
          _pickerPosition = new SKPoint(point.X / scale, point.Y / scale);
        
           UpdateSelectedColor();
                  InvalidateSurface();
           }
                else if (hueRect.Contains(point))
                {
  // Update hue bar
            var h = (point.Y - hueRect.Top) / hueRect.Height * 360f;
  _hue = Math.Clamp(h, 0, 360);
        
        _huePosition = new SKPoint(_huePosition.X, point.Y / scale);
            
    UpdateSelectedColor();
InvalidateSurface();
      }
     e.Handled = true;
           break;
        }
    }

    private void UpdateSelectedColor()
    {
        var (r, g, b) = HsvToRgb(_hue, _saturation, _value);
        SelectedColor = new Color(r, g, b);
    }

    private static (float h, float s, float v) RgbToHsv(float r, float g, float b)
    {
        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
      float delta = max - min;

  float h = 0;
        if (delta > 0)
   {
    if (max == r)
      h = 60 * (((g - b) / delta) % 6);
       else if (max == g)
        h = 60 * (((b - r) / delta) + 2);
     else
      h = 60 * (((r - g) / delta) + 4);
    }
  
        if (h < 0) h += 360;

        float s = max == 0 ? 0 : delta / max;
        float v = max;

    return (h, s, v);
    }

    private static (float r, float g, float b) HsvToRgb(float h, float s, float v)
    {
        float c = v * s;
    float x = c * (1 - Math.Abs((h / 60f) % 2 - 1));
 float m = v - c;

        float r = 0, g = 0, b = 0;

        if (h < 60)
   {
        r = c; g = x; b = 0;
        }
        else if (h < 120)
        {
            r = x; g = c; b = 0;
    }
    else if (h < 180)
        {
   r = 0; g = c; b = x;
        }
        else if (h < 240)
   {
            r = 0; g = x; b = c;
        }
else if (h < 300)
        {
       r = x; g = 0; b = c;
      }
        else
 {
       r = c; g = 0; b = x;
        }

        return (r + m, g + m, b + m);
    }
}
