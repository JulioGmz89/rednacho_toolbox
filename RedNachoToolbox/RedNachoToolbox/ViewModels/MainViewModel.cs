using System.Collections.ObjectModel;
using System.ComponentModel;
using RedNachoToolbox.Models;
using RedNachoToolbox.Tools.MarkdownToPdf;

namespace RedNachoToolbox.ViewModels;

/// <summary>
/// Main ViewModel for the Red Nacho ToolBox application.
/// Manages the collection of tools and handles the main application logic.
/// </summary>
public class MainViewModel : BaseViewModel
{
    private ObservableCollection<ToolInfo> _tools;
    private ObservableCollection<ToolInfo> _filteredTools;
    private ObservableCollection<ToolInfo> _recentlyUsedTools;
    private string _searchText = string.Empty;
    private ToolCategory? _selectedCategory;
    private ToolInfo? _selectedTool;
    private bool _isSidebarCollapsed;
    private bool _isDarkTheme;
    private string _activePage = "Dashboard"; // Default to Dashboard as active

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public MainViewModel()
    {
        Title = "Red Nacho ToolBox";
        _tools = new ObservableCollection<ToolInfo>();
        _filteredTools = new ObservableCollection<ToolInfo>();
        _recentlyUsedTools = new ObservableCollection<ToolInfo>();
        
        // Initialize sidebar and theme state from preferences
        LoadPreferences();
        
        // Initialize with sample data
        LoadSampleTools();
        
        // Initialize recently used tools with sample data
        LoadRecentlyUsedTools();
    }

    /// <summary>
    /// Gets the collection of all available tools.
    /// </summary>
    public ObservableCollection<ToolInfo> Tools
    {
        get => _tools;
        private set => SetProperty(ref _tools, value);
    }

    /// <summary>
    /// Gets the filtered collection of tools based on search and category filters.
    /// </summary>
    public ObservableCollection<ToolInfo> FilteredTools
    {
        get => _filteredTools;
        private set => SetProperty(ref _filteredTools, value);
    }

    /// <summary>
    /// Gets the collection of recently used tools (maximum 3 items).
    /// </summary>
    public ObservableCollection<ToolInfo> RecentlyUsedTools
    {
        get => _recentlyUsedTools;
        private set => SetProperty(ref _recentlyUsedTools, value);
    }

