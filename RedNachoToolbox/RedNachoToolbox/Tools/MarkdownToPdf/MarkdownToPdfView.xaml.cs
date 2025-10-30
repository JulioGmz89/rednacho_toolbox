using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using System.Threading;
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif

namespace RedNachoToolbox.Tools.MarkdownToPdf;

public partial class MarkdownToPdfView : ContentView
{
    private MarkdownToPdfViewModel ViewModel => (MarkdownToPdfViewModel)BindingContext;

    public MarkdownToPdfView()
    {
        InitializeComponent();
        BindingContext = new MarkdownToPdfViewModel();
        if (BindingContext is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += OnViewModelPropertyChanged;
        }

        // Initialize chevron rotation according to expander state after layout is ready
        this.Dispatcher.Dispatch(() =>
        {
            try
            {
                if (ColorExpander != null && ChevronImage != null)
                {
                    ChevronImage.Rotation = ColorExpander.IsExpanded ? 180 : 0;
                    ChevronImage.Opacity = 0.85;
                }
            }
            catch { }
        });
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(MarkdownToPdfViewModel.HtmlPreview))
            {
                if (!ViewModel.IsEditorMode && PreviewWebView != null)
                {
                    PreviewWebView.Source = new HtmlWebViewSource { Html = ViewModel.HtmlPreview };
                }
            }
            else if (e.PropertyName == nameof(MarkdownToPdfViewModel.IsEditorMode))
            {
                if (!ViewModel.IsEditorMode && PreviewWebView != null)
                {
                    PreviewWebView.Source = new HtmlWebViewSource { Html = ViewModel.HtmlPreview };
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating preview: {ex.Message}");
        }
    }

    private async void OnGeneratePdfClicked(object sender, EventArgs e)
    {
        try
        {
            void RestorePreview()
            {
                try
                {
                    if (PreviewWebView != null)
                    {
                        var previewHtml = ViewModel.HtmlPreview;
                        PreviewWebView.Source = new HtmlWebViewSource { Html = previewHtml };
                    }
                }
                catch (Exception rx)
                {
                    System.Diagnostics.Debug.WriteLine($"RestorePreview error: {rx.Message}");
                }
            }

            // 1) En Windows intentamos imprimir con WebView2 para que el PDF sea idéntico a la vista previa
            bool printedWithWebView2 = false;
#if WINDOWS
            try
            {
                var exportHtml = ViewModel.GetExportHtml();
                if (PreviewWebView?.Handler?.PlatformView is WebView2 native)
                {
                    var core = native.CoreWebView2;
                    if (core == null)
                    {
                        await native.EnsureCoreWebView2Async();
                        core = native.CoreWebView2;
                    }

                    if (core == null)
                    {
                        throw new InvalidOperationException("WebView2 core not initialized");
                    }

                    native.NavigateToString(exportHtml);
                    await WaitForDocumentCompleteAsync(native);

                    string tempPath = Path.Combine(FileSystem.CacheDirectory, $"Markdown_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                    var printSettings = core.Environment.CreatePrintSettings();
                    // Honor @page via default margins; background enabled to keep styles closer to preview
                    printSettings.ShouldPrintBackgrounds = true;
                    await core.PrintToPdfAsync(tempPath, printSettings);

                    // Guardar donde el usuario elija
                    await using var read = File.OpenRead(tempPath);
                    #pragma warning disable CA1416
                    var saveResult = await FileSaver.Default.SaveAsync(Path.GetFileName(tempPath), read, CancellationToken.None);
                    #pragma warning restore CA1416
                    if (!saveResult.IsSuccessful)
                    {
                        await Application.Current!.MainPage!.DisplayAlert("Error", saveResult.Exception?.Message ?? "No se pudo guardar el archivo.", "OK");
                        return;
                    }
                    // Abrir
                    if (!string.IsNullOrEmpty(saveResult.FilePath))
                    {
                        try
                        {
                            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(saveResult.FilePath) });
                        }
                        catch { /* noop */ }
                    }
                    printedWithWebView2 = true;
                    // Restaurar la vista previa (evita que se quede el HTML de exportación sin márgenes visuales)
                    RestorePreview();
                }
            }
            catch (Exception wv2ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fallo PrintToPdf con WebView2: {wv2ex.Message}");
                printedWithWebView2 = false;
            }
#endif

            if (!printedWithWebView2)
            {
                // 2) Fallback cross‑platform con HtmlRendererCore.PdfSharp
                var pdfBytes = await ViewModel.GeneratePdfAsync();
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo generar el PDF.", "OK");
                    return;
                }

                var fileName = $"Markdown_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                using var stream = new MemoryStream(pdfBytes);
                #pragma warning disable CA1416
                var saveResult = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);
                #pragma warning restore CA1416

                if (!saveResult.IsSuccessful)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", saveResult.Exception?.Message ?? "No se pudo guardar el archivo.", "OK");
                    // Aunque falle el guardado, restaurar la vista previa por si cambió
                    RestorePreview();
                    return;
                }

                try
                {
                    if (!string.IsNullOrEmpty(saveResult.FilePath))
                    {
                        await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(saveResult.FilePath) });
                    }
                }
                catch (Exception openEx)
                {
                    System.Diagnostics.Debug.WriteLine($"No se pudo abrir el PDF: {openEx.Message}");
                }

                // Asegurar que la vista previa regrese a su HTML con márgenes visuales
                RestorePreview();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error generating PDF: {ex.Message}");
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Ocurrió un problema al generar el PDF.\n\n{ex}", "OK");
        }
    }

    private void OnPreviewTabClicked(object sender, EventArgs e)
    {
        try
        {
            if (ViewModel.IsEditorMode)
            {
                ViewModel.IsEditorMode = false;
                ViewModel.RebuildPreviewHtml();
                if (PreviewWebView != null)
                {
                    PreviewWebView.Source = new HtmlWebViewSource { Html = ViewModel.HtmlPreview };
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error switching to preview: {ex.Message}");
        }
    }

    private void OnEditorTabClicked(object sender, EventArgs e)
    {
        try
        {
            if (!ViewModel.IsEditorMode)
            {
                ViewModel.IsEditorMode = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error switching to editor: {ex.Message}");
        }
    }

    private async void OnColorExpanderExpandedChanged(object sender, EventArgs e)
    {
        try
        {
            if (ChevronImage != null && ColorExpander != null)
            {
                bool expanded = ColorExpander.IsExpanded;
                double targetRotation = expanded ? 180 : 0;
                double targetOpacity = expanded ? 1.0 : 0.85;
                await Task.WhenAll(
                    ChevronImage.RotateTo(targetRotation, 160, Easing.CubicOut),
                    ChevronImage.FadeTo(targetOpacity, 120, Easing.CubicOut)
                );
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error animating expanded changed: {ex.Message}");
        }
    }

#if WINDOWS
    private static async Task WaitForDocumentCompleteAsync(Microsoft.UI.Xaml.Controls.WebView2 native)
    {
        // Poll document.readyState until complete or timeout
        for (int i = 0; i < 120; i++)
        {
            try
            {
                var state = await native.CoreWebView2.ExecuteScriptAsync("document.readyState");
                if (!string.IsNullOrEmpty(state) && state.Contains("complete", StringComparison.OrdinalIgnoreCase))
                    break;
            }
            catch { }
            await Task.Delay(50);
        }
    }
#endif
}
