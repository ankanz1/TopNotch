# Fix Smart App Control for development
# Run as Administrator

Write-Host "Attempting to fix Smart App Control..." -ForegroundColor Yellow

# Check current Smart App Control status
try {
    $status = Get-MpPreference | Select-Object -ExpandProperty SmartAppControl
    Write-Host "Smart App Control status: $status" -ForegroundColor Cyan
} catch {
    Write-Host "Could not read Smart App Control status" -ForegroundColor Red
}

# Try to add exclusions
Write-Host "`nAdding exclusions..." -ForegroundColor Yellow
try {
    Add-MpPreference -ExclusionPath "C:\Users\ankan\OneDrive\Projects\TopNotch"
    Write-Host "✓ Added folder exclusion" -ForegroundColor Green
} catch {
    Write-Host "✗ Could not add folder exclusion (may need IT admin)" -ForegroundColor Red
}

try {
    Add-MpPreference -ExclusionPath "C:\Program Files\dotnet"
    Write-Host "✓ Added dotnet exclusion" -ForegroundColor Green
} catch {
    Write-Host "✗ Could not add dotnet exclusion (may need IT admin)" -ForegroundColor Red
}

# Check if exclusions were applied
Write-Host "`nCurrent exclusions:" -ForegroundColor Yellow
$exclusions = Get-MpPreference | Select-Object -ExpandProperty ExclusionPath
$exclusions | ForEach-Object { Write-Host "  $_" }

Write-Host "`nIf still blocked, you need IT admin to:" -ForegroundColor Cyan
Write-Host "  1. Open: gpedit.msc" -ForegroundColor Gray
Write-Host "  2. Navigate: Computer Config → Administrative Templates → Windows Components → Windows Defender SmartScreen" -ForegroundColor Gray
Write-Host "  3. Set 'Enable Smart App Control' to: Disabled" -ForegroundColor Gray