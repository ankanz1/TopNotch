# TopNotch

A lightweight, always-on-top media display window built with **Avalonia UI** (.NET 8). Designed to show current time and media controls in a minimal, non-intrusive manner.

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/pixelisland/TopNotch)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Platform: Windows](https://img.shields.io/badge/platform-Windows-blue.svg)](https://github.com/pixelisland/TopNotch)

## Features

- **Minimal Footprint**: Compact window that shows current time
- **Expandable**: Click to expand for media controls
- **Always on Top**: Stays visible above other applications
- **Custom Theming**: Fluent design theme with dark/light support
- **Media Ready**: Designed for displaying "Now Playing" information
- **Smooth Animations**: Expand/collapse with smooth transitions

## Screenshots

<img src="docs/screenshot-compact.png" alt="Compact view showing time" width="300" style="border: 1px solid #ccc; border-radius: 8px; margin-right: 10px;">
<img src="docs/screenshot-expanded.png" alt="Expanded view with media controls" width="300" style="border: 1px solid #ccc; border-radius: 8px;">

## Prerequisites

Before building the project, ensure you have:

- **.NET SDK 8.0** or later
- **Windows 10/11** (for Avalonia desktop support)
- **8GB RAM recommended** (for full feature set)

### Installing .NET SDK

```powershell
# Download from https://dotnet.microsoft.com/download/dotnet/8.0
# Or using winget:
winget install Microsoft.DotNet.SDK.8

# Verify installation:
dotnet --version
```

## Building

### Clone the Repository

```bash
git clone https://github.com/pixelisland/TopNotch.git
cd TopNotch
```

### Build the Project

```powershell
cd src
dotnet build -c Debug -r win-x64 --self-contained false
```

### Build Output

After a successful build, you'll find the application at:

```
src\bin\Debug\net8.0\win-x64\TopNotch.exe
```

## Running

### Method 1: Direct Execution

```powershell
cd src
.\bin\Debug\net8.0\win-x64\TopNotch.exe
```

### Method 2: Using dotnet CLI

```powershell
cd src
dotnet run --project TopNotch.csproj
```

### Method 3: Release Build

```powershell
cd src
dotnet publish -c Release -r win-x64 --self-contained true
Start-Process .\bin\Release\net8.0\win-x64\publish\TopNotch.exe
```

## Development

### Project Structure

```
TopNotch/
├── src/                          # Main project source
│   ├── App/                        # Application entry point
│   │   ├── App.xaml               # Application XAML
│   │   └── App.xaml.cs            # Application code-behind
│   ├── Views/                      # UI Views
│   │   ├── Windows/               # Window views
│   │   │   ├── IslandWindow.xaml   # Main compact window
│   │   │   ├── IslandWindow.xaml.cs
│   │   │   ├── MainWindow.xaml     # Main application window
│   │   │   ├── MainWindow.xaml.cs
│   │   │   ├── Settings.xaml       # Settings panel
│   │   │   └── Settings.xaml.cs
│   │   └── Controls/              # Reusable controls
│   │       ├── MediaCard.xaml
│   │       ├── MediaCard.xaml.cs
│   │       ├── SettingsCard.xaml
│   │       └── SettingsCard.xaml.cs
│   ├── ViewModels/                # MVVM ViewModels
│   ├── Models/                    # Data models
│   ├── Program.cs                 # Application entry point
│   └── TopNotch.csproj           # Project file
├── docs/                          # Documentation
│   ├── screenshot-compact.png      # Compact view screenshot
│   └── screenshot-expanded.png     # Expanded view screenshot
├── fix-smart-control.ps1          # Smart App Control fix script
├── run-app.ps1                    # App runner script
└── test-build.ps1                 # Build verification script
```

## Troubleshooting

### Smart App Control Blocking the App

If your application is blocked by Windows Smart App Control, you can use the provided fix script:

```powershell
# Run as Administrator
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process -Force
.\fix-smart-control.ps1
```

Or manually add exclusions in Windows Security:
1. Go to **Windows Security → Virus & threat protection → Exclusions**
2. Add an exclusion for the project folder and `C:\Program Files\dotnet`

### Common Issues

#### App Won't Start

1. Ensure .NET 8 SDK is installed: `dotnet --version`
2. Check the build output for errors
3. Run from the `src` directory

#### Window Not Appearing

- Check if window is positioned off-screen
- Try `Task View` (Win+Tab) to locate the window
- The window starts at top center of primary monitor

#### XAML Loading Errors

The application gracefully handles XAML loading issues. If you see warnings in the console, the app will still function with default styling.

### Development Mode

For development with console output:

```powershell
# Set OutputType to Exe for console visibility
# This changes in src/TopNotch.csproj
<OutputType>Exe</OutputType>  # Instead of WinExe
```

## Configuration

### App Settings

The application uses a `FluentTheme` by default. Configuration can be found in:

- `App.xaml` - Application-level styles and resources
- `Views/Windows/Settings.xaml` - User-configurable settings panel

### Supported Settings

- **Start with system**: Auto-launch on Windows startup
- **Minimize to tray**: Minimize to system tray instead of closing
- **Check for updates**: Enable auto-update checks
- **Theme**: Switch between different UI themes

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make your changes
4. Run tests: `.\test-build.ps1`
5. Commit your changes: `git commit -m 'Add my feature'`
6. Push to branch: `git push origin feature/my-feature`
7. Submit a Pull Request

### Development Setup

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/TopNotch.git
cd TopNotch

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run --project src/TopNotch.csproj
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [Avalonia](https://avaloniaui.net/) - A cross-platform UI framework
- Inspired by notification widget designs
- Uses [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) for MVVM pattern

## Support

- 📧 **Contact**: [your-email@example.com]
- 🐛 **Bug Reports**: Open an Issue
- 💡 **Feature Requests**: Open an Issue with "Enhancement" label

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)

*Built with ❤️ for Windows users who want a minimal, elegant media display widget.*