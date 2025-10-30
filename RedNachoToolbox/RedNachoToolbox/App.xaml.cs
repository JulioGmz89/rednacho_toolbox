using RedNachoToolbox.Services;

namespace RedNachoToolbox;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Initialize theme service (will be resolved from DI after app is built)
        // Theme will be applied in OnStart
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        base.OnStart();

        // Apply saved theme preference at startup using IThemeService
        try
        {
            var themeService = ServiceHelper.Services?.GetService(typeof(IThemeService)) as IThemeService;
            themeService?.Initialize();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing theme: {ex.Message}");
            // Fallback to legacy method if service not available
            SettingsPage.ApplySavedTheme();
        }
    }
}
