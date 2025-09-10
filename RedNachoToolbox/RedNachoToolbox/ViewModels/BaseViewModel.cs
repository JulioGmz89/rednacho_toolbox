using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RedNachoToolbox.ViewModels;

/// <summary>
/// Base class for all ViewModels in the Red Nacho ToolBox application.
/// Provides common functionality like INotifyPropertyChanged implementation and common properties.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is currently performing an operation.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// Gets or sets the title for the current view or operation.
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// Gets a value indicating whether the ViewModel is not busy.
    /// Useful for binding to UI elements that should be enabled when not busy.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

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
    /// Also raises PropertyChanged for IsNotBusy when IsBusy changes.
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

        // Special handling for IsBusy to also notify IsNotBusy
        if (propertyName == nameof(IsBusy))
        {
            OnPropertyChanged(nameof(IsNotBusy));
        }

        return true;
    }

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Called when the ViewModel is appearing/loading.
    /// Override this method to perform initialization logic.
    /// </summary>
    public virtual Task OnAppearingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the ViewModel is disappearing/unloading.
    /// Override this method to perform cleanup logic.
    /// </summary>
    public virtual Task OnDisappearingAsync()
    {
        return Task.CompletedTask;
    }

    #endregion
}
