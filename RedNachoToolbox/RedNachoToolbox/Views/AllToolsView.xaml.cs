using RedNachoToolbox.ViewModels;
using RedNachoToolbox.Models;

namespace RedNachoToolbox.Views;

/// <summary>
/// Code-behind for AllToolsView - displays all available tools in a grid layout
/// </summary>
public partial class AllToolsView : ContentView
{
    /// <summary>
    /// Gets the MainViewModel instance bound to this view
    /// </summary>
    public MainViewModel ViewModel => (MainViewModel)BindingContext;

    /// <summary>
    /// Initializes a new instance of the AllToolsView class
    /// </summary>
    public AllToolsView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the search button pressed event
    /// </summary>
    /// <param name="sender">The SearchBar that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        if (sender is SearchBar searchBar)
        {
            // The search is automatically handled by the SearchText binding in MainViewModel
            // This event can be used for additional search logic if needed
            System.Diagnostics.Debug.WriteLine($"Search executed for: {searchBar.Text}");
            
            // Optional: Unfocus the search bar after search
            searchBar.Unfocus();
        }
    }

    /// <summary>
    /// Handles the search text changed event for real-time filtering
    /// </summary>
    /// <param name="sender">The SearchBar that triggered the event</param>
    /// <param name="e">Text changed event arguments</param>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        // Real-time search is handled automatically by the SearchText binding
        // This event can be used for additional logic like analytics or debouncing
        System.Diagnostics.Debug.WriteLine($"Search text changed: '{e.OldTextValue}' -> '{e.NewTextValue}'");
    }

    /// <summary>
    /// Handles the clear filters button click
    /// </summary>
    /// <param name="sender">The Button that was clicked</param>
    /// <param name="e">Event arguments</param>
    private void OnClearFiltersClicked(object sender, EventArgs e)
    {
        try
        {
            ViewModel?.ClearFilters();
            System.Diagnostics.Debug.WriteLine("Filters cleared successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing filters: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles tool selection changes in the CollectionView
    /// </summary>
    /// <param name="sender">The CollectionView that triggered the event</param>
    /// <param name="e">Selection changed event arguments</param>
    private async void OnToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is ToolInfo selectedTool)
            {
                System.Diagnostics.Debug.WriteLine($"Tool selected: {selectedTool.Name}");
                // Add to recently used tools if ViewModel is available
                ViewModel?.AddToRecentlyUsed(selectedTool);

                // Publish message so MainPage hosts the tool within the content area (keeping sidebar)
                try
                {
                    MessagingCenter.Send(this, "OpenTool", selectedTool);
                }
                catch (Exception msgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending OpenTool message: {msgEx.Message}");
                }
                
                // Clear selection to allow selecting the same item again
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling tool selection: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the add tool button click (future feature)
    /// </summary>
    /// <param name="sender">The Button that was clicked</param>
    /// <param name="e">Event arguments</param>
    private void OnAddToolClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Add tool button clicked");
            
            // TODO: Implement add tool functionality in future iteration
            DisplayAlert("Add Tool", "Add tool functionality will be implemented in a future version.", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling add tool click: {ex.Message}");
        }
    }

    /// <summary>
    /// Helper method to display alerts
    /// </summary>
    /// <param name="title">Alert title</param>
    /// <param name="message">Alert message</param>
    /// <param name="cancel">Cancel button text</param>
    private async void DisplayAlert(string title, string message, string cancel)
    {
        try
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error displaying alert: {ex.Message}");
        }
    }
}
