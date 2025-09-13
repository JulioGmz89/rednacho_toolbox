using System.Collections.ObjectModel;
using System.Text;
using Markdig;
using PdfSharpCore;
using RedNachoToolbox.ViewModels;
using System.IO;
using Microsoft.Maui.Graphics;
using HtmlRendererCore.PdfSharp;
using PdfSharpCore.Pdf;

namespace RedNachoToolbox.Tools.MarkdownToPdf;

public class MarkdownToPdfViewModel : BaseViewModel
{
    private string _markdownText = string.Empty;
    private string _sampleMarkdown = GetDefaultSampleMarkdown();
    private string _htmlPreview = string.Empty;

    public ObservableCollection<string> AvailableFonts { get; } = new()
    {
        "Sans-serif", "Serif", "Monospace"
    };

    private string _fontFamily = "Sans-serif";
    public string FontFamily
    {
        get => _fontFamily;
        set { if (SetProperty(ref _fontFamily, value)) RebuildPreviewHtml(); }
    }

    // Provide the full HTML (with export CSS) for engines that can print HTML directly (e.g., WebView2 on Windows)
    public string GetExportHtml() => BuildHtmlForExport(_markdownText);

    private double _fontSize = 12;
    public double FontSize
    {
        get => _fontSize;
        set { if (SetProperty(ref _fontSize, value)) RebuildPreviewHtml(); }
    }

    private string _textColorHex = "#222222";
    public string TextColorHex
    {
        get => _textColorHex;
        set
        {
            if (SetProperty(ref _textColorHex, value))
            {
                // Keep Color property in sync
                _textColor = HexToColor(_textColorHex);
                OnPropertyChanged(nameof(TextColor));
                RebuildPreviewHtml();
            }
        }
    }

    // New: Color property for UI color pickers
    private Color _textColor = Color.FromArgb("#222222");
    public Color TextColor
    {
        get => _textColor;
        set
        {
            if (SetProperty(ref _textColor, value))
            {
                // Keep hex in sync for CSS generation
                _textColorHex = ColorToHex(value);
                OnPropertyChanged(nameof(TextColorHex));
                RebuildPreviewHtml();
            }
        }
    }

