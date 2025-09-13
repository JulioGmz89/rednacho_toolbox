using RedNachoToolbox.ViewModels;
using RedNachoToolbox.Models;

namespace RedNachoToolbox.Views;

/// <summary>
/// Code-behind for ProductivityView - displays only Productivity tools in a grid layout
/// </summary>
public partial class ProductivityView : ContentView
{
    /// <summary>
    /// Gets the MainViewModel instance bound to this view
    /// </summary>
    public MainViewModel ViewModel => (MainViewModel)BindingContext;

    public ProductivityView()
    {
        InitializeComponent();
        
        // Ensure category filter is applied when BindingContext is set
        this.BindingContextChanged += (s, e) =>
        {
            if (BindingContext is MainViewModel vm)
            {
                vm.SelectedCategory = ToolCategory.Productivity;
            }
        };
    }

    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        if (sender is SearchBar searchBar)
        {
            System.Diagnostics.Debug.WriteLine($"Search executed for: {searchBar.Text}");
            searchBar.Unfocus();
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Search text changed: '{e.OldTextValue}' -> '{e.NewTextValue}'");
    }

    private async void OnToolSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is ToolInfo selectedTool)
            {
                System.Diagnostics.Debug.WriteLine($"Tool selected: {selectedTool.Name}");
                ViewModel?.AddToRecentlyUsed(selectedTool);
                try
                {
                    MessagingCenter.Send(this, "OpenTool", selectedTool);
                }
                catch (Exception msgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending OpenTool message: {msgEx.Message}");
                }

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
}
