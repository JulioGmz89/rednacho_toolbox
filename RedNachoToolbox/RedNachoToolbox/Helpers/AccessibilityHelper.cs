using Microsoft.Maui.Controls;

namespace RedNachoToolbox.Helpers;

/// <summary>
/// Helper class for setting accessibility properties on MAUI controls.
/// Provides methods to improve screen reader support and keyboard navigation.
/// </summary>
public static class AccessibilityHelper
{
    /// <summary>
    /// Sets automation properties for a control to improve accessibility.
    /// </summary>
    public static void SetAccessibility(
      Element element,
        string name,
     string helpText = "",
    string hint = "")
    {
     if (element == null) return;

        AutomationProperties.SetName(element, name);
        
     if (!string.IsNullOrEmpty(helpText))
   {
   AutomationProperties.SetHelpText(element, helpText);
        }

        // Set semantic properties for better screen reader support
        if (element is View view)
   {
            SemanticProperties.SetDescription(view, name);
    
     if (!string.IsNullOrEmpty(hint))
      {
        SemanticProperties.SetHint(view, hint);
            }
        }
    }

    /// <summary>
    /// Makes an element a keyboard focus target for better keyboard navigation.
    /// </summary>
    public static void SetKeyboardTarget(Element element, bool isKeyboardTarget = true)
    {
if (element == null) return;

AutomationProperties.SetIsInAccessibleTree(element, true);
  
        // Note: MAUI doesn't have IsTabStop like WPF/UWP
     // Keyboard navigation is handled automatically for interactive controls
    }

    /// <summary>
    /// Sets the role/type of a control for screen readers.
    /// </summary>
    public static void SetRole(Element element, string role)
    {
if (element == null || string.IsNullOrEmpty(role)) return;

// Note: MAUI doesn't have direct role setting, but we can use name convention
   var currentName = AutomationProperties.GetName(element);
        if (string.IsNullOrEmpty(currentName))
        {
        AutomationProperties.SetName(element, role);
        }
  }

    /// <summary>
    /// Configures a button for accessibility with descriptive text.
    /// </summary>
    public static void SetupButton(Button button, string name, string action)
    {
        if (button == null) return;

  SetAccessibility(button, name, $"Button. {action}", $"Double tap to {action.ToLower()}");
        SetKeyboardTarget(button, true);
    }

    /// <summary>
    /// Configures an entry/input field for accessibility.
    /// </summary>
    public static void SetupEntry(Entry entry, string label, string hint = "")
    {
        if (entry == null) return;

        var helpText = string.IsNullOrEmpty(hint) ? $"Text input for {label}" : hint;
        SetAccessibility(entry, label, helpText, "Double tap to edit");
   SetKeyboardTarget(entry, true);
    }

    /// <summary>
    /// Configures a label for accessibility (usually not focusable).
    /// </summary>
    public static void SetupLabel(Label label, string text, bool isHeading = false)
    {
if (label == null) return;

var name = isHeading ? $"Heading: {text}" : text;
   SetAccessibility(label, name);
   AutomationProperties.SetIsInAccessibleTree(label, true);
        
  // Labels are automatically not focusable in MAUI
    }
}
