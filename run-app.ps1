# Run TopNotch app with Smart App Control bypass
# Run PowerShell as Administrator before using this

# Add full exclusions
Add-MpPreference -ExclusionPath "C:\Users\ankan\OneDrive\Projects\TopNotch"
Add-MpPreference -ExclusionPath "C:\Users\ankan\OneDrive\Projects\TopNotch\src\bin"
Add-MpPreference -ExclusionProcess "C:\Program Files\dotnet\dotnet.exe"

Write-Host "Exclusions added. Starting app..." -ForegroundColor Green
Write-Host "If blocked again, check Windows Defender Security Center" -ForegroundColor Yellow

# Navigate and run
Set-Location "C:\Users\ankan\OneDrive\Projects\TopNotch\src"
& "C:\Program Files\dotnet\dotnet.exe" run