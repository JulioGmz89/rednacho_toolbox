using RedNachoToolbox.Models;
using System.Collections.ObjectModel;

namespace RedNachoToolbox.Services;

/// <summary>
/// Contrato para registrar y obtener herramientas disponibles en la aplicación.
/// </summary>
public interface IToolRegistry
{
    /// <summary>
    /// Devuelve la lista inmutable de herramientas registradas.
    /// </summary>
    IReadOnlyList<ToolInfo> GetAll();

    /// <summary>
    /// Registra una nueva herramienta si aún no existe por nombre.
    /// </summary>
    void Register(ToolInfo tool);

    /// <summary>
    /// Busca una herramienta por nombre (case-insensitive).
    /// </summary>
    ToolInfo? FindByName(string name);
}

