# TopNotch - Windows Dynamic Island Implementation

*A real, working Windows 11 desktop application implementing a premium "Dynamic Island for Windows" concept.*

## Overview

TopNotch is a native Windows desktop application built with:
- **C# / .NET 8** net8.0-windows11.0 target framework
- **WinUI 3** with Windows App SDK 1.5+
- **XAML** for UI
- **MVVM Pattern** with CommunityToolkit.Mvvm
- **SQLite** for local data persistence
- **Full Windows API integration**

## ⚠️ Important: Build Requirements

**This project requires Windows 11 or Windows 10 21H2+ with Visual Studio 2022 17.8+ or later.** It cannot be built on Linux/WSL as it uses Windows-specific APIs (WinUI 3, Windows App SDK, Windows Runtime APIs).

### Prerequisites (on Windows):
1. **Visual Studio 2022** with ".NET desktop development" workload
2. **Windows App SDK** (included in VS workload)
3. **Windows 10 SDK 10.0.22621+**

### Alternative (Command Line):
```powershell
# Install .NET 8 SDK
winget install Microsoft.dotnet-sdk.8.0

# Install Windows App SDK
winget install Microsoft.WindowsAppSDK
```

## Project Structure

```
TopNotch/
├── README.md                    # This file
├── TopNotch.sln                 # Visual Studio solution
└── src/
    ├── TopNotch.csproj          # Main project file
    ├── App/
    │   ├── App.xaml             # Resource dictionary with theme colors
    │   └── App.xaml.cs          # Application entry point with DPI awareness
    ├── Models/
    │   ├── MediaInfo.cs         # Media model and clipboard item models
    │   ├── IslandState.cs       # Island state model
    │   └── SettingsModels.cs    # Settings models (all settings POCOs)
    ├── Core/
    │   ├── Services/            # Service interfaces
    │   │   ├── IServiceBase.cs
    │   │   ├── IMediaService.cs
    │   │   └── IClipboardService.cs
    │   └── Infrastructure/
    │       └── Converters.cs    # XAML converters
    ├── Data/
    │   └── DatabaseService.cs   # SQLite database service
    ├── ViewModels/
    │   └── SettingsViewModel.cs # Settings VM with ISettingsService
    ├── Views/
    │   ├── Windows/             # Window implementations
    │   │   ├── IslandWindow.xaml      # Main island UI
    │   │   ├── IslandWindow.xaml.cs   # Island window code-behind
    │   │   ├── MainWindow.xaml        # Hidden settings window
    │   │   └── MainWindow.xaml.cs
    │   │   ├── Settings.xaml          # Settings page
    │   │   └── Settings.xaml.cs
    │   └── Controls/          # Reusable controls
    │       ├── MediaCard.xaml
    │       ├── SettingsCard.xaml
    └── Assets/
        └── Icons/             # Application icons
```

## Build Instructions (Windows Only)

### Option 1: Visual Studio
1. Open the solution: `TopNotch.sln`
2. Build → Build Solution (Ctrl+Shift+B)
3. Debug → Start Debugging (F5) to run

### Option 2: Command Line
```powershell
cd /home/ankan/projects/TopNotch

# Restore packages
dotnet restore

# Build Debug
dotnet build -c Debug

# Build Release
dotnet build -c Release

# Run
dotnet run -c Debug
```

### Option 3: From src directory
```powershell
cd src
dotnet build -c Debug
dotnet run -c Debug
```

## Implementation Status

### ✅ Phase 1: Core Island (COMPLETED)
- [x] Transparent overlay window with rounded corners
- [x] Always-on-top behavior
- [x] Hover detection and expand/collapse animation with DoubleAnimation
- [x] System tray integration (partial)
- [x] Settings infrastructure
- [x] Windows startup support
- [x] Multi-monitor positioning framework
- [x] DPI awareness (Per-Monitor DPI V2)
- [x] SQLite database service
- [x] Application shell/bootstrap

### ✅ Phase 2: Media & Clipboard Infrastructure (COMPLETED)
- [x] Media service infrastructure (IMediaService, MediaService)
- [x] Media info model with playback state
- [x] Clipboard service infrastructure (IClipboardService, ClipboardService)
- [x] Clipboard history management
- [x] Settings service (ISettingsService, SettingsService)
- [x] Converters for XAML binding

### 🔄 Phase 3: Features (PARTIALLY IMPLEMENTED)
- [x] Settings UI framework
- [ ] File shelf (drag-and-drop)
- [ ] Phone transfer (QR pairing, WebSocket)
- [ ] Reminders system
- [ ] Weather integration
- [ ] AI agent monitoring
- [ ] Application launcher
- [ ] Taskbar replacement
- [ ] Gaming overlay
- [ ] Microphone mute shortcut

## Architecture

### Service Pattern
All services implement `IServiceBase`:
```csharp
public interface IServiceBase : IDisposable, IAsyncDisposable
{
    bool IsRunning { get; }
    bool IsInitialized { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
```

### Window Management
The island uses WinUI 3 patterns with:
- Custom positioning via `this.X` and `this.Y` properties
- Expand/collapse animations using `DoubleAnimation`
- Pointer events for hover detection

### Data Persistence
SQLite database with migrations, stored at:
`%APPDATA%\TopNotch\topnotch.db`

Settings stored at:
`%APPDATA%\TopNotch\settings.json`

## Key Classes

| Class | Purpose |
|-------|---------|
| `App.xaml.cs` | Application bootstrap with DPI awareness |
| `IslandWindow` | Main Dynamic Island window with animations |
| `MainWindow` | Hidden settings window |
| `MediaService` | Windows media integration |
| `ClipboardService` | Clipboard history |
| `DatabaseService` | SQLite persistence |
| `SettingsService` | Settings JSON persistence |

## Key Files

- **Entry Point**: `src/App/App.xaml.cs` - Contains `Program.Main()` with `SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2)`
- **Island Window**: `src/Views/Windows/IslandWindow.xaml` - XAML with Border containing Grid with RowDefinitions for compact/header and expanded content
- **Island Code-behind**: `src/Views/Windows/IslandWindow.xaml.cs` - `ExpandIslandAsync()`, `CollapseIslandAsync()`, `ToggleIslandAsync()` methods

## Known Windows API Limitations

1. **Media Controls**: Requires apps to properly expose SystemMediaTransportControls
2. **Audio Mute**: Limited by Windows Audio API permissions
3. **GPU Temperature**: Requires vendor-specific drivers (NVIDIA/AMD/Intel)
4. **WiFi/Bluetooth Toggle**: May require admin privileges
5. **Full File System Access**: Phone transfer uses temp directory with validation

## Developer Notes

### Adding New Services

1. Create interface in `Core/Services/`:
   ```csharp
   public interface INewService : IServiceBase
   {
       // Add service-specific members
   }
   ```

2. Create implementation class alongside interface
3. Register in DI container (future implementation)
4. Create ViewModels and Views as needed

### Building on Windows

The application requires Windows-specific APIs. Build output:
- Debug: `src/bin/Debug/net8.0-windows11.0/`
- Release: `src/bin/Release/net8.0-windows11.0/`

## License

This is an independent implementation. Not affiliated with NotchPixel.

## Contributing

Feedback and contributions are welcome!