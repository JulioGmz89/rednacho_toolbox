using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui;
using RedNachoToolbox.Services; // added
using RedNachoToolbox.ViewModels; // added
using RedNachoToolbox.Tools.MarkdownToPdf; // future tool registrations
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif

namespace RedNachoToolbox;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                // Inter Font Family - Google Fonts TTF Collection (18pt variants)
                // Regular weights
                fonts.AddFont("Inter/Inter_18pt-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter/Inter_18pt-Medium.ttf", "InterMedium");
                fonts.AddFont("Inter/Inter_18pt-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter/Inter_18pt-Bold.ttf", "InterBold");
                fonts.AddFont("Inter/Inter_18pt-ExtraBold.ttf", "InterExtraBold");
                fonts.AddFont("Inter/Inter_18pt-Black.ttf", "InterBlack");
                fonts.AddFont("Inter/Inter_18pt-Light.ttf", "InterLight");
                fonts.AddFont("Inter/Inter_18pt-ExtraLight.ttf", "InterExtraLight");
                fonts.AddFont("Inter/Inter_18pt-Thin.ttf", "InterThin");
                
                // Italic weights
                fonts.AddFont("Inter/Inter_18pt-Italic.ttf", "InterItalic");
                fonts.AddFont("Inter/Inter_18pt-MediumItalic.ttf", "InterMediumItalic");
                fonts.AddFont("Inter/Inter_18pt-SemiBoldItalic.ttf", "InterSemiBoldItalic");
                fonts.AddFont("Inter/Inter_18pt-BoldItalic.ttf", "InterBoldItalic");
                fonts.AddFont("Inter/Inter_18pt-ExtraBoldItalic.ttf", "InterExtraBoldItalic");
                fonts.AddFont("Inter/Inter_18pt-BlackItalic.ttf", "InterBlackItalic");
                fonts.AddFont("Inter/Inter_18pt-LightItalic.ttf", "InterLightItalic");
                fonts.AddFont("Inter/Inter_18pt-ExtraLightItalic.ttf", "InterExtraLightItalic");
                fonts.AddFont("Inter/Inter_18pt-ThinItalic.ttf", "InterThinItalic");
            });

        // Service registration (DI)
        builder.Services.AddSingleton<IToolRegistry, ToolRegistry>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();
        
        // Configure logging
        builder.Logging.AddDebug();
     
#if DEBUG
        // Add console logging in debug mode for better diagnostics
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        // Future: builder.Services.AddSingleton<IThemeService, ThemeService>();

        // Hide native On/Off text beside Switch on Windows (ToggleSwitch OnContent/OffContent)
        SwitchHandler.Mapper.AppendToMapping("HideWinSwitchLabels", (handler, view) =>
        {
#if WINDOWS
            if (handler?.PlatformView is ToggleSwitch ts)
     {
           ts.OnContent = string.Empty;
    ts.OffContent = string.Empty;
            }
#endif
        });

        var app = builder.Build();
        // Initialize static helper to resolve services from pages without constructor DI
        ServiceHelper.Services = app.Services;
     return app;
    }
}