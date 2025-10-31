using RedNachoToolbox.Services;
using Microsoft.Extensions.DependencyInjection;

namespace RedNachoToolbox;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));

        InitializeComponent();

        // Initialize theme service
        try
        {
            var themeService = _services.GetRequiredService<IThemeService>();
            themeService.Initialize();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing theme: {ex.Message}");
        }

        MainPage = new AppShell();
    }
}
