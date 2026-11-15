# TopNotch Avalonia Migration Plan

## Overview
Migration from WinUI 3 / Windows App SDK to Avalonia UI for Linux compatibility. The entire application must be developed, built, tested, and runnable entirely inside Ubuntu/Linux with ZERO Windows dependencies.

## Existing Code Analysis

### ✅ REUSABLE Models (/home/ankan/projects/TopNotch/src/Models/)
- **MediaInfo.cs** - Can be reused with minor namespace adjustments
- **ClipboardItem.cs** - Can be reused
- **IslandState.cs** - Can be reused with Avalonia ObservableObject pattern
- **SettingsModels.cs** - All settings POCOs can be reused
- **IslandMode, IslandPosition, MonitorSelection, ThemeType enums** - Can be reused

### ✅ REUSABLE Services (/home/ankan/projects/TopNotch/src/Core/Services/)
- **IServiceBase.cs** - Fully reusable base interface/class
- **ServiceBase.cs** - Fully reusable base implementation
- **DatabaseService.cs** - Can be reused (SQLite is cross-platform)
- **IMediaService.cs** - Needs Avalonia-compatible media abstraction

### ✅ REUSABLE ViewModels (/home/ankan/projects/TopNotch/src/ViewModels/)
- **SettingsViewModel.cs** - Can be adapted for Avalonia
- CommunityToolkit.Mvvm patterns transfer well

### ⚠️ NEEDS ADAPTATION (/home/ankan/projects/TopNotch/src/Data/)
- **DatabaseService.cs** - SQLite works on Linux, but path logic needs adjustment (no AppData)
- Table mapping needs verification for Avalonia usage patterns

### ❌ NOT REUSABLE (/home/ankan/projects/TopNotch/src/Views/)
- All XAML files - Need Avalonia conversion
- All code-behind with Windows APIs
- IslandWindow with Win32 interop
- DPI awareness Windows-specific code

## Migration Plan

### Phase 0: Setup Avalonia Project

1. Create new Avalonia App project targeting .NET 8
2. Add required NuGet packages:
   - Avalonia.Desktop
   - Avalonia.Themes.Default
   - Avalonia.Controls
   - CommunityToolkit.Mvvm
   - Microsoft.Data.Sqlite
   - SQLitePCLRaw.bundle_e_sqlite3 (or equivalent)

3. Update .csproj to target .NET 8 and use Avalonia SDK

### Phase 1: Migrate Core Models
1. Move reusable models to new project structure
2. Adjust namespaces from `TopNotch.Models` to appropriate Avalonia namespaces
3. Verify all model classes compile

### Phase 2: Migrate Services
1. DatabaseService - verify SQLite path logic for Linux
2. Media service - create Avalonia-compatible abstraction
3. Clipboard service - use Avalonia/OS clipboard APIs

### Phase 3: Migrate ViewModels
1. Adapt SettingsViewModel for Avalonia data binding
2. Update IslandState to use Avalonia properties
3. Create any new ViewModels needed for Avalonia

### Phase 4: Create Avalonia XAML Views
1. Replace IslandWindow.xaml with Avalonia native XAML
2. Replace MainWindow.xaml with Avalonia native XAML
3. Replace Settings.xaml with Avalonia native XAML
4. Replace MediaCard.xaml with Avalonia native XAML
5. Replace SettingsCard.xaml with Avalonia native XAML

### Phase 5: Migrate App Startup
1. Replace App.xaml.cs with Avalonia startup
2. Replace Program.cs entry point
3. Set up Linux-compatible window positioning

### Phase 6: Build and Test
1. `dotnet restore`
2. `dotnet build`
3. `dotnet run`
4. Verify all milestones work

### Milestones:
1. Avalonia application launches in Ubuntu
2. Main TopNotch window appears
3. Top-center floating island UI works
4. Transparent/frameless window behavior (as supported by Ubuntu)
5. Island can expand and collapse
6. Basic navigation works
7. Settings screen works
8. SQLite database initializes
9. Existing services connected where practical
10. Application can be closed cleanly

## NuGet Packages Required (Linux/Avalonia)

```xml
<PackageReference Include="Avalonia.Desktop" Version="11.0.0" />
<PackageReference Include="Avalonia.Themes.Default" Version="11.0.0" />
<PackageReference Include="Avalonia.Controls" Version="11.0.0" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.12" />
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.6" />
```

## Key Changes from WinUI 3 to Avalonia

1. **Project SDK**: `Microsoft.NET.Sdk.WindowsDesktop` → `Avalonia.Desktop`
2. **XAML Namespaces**: `xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"` stays the same but renders differently
3. **Window Setup**: Remove `ExtendsContentIntoTitleBar`, use `Avalonia-style` window chrome
4. **Data Binding**: `x:Bind` works the same, some conventions differ
5. **Resources**: `ThemeResource` vs `StaticResource` differences
6. **Services**: Replace Windows-specific APIs with cross-platform alternatives