    /// <summary>
    /// Gets or sets the current search text for filtering tools.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilters();
                OnPropertyChanged(nameof(HasActiveFilters));
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected category for filtering tools.
    /// </summary>
    public ToolCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                ApplyFilters();
                OnPropertyChanged(nameof(HasActiveFilters));
            }
        }
    }

    /// <summary>
    /// Gets the total count of tools.
    /// </summary>
    public int ToolsCount => Tools.Count;

    /// <summary>
    /// Gets the count of filtered tools.
    /// </summary>
    public int FilteredToolsCount => FilteredTools.Count;

    /// <summary>
    /// Gets or sets the currently selected tool.
    /// </summary>
    public ToolInfo? SelectedTool
    {
        get => _selectedTool;
        set => SetProperty(ref _selectedTool, value);
    }

    /// <summary>
    /// Gets a value indicating whether there are active filters applied.
    /// </summary>
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(SearchText) || SelectedCategory.HasValue;

    /// <summary>
    /// Gets a value indicating whether there are recently used tools to display.
    /// </summary>
    public bool HasRecentlyUsedTools => RecentlyUsedTools.Count > 0;

    /// <summary>
    /// Gets or sets whether the sidebar is collapsed.
    /// </summary>
    public bool IsSidebarCollapsed
    {
        get => _isSidebarCollapsed;
        set
        {
            if (SetProperty(ref _isSidebarCollapsed, value))
            {
                OnPropertyChanged(nameof(LogoImageSource));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether dark theme is currently active.
    /// </summary>
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                OnPropertyChanged(nameof(LogoImageSource));
            }
        }
    }

    /// <summary>
    /// Gets the appropriate logo image source based on sidebar state and theme.
    /// </summary>
    public string LogoImageSource
    {
        get
        {
            if (IsSidebarCollapsed)
            {
                return "rn_toolkit_collapsed.png";
            }
            else
            {
                return IsDarkTheme ? "rn_toolkit_expanded_dark.png" : "rn_toolkit_expanded_light.png";
            }
        }
    }

    /// <summary>
    /// Gets the base name for the dashboard icon (grid_outline).
    /// </summary>
    public string DashboardIconBase => "grid_outline";

    /// <summary>
    /// Gets the base name for the documentation icon (document_outline).
    /// </summary>
    public string DocumentationIconBase => "document_outline";

    /// <summary>
    /// Gets the base name for the settings icon (options_outline).
    /// </summary>
    public string SettingsIconBase => "options_outline";

    /// <summary>
    /// Gets or sets the currently active page.
    /// </summary>
    public string ActivePage
    {
        get => _activePage;
        set => SetProperty(ref _activePage, value);
    }

    /// <summary>
    /// Gets whether the Dashboard page is currently active.
    /// </summary>
    public bool IsDashboardActive => ActivePage == "Dashboard";

    /// <summary>
    /// Gets whether the Documentation page is currently active.
    /// </summary>
    public bool IsDocumentationActive => ActivePage == "Documentation";

    /// <summary>
    /// Gets whether the Settings page is currently active.
    /// Note: Settings is excluded from active page indicator system since it navigates to a separate view.
    /// </summary>
    public bool IsSettingsActive => false; // Settings doesn't show active indicator

    /// <summary>
    /// Sets the active page and notifies all active state properties.
    /// </summary>
    /// <param name="pageName">The name of the page to set as active</param>
    public void SetActivePage(string pageName)
    {
        ActivePage = pageName;
        
        // Notify all active state properties
        OnPropertyChanged(nameof(IsDashboardActive));
        OnPropertyChanged(nameof(IsDocumentationActive));
        OnPropertyChanged(nameof(IsSettingsActive));
        
        System.Diagnostics.Debug.WriteLine($"Active page changed to: {pageName}");
    }

    /// <summary>
    /// Loads user preferences for sidebar and theme state.
    /// </summary>
    private void LoadPreferences()
    {
        // Load sidebar preference
        _isSidebarCollapsed = Preferences.Get("IsSidebarCollapsed", false);
        
        // Load theme preference by checking current application resources
        _isDarkTheme = IsCurrentlyDarkTheme();
        
        System.Diagnostics.Debug.WriteLine($"MainViewModel loaded preferences - Sidebar: {(_isSidebarCollapsed ? "Collapsed" : "Expanded")}, Theme: {(_isDarkTheme ? "Dark" : "Light")}");
    }

    /// <summary>
    /// Determines if the current theme is dark by checking applied color values.
    /// </summary>
    private bool IsCurrentlyDarkTheme()
    {
        try
        {
            var resources = Application.Current?.Resources;
            if (resources == null) return false;

            // Check the current PageBackgroundColor to determine theme
            if (resources.TryGetValue("PageBackgroundColor", out var bgColorObj) && bgColorObj is Color bgColor)
            {
                // Dark theme background is very dark, light theme background is white
                return bgColor.Red < 0.5f && bgColor.Green < 0.5f && bgColor.Blue < 0.5f;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error detecting current theme in MainViewModel: {ex.Message}");
        }
        
        return false; // Default to light theme
    }

    /// <summary>
    /// Updates the theme state (called from external sources like SettingsPage).
    /// </summary>
    public void UpdateThemeState(bool isDarkTheme)
    {
        IsDarkTheme = isDarkTheme;
    }

    /// <summary>
    /// Updates the sidebar state (called from external sources like SettingsPage).
    /// </summary>
    public void UpdateSidebarState(bool isSidebarCollapsed)
    {
        IsSidebarCollapsed = isSidebarCollapsed;
    }

    /// <summary>
    /// Loads sample tools for demonstration purposes.
    /// In a real application, this would load from a service or database.
    /// </summary>
    private void LoadSampleTools()
    {
        var sampleTools = new List<ToolInfo>
        {
            new ToolInfo(
                "Markdown → PDF",
                "Convierte Markdown a PDF con estilos personalizables y vista previa.",
                "document_outline_black.png",
                ToolCategory.Productivity,
                typeof(MarkdownToPdfView)
            ),

            new ToolInfo(
                "Calculator", 
                "A powerful calculator with advanced mathematical functions and unit conversions.", 
                "calculator_icon.png", 
                ToolCategory.Utilities),
            
            new ToolInfo(
                "JSON Formatter", 
                "Format, validate, and beautify JSON data with syntax highlighting.", 
                "json_icon.png", 
                ToolCategory.Development),
            
            new ToolInfo(
                "Base64 Encoder/Decoder", 
                "Encode and decode Base64 strings with support for files and text.", 
                "base64_icon.png", 
                ToolCategory.Development),
            
            new ToolInfo(
                "Hash Generator", 
                "Generate MD5, SHA1, SHA256, and other hash values for text and files.", 
                "hash_icon.png", 
                ToolCategory.Security),
            
            new ToolInfo(
                "Color Picker", 
                "Pick colors from screen, convert between formats (HEX, RGB, HSL).", 
                "color_icon.png", 
                ToolCategory.Utilities),
            
            new ToolInfo(
                "QR Code Generator", 
                "Generate QR codes for text, URLs, WiFi credentials, and more.", 
                "qr_icon.png", 
                ToolCategory.Utilities),
            
            new ToolInfo(
                "System Information", 
                "Display detailed system information including hardware and software details.", 
                "system_icon.png", 
                ToolCategory.System),
            
            new ToolInfo(
                "Network Scanner", 
                "Scan local network for devices, open ports, and network information.", 
                "network_icon.png", 
                ToolCategory.Network),
            
            new ToolInfo(
                "File Hasher", 
                "Calculate file hashes and verify file integrity with multiple algorithms.", 
                "file_hash_icon.png", 
                ToolCategory.FileManagement),
            
            new ToolInfo(
                "Password Generator", 
                "Generate secure passwords with customizable length and character sets.", 
                "password_icon.png", 
                ToolCategory.Security),
            
            new ToolInfo(
                "Text Diff Viewer", 
                "Compare two text files or strings and highlight differences.", 
                "diff_icon.png", 
                ToolCategory.Development),
            
            new ToolInfo(
                "Image Converter", 
                "Convert images between different formats (PNG, JPG, WebP, etc.).", 
                "image_icon.png", 
                ToolCategory.Media)
        };

        // Add tools to the collection
        foreach (var tool in sampleTools)
        {
            Tools.Add(tool);
        }

        // Initially show all tools
        ApplyFilters();
    }

    /// <summary>
    /// Applies current search and category filters to the tools collection.
    /// </summary>
    private void ApplyFilters()
    {
        var filtered = Tools.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(t => 
                t.Name.ToLowerInvariant().Contains(searchLower) ||
                t.Description.ToLowerInvariant().Contains(searchLower) ||
                t.Category.ToString().ToLowerInvariant().Contains(searchLower));
        }

        // Apply category filter
        if (SelectedCategory.HasValue)
        {
            filtered = filtered.Where(t => t.Category == SelectedCategory.Value);
        }

        // Update filtered collection
        FilteredTools.Clear();
        foreach (var tool in filtered)
        {
            FilteredTools.Add(tool);
        }

        // Notify count changes
        OnPropertyChanged(nameof(ToolsCount));
        OnPropertyChanged(nameof(FilteredToolsCount));
    }

    /// <summary>
    /// Adds a new tool to the collection.
    /// </summary>
    /// <param name="tool">The tool to add</param>
    public void AddTool(ToolInfo tool)
    {
        if (tool == null)
            throw new ArgumentNullException(nameof(tool));

        Tools.Add(tool);
        ApplyFilters();
    }

    /// <summary>
    /// Removes a tool from the collection.
    /// </summary>
    /// <param name="tool">The tool to remove</param>
    /// <returns>True if the tool was removed, false otherwise</returns>
    public bool RemoveTool(ToolInfo tool)
    {
        if (tool == null)
            return false;

        var removed = Tools.Remove(tool);
        if (removed)
        {
            ApplyFilters();
        }
        return removed;
    }

    /// <summary>
    /// Clears the search text and category filter.
    /// </summary>
    public void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedCategory = null;
    }

    /// <summary>
    /// Refreshes the tools collection by reapplying filters.
    /// </summary>
    public void RefreshTools()
    {
        ApplyFilters();
    }

    /// <summary>
    /// Adds a tool to the recently used tools collection.
    /// Maintains a maximum of 3 items, with the most recent at the top.
    /// </summary>
    /// <param name="tool">The tool to add to recently used</param>
    public void AddToRecentlyUsed(ToolInfo tool)
    {
        if (tool == null) return;

        // Remove the tool if it already exists in the list
        var existingTool = RecentlyUsedTools.FirstOrDefault(t => t.Name == tool.Name);
        if (existingTool != null)
        {
            RecentlyUsedTools.Remove(existingTool);
        }

        // Add the tool to the beginning of the list
        RecentlyUsedTools.Insert(0, tool);

        // Keep only the 3 most recent tools
        while (RecentlyUsedTools.Count > 3)
        {
            RecentlyUsedTools.RemoveAt(RecentlyUsedTools.Count - 1);
        }

        // Notify property change for HasRecentlyUsedTools
        OnPropertyChanged(nameof(HasRecentlyUsedTools));

        System.Diagnostics.Debug.WriteLine($"Added '{tool.Name}' to recently used tools. Total: {RecentlyUsedTools.Count}");
    }

    /// <summary>
    /// Loads sample recently used tools for demonstration purposes.
    /// In a real application, this would load from preferences or local storage.
    /// </summary>
    private void LoadRecentlyUsedTools()
    {
        // Add some sample recently used tools (last 3 tools from the sample data)
        var sampleRecentTools = Tools.TakeLast(3).Reverse().ToList();
        
        foreach (var tool in sampleRecentTools)
        {
            RecentlyUsedTools.Add(tool);
        }

        System.Diagnostics.Debug.WriteLine($"Loaded {RecentlyUsedTools.Count} recently used tools");
    }
}
