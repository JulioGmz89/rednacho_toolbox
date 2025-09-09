namespace RedNachoToolbox;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Apply saved theme preference at startup
        SettingsPage.ApplySavedTheme();

        MainPage = new AppShell();
    }
}
