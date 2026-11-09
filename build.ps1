# Build MyCleaner, create Setup.exe and print its SHA256
$ErrorActionPreference = "Stop"

dotnet publish .\MyCleaner.csproj -c Release -r win-x64 --self-contained true -o .\publish

$iscc = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
& $iscc .\installer\setup.iss

$setup = Get-ChildItem .\output\MyCleaner-Setup-*.exe | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$hash = (Get-FileHash $setup.FullName -Algorithm SHA256).Hash.ToLower()

Write-Host ""
Write-Host "Done! File: $($setup.FullName)" -ForegroundColor Green
Write-Host "SHA256: $hash" -ForegroundColor Yellow
Write-Host "Put this hash in the sha256 field of version.json"
