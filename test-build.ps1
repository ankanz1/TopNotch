# Test build script - verifies compilation without Smart App Control blocking
Write-Host "Building TopNotch..." -ForegroundColor Cyan

# Change to src directory
Set-Location $PSScriptRoot\src

# Build the project
dotnet build -c Debug

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To run the application:" -ForegroundColor Yellow
    Write-Host "1. Open Windows Security → App & browser control" -ForegroundColor Gray
    Write-Host "2. Under Smart App Control, select 'Check apps and files'" -ForegroundColor Gray
    Write-Host "3. When prompted, click 'More info' → 'Run anyway'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Or temporarily disable Smart App Control for development:" -ForegroundColor Yellow
    Write-Host "Windows Security → App & browser control → Smart App Control → Turn off" -ForegroundColor Gray
} else {
    Write-Host "❌ Build failed with error code $LASTEXITCODE" -ForegroundColor Red
}