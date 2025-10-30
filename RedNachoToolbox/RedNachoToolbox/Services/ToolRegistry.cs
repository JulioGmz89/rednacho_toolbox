using RedNachoToolbox.Models;
using RedNachoToolbox.Tools.MarkdownToPdf;

namespace RedNachoToolbox.Services;

/// <summary>
/// Registro simple de herramientas. En esta primera iteración está embebido; más adelante podría cargarse dinámicamente.
/// </summary>
public class ToolRegistry : IToolRegistry
{
    private readonly List<ToolInfo> _tools = new();
    private bool _initialized;
    private readonly object _lock = new();

    public ToolRegistry()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (_initialized) return;
        lock (_lock)
        {
            if (_initialized) return;
            // Registro inicial (Markdown to PDF)
            Register(new ToolInfo(
                name: "Markdown to PDF",
                description: "Convert Markdown to PDF with customizable styles and live preview.",
                iconPath: "md_to_pdf_lightmode_black.png",
                category: ToolCategory.Productivity,
                targetType: typeof(MarkdownToPdfView)
            ));
            _initialized = true;
        }
    }

    public IReadOnlyList<ToolInfo> GetAll() => _tools.AsReadOnly();

    public void Register(ToolInfo tool)
    {
        if (tool == null) throw new ArgumentNullException(nameof(tool));
        if (_tools.Any(t => string.Equals(t.Name, tool.Name, StringComparison.OrdinalIgnoreCase))) return; // evitar duplicados
        _tools.Add(tool);
    }

    public ToolInfo? FindByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return _tools.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}

