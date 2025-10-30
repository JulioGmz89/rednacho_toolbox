namespace RedNachoToolbox.Services;

/// <summary>
/// Service for managing application preferences with type-safe access.
/// Provides an abstraction over the platform-specific preferences storage.
/// </summary>
public interface IPreferencesService
{
    /// <summary>
    /// Gets a preference value with a default fallback.
    /// </summary>
    /// <typeparam name="T">The type of the preference value</typeparam>
    /// <param name="key">The preference key</param>
    /// <param name="defaultValue">The default value if key doesn't exist</param>
    /// <returns>The preference value or default</returns>
    T Get<T>(string key, T defaultValue);

    /// <summary>
    /// Sets a preference value.
    /// </summary>
    /// <typeparam name="T">The type of the preference value</typeparam>
    /// <param name="key">The preference key</param>
    /// <param name="value">The value to store</param>
void Set<T>(string key, T value);

/// <summary>
    /// Removes a preference by key.
    /// </summary>
    /// <param name="key">The preference key to remove</param>
    void Remove(string key);

    /// <summary>
    /// Clears all preferences.
    /// </summary>
    void Clear();

    /// <summary>
    /// Checks if a preference key exists.
    /// </summary>
    /// <param name="key">The preference key to check</param>
    /// <returns>True if the key exists, false otherwise</returns>
    bool ContainsKey(string key);
}
