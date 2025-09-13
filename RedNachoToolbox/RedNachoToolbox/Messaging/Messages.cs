using CommunityToolkit.Mvvm.Messaging.Messages;
using RedNachoToolbox.Models;

namespace RedNachoToolbox.Messaging;

// Message broadcast when a tool should be opened in the MainPage content area
public sealed class OpenToolMessage : ValueChangedMessage<ToolInfo>
{
    public OpenToolMessage(ToolInfo value) : base(value) { }
}

// Message broadcast when the theme changes, carries the effective IsDarkTheme flag
public sealed class ThemeChangedMessage : ValueChangedMessage<bool>
{
    public ThemeChangedMessage(bool isDarkTheme) : base(isDarkTheme) { }
}
