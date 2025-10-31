using Microsoft.Maui.Controls;

namespace RedNachoToolbox.Controls;

/// <summary>
/// A reusable loading overlay that can be shown/hidden with smooth animations.
/// </summary>
public partial class LoadingOverlay : ContentView
{
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(
      nameof(IsLoading),
     typeof(bool),
  typeof(LoadingOverlay),
    false,
       propertyChanged: OnIsLoadingChanged);

   public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(
            nameof(Message),
     typeof(string),
            typeof(LoadingOverlay),
        "Loading...");

    public bool IsLoading
    {
get => (bool)GetValue(IsLoadingProperty);
 set => SetValue(IsLoadingProperty, value);
    }

    public string Message
    {
  get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public LoadingOverlay()
    {
      InitializeComponent();
 }

    private static async void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LoadingOverlay overlay)
        {
     bool isLoading = (bool)newValue;
        
         if (isLoading)
 {
overlay.IsVisible = true;
       overlay.Opacity = 0;
    await overlay.FadeTo(1, 150, Easing.CubicOut);
   }
   else
       {
     await overlay.FadeTo(0, 150, Easing.CubicIn);
     overlay.IsVisible = false;
    }
        }
    }
}
