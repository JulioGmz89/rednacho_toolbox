using System.Collections.ObjectModel;
using RedNachoToolbox.Models;
using RedNachoToolbox.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RedNachoToolbox.Messaging;
using RedNachoToolbox.Constants;

namespace RedNachoToolbox.ViewModels;

/// <summary>
/// Main ViewModel para la aplicación. Centraliza estado de navegación, filtros y herramientas.
/// Ahora se alimenta desde IToolRegistry y expone RelayCommands para reducir lógica en code-behind.
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IToolRegistry _toolRegistry;

    private ObservableCollection<ToolInfo> _tools = new();
    private ObservableCollection<ToolInfo> _filteredTools = new();
    private ObservableCollection<ToolInfo> _recentlyUsedTools = new();
    private string _searchText = string.Empty;
    private ToolCategory? _selectedCategory;
    private ToolInfo? _selectedTool;
    private bool _isSidebarCollapsed;
    private bool _isDarkTheme;
    private bool _isProductivityExpanded;
    private string _activePage = "Dashboard";

    public MainViewModel(IToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry ?? throw new ArgumentNullException(nameof(toolRegistry));
        Title = "Red Nacho ToolBox";
        LoadPreferences();
        InitializeToolsFromRegistry();
        LoadRecentlyUsedTools();
    }

    #region Colecciones
    public ObservableCollection<ToolInfo> Tools
    {
        get => _tools; private set => SetProperty(ref _tools, value);
    }
    public ObservableCollection<ToolInfo> FilteredTools
    {
        get => _filteredTools; private set => SetProperty(ref _filteredTools, value);
    }
    public ObservableCollection<ToolInfo> RecentlyUsedTools
    {
        get => _recentlyUsedTools; private set => SetProperty(ref _recentlyUsedTools, value);
    }
    #endregion

    #region Propiedades de filtro / estado
    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) { ApplyFilters(); OnPropertyChanged(nameof(HasActiveFilters)); } }
    }
    public ToolCategory? SelectedCategory
    {
        get => _selectedCategory;
        set { if (SetProperty(ref _selectedCategory, value)) { ApplyFilters(); OnPropertyChanged(nameof(HasActiveFilters)); } }
    }
    public ToolInfo? SelectedTool { get => _selectedTool; set => SetProperty(ref _selectedTool, value); }
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(SearchText) || SelectedCategory.HasValue;
    public bool HasRecentlyUsedTools => RecentlyUsedTools.Count > 0;

    public bool IsSidebarCollapsed
    {
        get => _isSidebarCollapsed;
        set { if (SetProperty(ref _isSidebarCollapsed, value)) OnPropertyChanged(nameof(LogoImageSource)); }
    }
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set { if (SetProperty(ref _isDarkTheme, value)) OnPropertyChanged(nameof(LogoImageSource)); }
    }
    public bool IsProductivityExpanded { get => _isProductivityExpanded; set => SetProperty(ref _isProductivityExpanded, value); }

    public string ActivePage { get => _activePage; set => SetProperty(ref _activePage, value); }
    public bool IsDashboardActive => ActivePage == "Dashboard";
    public bool IsProductivityActive => ActivePage == "Productivity";
    public bool IsSettingsActive => false; // Settings no participa en indicador activo

    public string LogoImageSource => IsSidebarCollapsed ? "rn_toolkit_collapsed.png" : (IsDarkTheme ? "rn_toolkit_expanded_dark.png" : "rn_toolkit_expanded_light.png");
    public string DashboardIconBase => "grid_outline";
    public string ProductivityIconBase => "rocket";
    public string SettingsIconBase => "options_outline";

    public IEnumerable<ToolInfo> ProductivityTools => Tools.Where(t => t.Category == ToolCategory.Productivity);
    #endregion

    #region Inicialización
    private void InitializeToolsFromRegistry()
    {
        Tools.Clear();
        foreach (var t in _toolRegistry.GetAll())
            Tools.Add(t);
        ApplyFilters();
    }
    private void LoadPreferences()
    {
         _isSidebarCollapsed = Preferences.Get(PreferenceKeys.IsSidebarCollapsed, false);
   _isDarkTheme = IsCurrentlyDarkTheme();
    }
    private void LoadRecentlyUsedTools() { /* Inicialmente vacío; futura persistencia */ }
    #endregion

    #region Tema / Sidebar Updates externos
    public void UpdateThemeState(bool isDarkTheme) => IsDarkTheme = isDarkTheme;
    public void UpdateSidebarState(bool isSidebarCollapsed) => IsSidebarCollapsed = isSidebarCollapsed;
    private bool IsCurrentlyDarkTheme()
    {
        try
        {
            var resources = Application.Current?.Resources; if (resources == null) return false;
            if (resources.TryGetValue("PageBackgroundColor", out var bgObj) && bgObj is Color c)
                return c.Red < 0.5f && c.Green < 0.5f && c.Blue < 0.5f;
        }
        catch { }
        return false;
    }
    #endregion

    #region Filtros y Colecciones
    private void ApplyFilters()
    {
        var filtered = Tools.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.ToLowerInvariant();
            filtered = filtered.Where(t => t.Name.ToLowerInvariant().Contains(s) || t.Description.ToLowerInvariant().Contains(s) || t.Category.ToString().ToLowerInvariant().Contains(s));
        }
        if (SelectedCategory.HasValue)
            filtered = filtered.Where(t => t.Category == SelectedCategory.Value);
        FilteredTools.Clear();
        foreach (var t in filtered) FilteredTools.Add(t);
        OnPropertyChanged(nameof(ToolsCount));
        OnPropertyChanged(nameof(FilteredToolsCount));
    }
    public int ToolsCount => Tools.Count;
    public int FilteredToolsCount => FilteredTools.Count;

    public void AddTool(ToolInfo tool)
    {
        if (tool == null) throw new ArgumentNullException(nameof(tool));
        Tools.Add(tool); ApplyFilters();
    }
    public bool RemoveTool(ToolInfo tool)
    {
        if (tool == null) return false;
        var removed = Tools.Remove(tool); if (removed) ApplyFilters(); return removed;
    }
    public void ClearFilters() { SearchText = string.Empty; SelectedCategory = null; }
    public void RefreshTools() => ApplyFilters();

    public void AddToRecentlyUsed(ToolInfo tool)
    {
        if (tool == null) return;
        var existing = RecentlyUsedTools.FirstOrDefault(t => t.Name == tool.Name);
        if (existing != null) RecentlyUsedTools.Remove(existing);
        RecentlyUsedTools.Insert(0, tool);
        while (RecentlyUsedTools.Count > 3) RecentlyUsedTools.RemoveAt(RecentlyUsedTools.Count - 1);
        OnPropertyChanged(nameof(HasRecentlyUsedTools));
    }

    private void ClearActiveProductivityTools()
    {
        foreach (var t in ProductivityTools)
            if (t.IsActive) t.IsActive = false;
    }

    private void SetActiveProductivityTool(ToolInfo tool)
    {
        foreach (var t in ProductivityTools)
            t.IsActive = ReferenceEquals(t, tool);
    }
    #endregion

    #region Active Page
    public void SetActivePage(string pageName)
    {
        ActivePage = pageName;
        OnPropertyChanged(nameof(IsDashboardActive));
        OnPropertyChanged(nameof(IsProductivityActive));
        OnPropertyChanged(nameof(IsSettingsActive));
    }
    #endregion

    #region RelayCommands
    [RelayCommand]
    private void GoDashboard()
    {
        SetActivePage("Dashboard");
        ClearFilters();
        ClearActiveProductivityTools();
    }

    [RelayCommand]
    private void GoProductivity()
    {
        SetActivePage("Productivity");
        SelectedCategory = ToolCategory.Productivity;
    }

    [RelayCommand]
    private void ToggleProductivityExpanded()
    {
        IsProductivityExpanded = !IsProductivityExpanded;
    }

    [RelayCommand]
    private void OpenTool(ToolInfo tool)
    {
        if (tool == null) return;
        AddToRecentlyUsed(tool);
        if (tool.Category == ToolCategory.Productivity)
        {
            SetActivePage("Productivity");
            SetActiveProductivityTool(tool);
        }
        WeakReferenceMessenger.Default.Send(new OpenToolMessage(tool));
    }
    #endregion
}
