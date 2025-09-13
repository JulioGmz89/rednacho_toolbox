using Microsoft.Maui.Controls;
using RedNachoToolbox.ViewModels;
using RedNachoToolbox.Models;
using CommunityToolkit.Mvvm.Messaging;
using RedNachoToolbox.Messaging;

namespace RedNachoToolbox.Views;

/// <summary>
/// Dashboard view that displays recently used tools and all available applications.
/// </summary>
public partial class DashboardView : ContentView
{
    /// <summary>
    /// Gets the MainViewModel from the binding context.
    /// </summary>
    private MainViewModel? ViewModel => BindingContext as MainViewModel;

    /// <summary>
    /// Initializes a new instance of the DashboardView class.
    /// </summary>
    public DashboardView()
    {
        InitializeComponent();
        // Subscribe to theme change notifications to refresh templates
        try
        {
            WeakReferenceMessenger.Default.Register<ThemeChangedMessage>(this, (recipient, message) =>
            {
                RefreshTemplates();
            });
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error subscribing to ThemeChanged: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the search button pressed event.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event arguments</param>
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        try
        {
            if (sender is SearchBar searchBar && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Search executed for: '{searchBar.Text}'");
                
                // The search is automatically handled by the SearchText binding in MainViewModel
                // Additional search logic can be added here if needed
                
                // Removed debug popup feedback for search to avoid intrusive alerts
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSearchButtonPressed: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the search text changed event for real-time filtering.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The text changed event arguments</param>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (sender is SearchBar searchBar && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Search text changed to: '{e.NewTextValue}'");
                
                // The filtering is automatically handled by the SearchText property binding
                // in the MainViewModel through the ApplyFilters method
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSearchTextChanged: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the clear filters button click event.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event arguments</param>
    private void OnClearFiltersClicked(object sender, EventArgs e)
    {
        try
        {
            if (ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine("Clearing all filters");
                
                ViewModel.ClearFilters();
                
                // Removed debug popup feedback for clearing filters
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnClearFiltersClicked: {ex.Message}");
            // Removed error popup; keeping only debug log
        }
    }

    /// <summary>
    /// Handles the tool selection changed event for the main tools collection.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The selection changed event arguments</param>
    private void OnToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is ToolInfo selectedTool && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Tool selected: {selectedTool.Name}");
                
                // Add to recently used tools
                ViewModel.AddToRecentlyUsed(selectedTool);

                // Publish message to host the tool inside MainPage content area (keep sidebar)
                try
                {
                    WeakReferenceMessenger.Default.Send(new OpenToolMessage(selectedTool));
                }
                catch (Exception msgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending OpenTool message: {msgEx.Message}");
                }

                // Clear selection to allow reselection
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
                
                // Navigation handled by MainPage via MessagingCenter
            }
        }
        catch (Exception ex)
        {
            // Log the exception to avoid unused variable warning and aid debugging
            System.Diagnostics.Debug.WriteLine($"Error in OnToolSelectionChanged: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the recently used tool selection changed event.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The selection changed event arguments</param>
    private void OnRecentToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is ToolInfo selectedTool && ViewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Recent tool selected: {selectedTool.Name}");
                
                // Update the recently used order (move to top)
                ViewModel.AddToRecentlyUsed(selectedTool);
                
                // Clear selection to allow reselection
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
                
                // Removed debug popup for recent tool selection
                
                // TODO: Navigate to the specific tool page
                // await Shell.Current.GoToAsync($"tool?name={selectedTool.Name}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnRecentToolSelectionChanged: {ex.Message}");
            // Removed error popup; logging only
        }
    }

    /// <summary>
    /// Handles the add tool button click event (future feature).
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">The event arguments</param>
    private void OnAddToolClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Add tool button clicked");
            
            // Future feature: Open add tool dialog or page (popup removed)
            
            // TODO: Implement add tool functionality
            // await Shell.Current.GoToAsync("addtool");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnAddToolClicked: {ex.Message}");
        }
    }

    // Removed DisplayAlert helper; debug popups are not used in release

    /// <summary>
    /// Force-refreshes the ItemTemplate of the collection views so any DynamicResource
    /// based colors inside the DataTemplate re-evaluate after theme changes.
    /// </summary>
    private void RefreshTemplates()
    {
        try
        {
            var template = Application.Current?.Resources?["ToolCardTemplate"] as DataTemplate;

            if (template != null)
            {
                if (AllToolsCollectionView != null)
                {
                    var old = AllToolsCollectionView.ItemTemplate;
                    AllToolsCollectionView.ItemTemplate = null;
                    AllToolsCollectionView.ItemTemplate = template;
                }

                if (RecentToolsCollectionView != null)
                {
                    var old = RecentToolsCollectionView.ItemTemplate;
                    RecentToolsCollectionView.ItemTemplate = null;
                    RecentToolsCollectionView.ItemTemplate = template;
                }
            }
            // Invalidate measure to ensure visual update
            this.InvalidateMeasure();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing templates: {ex.Message}");
        }
    }
}
