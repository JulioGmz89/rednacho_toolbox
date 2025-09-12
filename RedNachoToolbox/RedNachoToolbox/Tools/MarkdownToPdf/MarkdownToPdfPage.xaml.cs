using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.ComponentModel;

namespace RedNachoToolbox.Tools.MarkdownToPdf;

public partial class MarkdownToPdfPage : ContentPage
{
    private MarkdownToPdfViewModel ViewModel => (MarkdownToPdfViewModel)BindingContext;

    public MarkdownToPdfPage()
    {
        InitializeComponent();
        BindingContext = new MarkdownToPdfViewModel();
        if (BindingContext is INotifyPropertyChanged inpc)
        {
            inpc.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnPreviewTabClicked(object sender, EventArgs e)
    {
        try
        {
            if (ViewModel.IsEditorMode)
            {
                ViewModel.IsEditorMode = false;
                // Rebuild to ensure latest styles/content
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
    
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(MarkdownToPdfViewModel.HtmlPreview))
            {
                // Always force a WebView reload when HTML changes and preview is visible
                if (!ViewModel.IsEditorMode && PreviewWebView != null)
                {
                    PreviewWebView.Source = new HtmlWebViewSource { Html = ViewModel.HtmlPreview };
                }
            }
            else if (e.PropertyName == nameof(MarkdownToPdfViewModel.IsEditorMode))
            {
                // If switched back to preview, ensure WebView shows latest HTML
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
            var pdfBytes = await ViewModel.GeneratePdfAsync();
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                await DisplayAlert("Error", "No se pudo generar el PDF.", "OK");
                return;
            }

            var fileName = $"Markdown_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllBytes(filePath, pdfBytes);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "PDF generado",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error generating PDF: {ex.Message}");
            await DisplayAlert("Error", $"Ocurrió un problema al generar el PDF.\n\n{ex}", "OK");
        }
    }
}
