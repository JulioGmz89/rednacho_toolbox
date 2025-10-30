using CommunityToolkit.Mvvm.ComponentModel;

namespace RedNachoToolbox.ViewModels;

/// <summary>
/// Base class for all ViewModels in the Red Nacho ToolBox application.
/// Inherits from ObservableObject to provide INotifyPropertyChanged implementation.
/// </summary>
public abstract class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private string _title = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is currently performing an operation.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
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