    private static string ColorToHex(Color c)
    {
        // ignore alpha for CSS text color
        byte r = (byte)Math.Round(c.Red * 255);
        byte g = (byte)Math.Round(c.Green * 255);
        byte b = (byte)Math.Round(c.Blue * 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static Color HexToColor(string hex)
    {
        try { return Color.FromArgb(hex); } catch { return Color.FromArgb("#222222"); }
    }

    // Word default margins ~ 2.54 cm
    private double _marginTop = 2.54;
    public double MarginTop { get => _marginTop; set { if (SetProperty(ref _marginTop, value)) RebuildPreviewHtml(); } }
    private double _marginRight = 2.54;
    public double MarginRight { get => _marginRight; set { if (SetProperty(ref _marginRight, value)) RebuildPreviewHtml(); } }
    private double _marginBottom = 2.54;
    public double MarginBottom { get => _marginBottom; set { if (SetProperty(ref _marginBottom, value)) RebuildPreviewHtml(); } }
    private double _marginLeft = 2.54;
    public double MarginLeft { get => _marginLeft; set { if (SetProperty(ref _marginLeft, value)) RebuildPreviewHtml(); } }

    private string _pageSize = "A4";
    public string PageSize
    {
        get => _pageSize;
        set { if (SetProperty(ref _pageSize, value)) RebuildPreviewHtml(); }
    }

    private string _theme = "Light";
    public string Theme
    {
        get => _theme;
        set { if (SetProperty(ref _theme, value)) RebuildPreviewHtml(); }
    }

    public string MarkdownText
    {
        get => _markdownText;
        set
        {
            if (SetProperty(ref _markdownText, value))
            {
                RebuildPreviewHtml();
                UpdateLineNumbersFromMarkdown();
            }
        }
    }

    public string SampleMarkdown
    {
        get => _sampleMarkdown;
        set { SetProperty(ref _sampleMarkdown, value); }
    }

    public string HtmlPreview
    {
        get => _htmlPreview;
        private set => SetProperty(ref _htmlPreview, value);
    }

    // Toggles right pane between preview (false) and editor (true)
    private bool _isEditorMode = false;
    public bool IsEditorMode
    {
        get => _isEditorMode;
        set
        {
            if (SetProperty(ref _isEditorMode, value))
            {
                OnPropertyChanged(nameof(RightPaneTitle));
                if (!value)
                {
                    // Switching to preview: ensure HTML is rebuilt
                    RebuildPreviewHtml();
                }
            }
        }
    }

    // Title shown in right pane header
    public string RightPaneTitle => IsEditorMode ? "Markdown Editor" : "Preview";

    // Line numbers shown alongside the editor
    private string _lineNumbersText = "1";
    public string LineNumbersText
    {
        get => _lineNumbersText;
        private set => SetProperty(ref _lineNumbersText, value);
    }

    public MarkdownToPdfViewModel()
    {
        Title = "Markdown to PDF";
        // Prefill the editor with the same content as the placeholder sample
        // so users see a ready-to-edit example immediately.
        MarkdownText = _sampleMarkdown;
    }

    public void RebuildPreviewHtml()
    {
        var html = BuildHtmlForPreview(_markdownText, _sampleMarkdown);
        HtmlPreview = html;
    }

    private void UpdateLineNumbersFromMarkdown()
    {
        var text = _markdownText ?? string.Empty;
        var lineCount = text.Length == 0 ? 1 : text.Split('\n').Length;
        if (lineCount < 1) lineCount = 1;
        var sb = new StringBuilder();
        for (int i = 1; i <= lineCount; i++)
        {
            sb.Append(i);
            if (i < lineCount) sb.Append('\n');
        }
        LineNumbersText = sb.ToString();
    }

    public async Task<byte[]> GeneratePdfAsync()
    {
        try
        {
            IsBusy = true;
            var html = BuildHtmlForExport(_markdownText);

            // Ejecutar la generación en un subproceso en segundo plano para no bloquear la UI
            var bytes = await Task.Run(() =>
            {
                var config = new PdfGenerateConfig
                {
                    PageSize = PageSize == "Letter" ? PdfSharpCore.PageSize.Letter : PdfSharpCore.PageSize.A4,
                    MarginTop = CmToPointsInt(MarginTop),
                    MarginRight = CmToPointsInt(MarginRight),
                    MarginBottom = CmToPointsInt(MarginBottom),
                    MarginLeft = CmToPointsInt(MarginLeft)
                };

                PdfDocument doc = PdfGenerator.GeneratePdf(html, config);
                using var ms = new MemoryStream();
                doc.Save(ms);
                return ms.ToArray();
            });

            return bytes;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static int CmToPointsInt(double cm)
    {
        return (int)Math.Round(cm / 2.54 * 72.0);
    }

    private string BuildHtmlForPreview(string userMarkdown, string sampleMarkdown)
    {
        var pipeline = BuildMarkdownPipeline();
        var sb = new StringBuilder();
        var css = BuildCss(forExport: false);

        sb.Append("<!DOCTYPE html><html><head><meta charset='utf-8' /><style>")
          .Append(css)
          .Append("</style></head><body>");

        // Only render user's markdown in preview now (sample is placeholder in editor)
        sb.Append("<section class='section'>")
          .Append(Markdig.Markdown.ToHtml(userMarkdown ?? string.Empty, pipeline))
          .Append("</section>");

        sb.Append("</body></html>");
        return sb.ToString();
    }

    private string BuildHtmlForExport(string userMarkdown)
    {
        var pipeline = BuildMarkdownPipeline();
        var css = BuildCss(forExport: true);
        var body = Markdig.Markdown.ToHtml(userMarkdown ?? string.Empty, pipeline);
        return $"<!DOCTYPE html><html><head><meta charset='utf-8'/><style>{css}</style></head><body><section class='section'>{body}</section></body></html>";
    }

    private string BuildCss(bool forExport)
    {
        // Map selected font option to CSS stack
        var fontStack = FontFamily switch
        {
            "Serif" => "\"Times New Roman\", Times, serif",
            "Monospace" => "\"Courier New\", Courier, monospace",
            _ => "-apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, Helvetica, Arial, \"Noto Sans\", \"Liberation Sans\", sans-serif"
        };

        var textColor = string.IsNullOrWhiteSpace(TextColorHex) ? "#222222" : TextColorHex.Trim();
        var bg = Theme == "Dark" ? "#111111" : "#FFFFFF";
        var codeBg = Theme == "Dark" ? "#1e1e1e" : "#f5f5f5";
        var border = Theme == "Dark" ? "#333333" : "#e0e0e0";
        var hColor = textColor; // could choose different shade

        if (!forExport)
        {
            // Screen/preview CSS
            return $@"
body {{
  background: {bg};
  color: {textColor};
  font-family: {fontStack};
  font-size: {FontSize}pt;
  line-height: 1.6;
  padding: 16px;
  margin: {MarginTop}cm {MarginRight}cm {MarginBottom}cm {MarginLeft}cm;
}}
.section {{
  max-width: 900px;
  margin: 0 auto 24px auto;
}}
h1,h2,h3,h4,h5,h6 {{
  color: {hColor};
  margin: 1.2em 0 0.6em 0;
  line-height: 1.25;
}}
h1 {{ font-size: {FontSize * 2.0:0.##}pt; }}
h2 {{ font-size: {FontSize * 1.6:0.##}pt; }}
h3 {{ font-size: {FontSize * 1.3:0.##}pt; }}
p, li {{
  margin: 0.5em 0;
}}
ul, ol {{ padding-left: 1.2em; }}
hr {{ border: 0; height: 1px; background: {border}; margin: 24px 0; }}
blockquote {{
  border-left: 4px solid {border};
  margin: 1em 0;
  padding: 0.2em 1em;
  color: {(Theme=="Dark"?"#cccccc":"#555555")};
}}
code {{
  background: {codeBg};
  padding: 0.2em 0.4em;
  border-radius: 4px;
  font-family: ""Courier New"", Courier, monospace;
}}
pre code {{ display: block; padding: 12px; overflow: auto; }}
table {{ border-collapse: collapse; width: 100%; margin: 12px 0; }}
th, td {{ border: 1px solid {border}; padding: 8px; text-align: left; }}
img, svg {{ max-width: 100%; height: auto; }}
";
        }
        else
        {
            // Print/PDF CSS
            bg = "#FFFFFF"; // Forzar blanco en PDF
            textColor = "#222222";
            var pageSizeCss = PageSize == "Letter" ? "Letter" : "A4";
            return $@"
@page {{ size: {pageSizeCss}; margin: {MarginTop}cm {MarginRight}cm {MarginBottom}cm {MarginLeft}cm; }}
body {{
  background: {bg};
  color: {textColor};
  font-family: {fontStack};
  font-size: {FontSize}pt;
  line-height: 1.6;
  margin: 0;
  padding: 0;
}}
.section {{
  width: 100%;
  margin: 0;
  padding: 0;
}}
h1,h2,h3,h4,h5,h6 {{
  color: {hColor};
  margin: 12pt 0 8pt 0;
  line-height: 1.25;
  page-break-after: avoid;
}}
h1 {{ font-size: {FontSize * 2.0:0.##}pt; }}
h2 {{ font-size: {FontSize * 1.6:0.##}pt; }}
h3 {{ font-size: {FontSize * 1.3:0.##}pt; }}
p, li {{
  margin: 6pt 0;
  orphans: 2;
  widows: 2;
}}
ul, ol {{ padding-left: 18pt; margin: 6pt 0; }}
hr {{ border: 0; height: 1px; background: {border}; margin: 12pt 0; }}
blockquote {{
  border-left: 4px solid {border};
  margin: 8pt 0;
  padding: 2pt 12pt;
  color: {(Theme=="Dark"?"#cccccc":"#555555")};
}}
code {{
  background: {codeBg};
  padding: 2pt 4pt;
  border-radius: 4pt;
  font-family: ""Courier New"", Courier, monospace;
  white-space: pre-wrap;
  word-break: break-word;
}}
pre {{
  background: {codeBg};
  padding: 8pt;
  border-radius: 4pt;
  white-space: pre-wrap;
  word-break: break-word;
  overflow: visible;
}}
table {{ border-collapse: collapse; width: 100%; margin: 8pt 0; page-break-inside: auto; }}
thead {{ display: table-header-group; }}
tr, td, th {{ page-break-inside: avoid; }}
th, td {{ border: 1px solid {border}; padding: 6pt; text-align: left; vertical-align: top; }}
img, svg {{ max-width: 100%; height: auto; page-break-inside: avoid; }}
";
        }
    }

    private static MarkdownPipeline BuildMarkdownPipeline()
    {
        var builder = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePipeTables()
            .UseGridTables()
            .UseEmphasisExtras()
            .UseAutoLinks()
            .UseTaskLists()
            .UseListExtras()
            .UseFootnotes()
            .UseSmartyPants()
            .UseGenericAttributes();
        return builder.Build();
    }

    private static string GetDefaultSampleMarkdown()
    {
        return "# Título H1\n\n## Título H2\n\n### Título H3\n\nTexto de ejemplo con **negritas**, _itálicas_ y `código inline`.\n\n---\n\n> Blockquote de ejemplo.\n\n- Lista 1\n- Lista 2\n  - Subitem\n\n1. Paso uno\n2. Paso dos\n\n```csharp\n// Código de ejemplo\nConsole.WriteLine(\"Hola mundo\");\n```\n\n| Columna A | Columna B |\n|---|---|\n| A1 | B1 |\n| A2 | B2 |\n";
    }
}
