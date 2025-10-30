# .NET 9 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 9 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9 upgrade.
3. Upgrade RedNachoToolbox\RedNachoToolbox.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name      | Current Version | New Version | Description          |
|:---------------------------------------|:---------------:|:-----------:|:-----------------------------------------------|
| ColorPicker.Maui           | 1.0.0           |             | No compatible version found for .NET 9|
| Microsoft.Extensions.Logging.Debug     | 8.0.1   | 9.0.10      | Recommended for .NET 9  |
| System.Drawing.Common       | 8.0.6      | 9.0.10      | Recommended for .NET 9        |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### RedNachoToolbox\RedNachoToolbox.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `net8.0-android;net8.0-ios;net8.0-maccatalyst;net8.0-windows10.0.19041.0` to `net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0`

NuGet packages changes:
  - Microsoft.Extensions.Logging.Debug should be updated from `8.0.1` to `9.0.10` (*recommended for .NET 9*)
  - System.Drawing.Common should be updated from `8.0.6` to `9.0.10` (*recommended for .NET 9*)
  - ColorPicker.Maui `1.0.0` - No compatible version found for .NET 9. This package may need to be removed or replaced with an alternative.

Other changes:
  - Review and update MAUI workload specific properties for .NET 9 compatibility
  - Verify SupportedOSPlatformVersion values are compatible with new target frameworks
  - Update any platform-specific configurations as needed
