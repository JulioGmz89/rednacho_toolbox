using System;
using Microsoft.Extensions.DependencyInjection;

namespace RedNachoToolbox.Services;

/// <summary>
/// Helper estático para resolver servicios registrados en el contenedor de MAUI.
/// Evita tener que pasar IServiceProvider manualmente a cada vista.
/// </summary>
public static class ServiceHelper
{
    public static IServiceProvider Services { get; set; } = default!;

    public static T GetRequiredService<T>() where T : notnull => Services.GetRequiredService<T>();
}

