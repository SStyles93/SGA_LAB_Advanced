param (
    [string]$TagName
)

# This script is designed to be placed in your Unity project's root folder.
# It automates checking out a specific Git tag and launching the corresponding Unity Editor version.

if (-not $TagName) {
    Write-Host "Error: No tag name provided."
    Write-Host "Usage: gototag <tag-name>"
    exit 1
}

Write-Host "`Closing Unity if running..."
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process

Start-Sleep -Seconds 2

Write-Host "Cleaning working directory (pre-checkout)..."
git reset --hard
git clean -xfd

Write-Host "`Checking out tag: $TagName"
git checkout tags/$TagName
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to checkout tag '$TagName'. Does it exist?"
    exit 1
}

Write-Host "`Final reset and clean after switching..."
git reset --hard
git clean -xfd

# Read Unity version from ProjectSettings/ProjectVersion.txt
$versionFile = Join-Path -Path (Get-Location) -ChildPath "ProjectSettings\ProjectVersion.txt"

if (-Not (Test-Path $versionFile)) {
    Write-Host "Cannot find ProjectVersion.txt. Are you in a Unity project folder?"
    exit 1
}

$content = Get-Content $versionFile
$versionLine = $content | Where-Object { $_ -match 'm_EditorVersion:' }
if (-not $versionLine) {
    Write-Host "Could not find Unity version line in ProjectVersion.txt"
    exit 1
}

$unityVersion = $versionLine -replace 'm_EditorVersion:\s*',''
Write-Host "`Detected Unity version: $unityVersion"

# Construct Unity Editor path (adjust if your install path differs)
$unityPath = "C:\Program Files\Unity\Hub\Editor\$unityVersion\Editor\Unity.exe"

if (-Not (Test-Path $unityPath)) {
    Write-Host "Unity Editor not found at $unityPath"
    Write-Host "Please install Unity version $unityVersion via Unity Hub or update this path."
    exit 1
}

Write-Host "`Launching Unity Editor..."
Start-Process -FilePath $unityPath -ArgumentList "-projectPath `"$PWD`""

Write-Host "`Done. Tag '$TagName' is now active.`n"

