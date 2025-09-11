using Microsoft.Extensions.Logging;

namespace RedNachoToolbox;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}