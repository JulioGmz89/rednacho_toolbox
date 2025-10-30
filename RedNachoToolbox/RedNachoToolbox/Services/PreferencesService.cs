using Microsoft.Extensions.Logging;

namespace RedNachoToolbox.Services;

/// <summary>
/// Default implementation of IPreferencesService using Microsoft.Maui.Storage.Preferences.
/// Provides type-safe access to platform-specific preferences storage with logging.
/// </summary>
public class PreferencesService : IPreferencesService
{
    private readonly ILogger<PreferencesService> _logger;

    public PreferencesService(ILogger<PreferencesService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _logger.LogDebug("PreferencesService initialized");
    }

    /// <inheritdoc/>
    public T Get<T>(string key, T defaultValue)
  {
  try
   {
     if (string.IsNullOrWhiteSpace(key))
    {
      _logger.LogWarning("Attempted to get preference with null or empty key");
           return defaultValue;
    }

     var value = typeof(T) switch
            {
       Type t when t == typeof(string) => (T)(object)Preferences.Get(key, (string)(object)defaultValue!),
         Type t when t == typeof(int) => (T)(object)Preferences.Get(key, (int)(object)defaultValue!),
        Type t when t == typeof(bool) => (T)(object)Preferences.Get(key, (bool)(object)defaultValue!),
  Type t when t == typeof(long) => (T)(object)Preferences.Get(key, (long)(object)defaultValue!),
   Type t when t == typeof(double) => (T)(object)Preferences.Get(key, (double)(object)defaultValue!),
    Type t when t == typeof(float) => (T)(object)Preferences.Get(key, (float)(object)defaultValue!),
        Type t when t == typeof(DateTime) => (T)(object)Preferences.Get(key, (DateTime)(object)defaultValue!),
   _ => throw new NotSupportedException($"Type {typeof(T).Name} is not supported")
      };

     _logger.LogTrace("Retrieved preference {Key} with value of type {Type}", key, typeof(T).Name);
return value;
        }
        catch (Exception ex)
     {
   _logger.LogError(ex, "Error getting preference {Key}, returning default value", key);
            return defaultValue;
        }
    }

    /// <inheritdoc/>
    public void Set<T>(string key, T value)
    {
  try
        {
         if (string.IsNullOrWhiteSpace(key))
   {
          _logger.LogWarning("Attempted to set preference with null or empty key");
         return;
   }

          switch (value)
            {
      case string stringValue:
  Preferences.Set(key, stringValue);
     break;
                case int intValue:
    Preferences.Set(key, intValue);
       break;
     case bool boolValue:
     Preferences.Set(key, boolValue);
                 break;
    case long longValue:
                Preferences.Set(key, longValue);
 break;
      case double doubleValue:
         Preferences.Set(key, doubleValue);
         break;
       case float floatValue:
       Preferences.Set(key, floatValue);
   break;
      case DateTime dateTimeValue:
   Preferences.Set(key, dateTimeValue);
 break;
    default:
      throw new NotSupportedException($"Type {typeof(T).Name} is not supported");
      }

   _logger.LogDebug("Set preference {Key} with value of type {Type}", key, typeof(T).Name);
    }
     catch (Exception ex)
        {
       _logger.LogError(ex, "Error setting preference {Key}", key);
  throw;
        }
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        try
        {
     if (string.IsNullOrWhiteSpace(key))
      {
      _logger.LogWarning("Attempted to remove preference with null or empty key");
     return;
            }

            Preferences.Remove(key);
            _logger.LogDebug("Removed preference {Key}", key);
     }
        catch (Exception ex)
        {
       _logger.LogError(ex, "Error removing preference {Key}", key);
    throw;
      }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        try
        {
       Preferences.Clear();
  _logger.LogInformation("Cleared all preferences");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing preferences");
   throw;
        }
  }

    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
  {
       return false;
            }

            var exists = Preferences.ContainsKey(key);
            _logger.LogTrace("Checked if preference {Key} exists: {Exists}", key, exists);
            return exists;
 }
   catch (Exception ex)
  {
         _logger.LogError(ex, "Error checking if preference {Key} exists", key);
      return false;
 }
    }
}
