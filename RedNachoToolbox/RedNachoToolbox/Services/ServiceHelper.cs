using System;
using Microsoft.Extensions.DependencyInjection;

namespace RedNachoToolbox.Services;

/// <summary>
/// Helper for resolving services from the MAUI dependency injection container.
/// WARNING: This is an anti-pattern (Service Locator). Use constructor injection instead.
/// </summary>
[Obsolete("Use constructor injection instead of service locator pattern. This class will be removed in future versions.")]
public static class ServiceHelper
{
    public static IServiceProvider Services { get; set; } = default!;

    [Obsolete("Use constructor injection instead")]
    public static T GetRequiredService<T>() where T : notnull => Services.GetRequiredService<T>();
}

