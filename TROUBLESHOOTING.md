# Troubleshooting

## Smart App Control Blocked

Windows 11's Smart App Control (formerly Windows Defender SmartScreen/App Control) is blocking the application because it's unsigned.

### Solutions:

#### Option 1: Disable Smart App Control Temporarily (Recommended for development)
1. Go to **Windows Security** → **App & browser control**
2. Under "Smart App Control", select **Check apps and files**
3. Click "Allow an app through Smart App Control"
4. Browse to and select `src\bin\Debug\net8.0\TopNotch.exe`
5. Choose "Allow"

#### Option 2: Run from PowerShell with bypass
```powershell
cd src
dotnet run
```
Then when Smart App Control prompts, click "More info" → "Run anyway"

#### Option 3: Sign the application (for production)
For production deployment, you'll need to:
1. Obtain an Authenticode code signing certificate
2. Sign using: `signtool sign /fd SHA256 /a /tr http://timestamp.digicert.com /td SHA256 <path_to_exe>`

#### Option 4: Add to Windows Defender exclusions
1. Windows Security → Virus & threat protection → Exclusions
2. Add an exclusion for the project folder
3. Add exe exclusion for `TopNotch.exe`

## Alternative: Test with console visibility
Run from PowerShell to see if console output shows before the block:
```powershell
dotnet build
# If build succeeds, the code is working
```

## If Smart App Control persists:
1. Open Windows Security
2. App & browser control → Reputation-based protection
3. Turn off "Check apps and files" temporarily
4. Or contact your IT admin if on a managed device