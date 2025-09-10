using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RedNachoToolbox.Models;

/// <summary>
/// Represents information about a tool in the Red Nacho ToolBox application.
/// Implements INotifyPropertyChanged for MAUI data binding support.
/// </summary>
public class ToolInfo : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _iconPath = string.Empty;
    private ToolCategory _category = ToolCategory.Utilities;
    private Type? _targetType;

    /// <summary>
    /// Gets or sets the display name of the tool.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the description of the tool's functionality.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Gets or sets the path to the tool's icon image.
    /// </summary>
    public string IconPath
    {
        get => _iconPath;
        set => SetProperty(ref _iconPath, value);
    }

    /// <summary>
    /// Gets or sets the category this tool belongs to.
    /// </summary>
    public ToolCategory Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    /// <summary>
    /// Gets or sets the target page type for navigation to this tool.
    /// </summary>
    public Type? TargetType
    {
        get => _targetType;
        set => SetProperty(ref _targetType, value);
    }

    /// <summary>
    /// Initializes a new instance of the ToolInfo class.
    /// </summary>
    public ToolInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ToolInfo class with specified values.
    /// </summary>
    /// <param name="name">The tool name</param>
    /// <param name="description">The tool description</param>
    /// <param name="iconPath">The path to the tool icon</param>
    /// <param name="category">The tool category</param>
    /// <param name="targetType">The target page type for navigation</param>
    public ToolInfo(string name, string description, string iconPath, ToolCategory category, Type? targetType = null)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _description = description ?? throw new ArgumentNullException(nameof(description));
        _iconPath = iconPath ?? throw new ArgumentNullException(nameof(iconPath));
        _category = category;
        _targetType = targetType;
    }

    #region INotifyPropertyChanged Implementation

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property value and raises PropertyChanged if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property</typeparam>
    /// <param name="field">Reference to the backing field</param>
    /// <param name="value">The new value</param>
    /// <param name="propertyName">The name of the property</param>
    /// <returns>True if the property was changed, false otherwise</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region Equality and ToString

    /// <summary>
    /// Determines whether the specified object is equal to the current ToolInfo.
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not ToolInfo other)
            return false;

        return Name == other.Name && 
               Description == other.Description && 
               IconPath == other.IconPath && 
               Category == other.Category;
    }

    /// <summary>
    /// Returns a hash code for this ToolInfo.
    /// </summary>
    /// <returns>A hash code for the current object</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description, IconPath, Category);
    }

    /// <summary>
    /// Returns a string representation of this ToolInfo.
    /// </summary>
    /// <returns>A string containing the tool name and category</returns>
    public override string ToString()
    {
        return $"{Name} ({Category})";
    }

    #endregion
}
