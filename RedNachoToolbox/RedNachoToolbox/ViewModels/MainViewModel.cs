using System.Collections.ObjectModel;
using RedNachoToolbox.Models;

namespace RedNachoToolbox.ViewModels;

/// <summary>
/// Main ViewModel for the Red Nacho ToolBox application.
/// Manages the collection of tools and handles the main application logic.
/// </summary>
public class MainViewModel : BaseViewModel
{
    private ObservableCollection<ToolInfo> _tools;
    private ObservableCollection<ToolInfo> _filteredTools;
    private string _searchText = string.Empty;
    private ToolCategory? _selectedCategory;
    private ToolInfo? _selectedTool;

    /// <summary>
    /// Initializes a new instance of the MainViewModel class.
    /// </summary>
    public MainViewModel()
    {
        Title = "Red Nacho ToolBox";
        _tools = new ObservableCollection<ToolInfo>();
        _filteredTools = new ObservableCollection<ToolInfo>();
        
        // Initialize with sample data
        LoadSampleTools();
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
    /// Loads sample tools for demonstration purposes.
    /// In a real application, this would load from a service or database.
    /// </summary>
    private void LoadSampleTools()
    {
        var sampleTools = new List<ToolInfo>
        {
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
}
