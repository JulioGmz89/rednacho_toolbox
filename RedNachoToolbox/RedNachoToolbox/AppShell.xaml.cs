using Microsoft.Maui.Controls;
using RedNachoToolbox.Tools.MarkdownToPdf;

namespace RedNachoToolbox;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(MarkdownToPdfPage), typeof(MarkdownToPdfPage));
    }
}