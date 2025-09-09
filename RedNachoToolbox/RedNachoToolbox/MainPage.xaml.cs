namespace RedNachoToolbox;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    #region Search Event Handlers
    
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        if (sender is SearchBar searchBar)
        {
            var searchText = searchBar.Text?.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                // TODO: Implement search functionality
                // This will be connected to the ViewModel in future iterations
                DisplayAlert("Search", $"Searching for: {searchText}", "OK");
            }
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.Trim();
        // TODO: Implement real-time search filtering
        // This will be connected to the ViewModel in future iterations
        System.Diagnostics.Debug.WriteLine($"Search text changed: {searchText}");
    }

    #endregion

    #region Navigation Event Handlers

    private void OnAllApplicationsClicked(object sender, EventArgs e)
    {
        // TODO: Navigate to All Applications view
        // This will be connected to the ViewModel and navigation service in future iterations
        DisplayAlert("Navigation", "All Applications selected", "OK");
        
        // Visual feedback - highlight the selected button
        ResetButtonStyles();
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#E3F2FD"); // Light blue highlight
        }
    }

    private void OnDocumentationClicked(object sender, EventArgs e)
    {
        // TODO: Navigate to Documentation view
        // This will be connected to the ViewModel and navigation service in future iterations
        DisplayAlert("Navigation", "Documentation selected", "OK");
        
        // Visual feedback - highlight the selected button
        ResetButtonStyles();
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#E3F2FD"); // Light blue highlight
        }
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        try
        {
            // Navigate to SettingsPage
            var settingsPage = new SettingsPage();
            await Navigation.PushAsync(settingsPage);
        }
        catch (Exception ex)
        {
            // Handle navigation error gracefully
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            await DisplayAlert("Navigation Error", "Unable to open Settings page. Please try again.", "OK");
        }
        
        // Visual feedback - highlight the selected button
        ResetButtonStyles();
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#E3F2FD"); // Light blue highlight
        }
    }

    #endregion

    #region Helper Methods

    private void ResetButtonStyles()
    {
        // Reset all navigation buttons to default style
        var defaultLightColor = Color.FromArgb("#FFFFFF");
        var defaultDarkColor = Color.FromArgb("#2A2A2A");
        
        AllApplicationsButton.BackgroundColor = Application.Current?.RequestedTheme == AppTheme.Dark 
            ? defaultDarkColor 
            : defaultLightColor;
            
        DocumentationButton.BackgroundColor = Application.Current?.RequestedTheme == AppTheme.Dark 
            ? defaultDarkColor 
            : defaultLightColor;
            
        SettingsButton.BackgroundColor = Application.Current?.RequestedTheme == AppTheme.Dark 
            ? defaultDarkColor 
            : defaultLightColor;
    }

    #endregion
}
