# Troubleshooting

## Smart App Control Blocked

Windows 11's Smart App Control (formerly Windows Defender SmartScreen/App Control) is blocking the application because it's unsigned and the native Avalonia DLLs are triggering the policy.

### Primary Solution: Add Defender Exclusion

**This is the fastest solution for development:**

**Option 1: GUI Method (click method):**
1. Open **Windows Security** → **Virus & threat protection** → **Exclusions**
2. Click **Add or remove exclusions** → **Add an exclusion**
3. Add **BOTH**:
   - **Folder**: `C:\Users\ankan\OneDrive\Projects\TopNotch`
   - **File**: `C:\Users\ankan\OneDrive\Projects\TopNotch\src\bin\Debug\net8.0\TopNotch.exe`

**Option 2: PowerShell Method (recommended):**
Open PowerShell as **Administrator** and run:
```powershell
Add-MpPreference -ExclusionPath "C:\Users\ankan\OneDrive\Projects\TopNotch"
Add-MpPreference -ExclusionPath "C:\Users\ankan\OneDrive\Projects\TopNotch\src\bin\Debug\net8.0"
```

Then restart PowerShell or run:
```powershell
$env:PATH += ";C:\Program Files\dotnet"
```

### Alternative Solutions

#### Option A: Use PowerShell with Administrator privileges
Open PowerShell as Administrator and run:
```powershell
# Add exclusion for project folder
Add-MpPreference -ExclusionPath "C:\Users\ankan\OneDrive\Projects\TopNotch"

# Or unblock all binaries at once
Get-ChildItem -Path "C:\Users\ankan\OneDrive\Projects\TopNotch\src\bin\Debug\net8.0" -Include *.dll,*.exe | Unblock-File
```

#### Option B: Temporarily disable Smart App Control
1. Go to **Windows Security** → **App & browser control**
2. Under "Smart App Control", set to **Off (Evaluation mode)**
3. Restart PowerShell and try running again

#### Option C: Run from VS Code integrated terminal
Visual Studio Code's integrated terminal sometimes bypasses Windows policies:
```powershell
# From the VS Code terminal
cd "C:\Users\ankan\OneDrive\Projects\TopNotch\src"
dotnet run
```

#### Option D: Publish as self-contained
Build a standalone executable that doesn't require dotnet to be installed:
```powershell
dotnet publish -c Release -r win-x64 --self-contained true
```
The published exe will be a single file that can be run directly.

### Expected Console Output
Once Smart App Control is bypassed, you should see:
```
TopNotch starting...
OnFrameworkInitializationCompleted called
Creating IslandWindow...
IslandWindow created, starting position: (x, y)
IslandWindow.Show() called
```

### Expected Windows PowerShell Commands After Exclusion
Once exclusions are added, open a **new** PowerShell and run:
```powershell
cd "C:\Users\ankan\OneDrive\Projects\TopNotch\src"
& "C:\Program Files\dotnet\dotnet.exe" run
```

Or run the published exe directly (if you publish):
```powershell
& "C:\Users\ankan\OneDrive\Projects\TopNotch\src\bin\Debug\net8.0\TopNotch.exe"
```

### If Nothing Works:
If you're on a managed device (work/school computer), contact your IT administrator to:
1. Add the project folder to Windows Defender exclusions for **all users**, OR
2. Temporarily disable Smart App Control for development
3. Whitelist: `C:\Program Files\dotnet\dotnet.exe` and the entire `src\bin` folder