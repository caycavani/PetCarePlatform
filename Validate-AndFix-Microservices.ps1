# BuildSolution.ps1
$root     = Split-Path -Parent $MyInvocation.MyCommand.Path
$solution = Get-ChildItem -Path $root -Recurse -Filter '*.sln' |
            Select-Object -First 1

if (-not $solution) {
  Write-Error "No .sln file found under $root"
  exit 1
}

Write-Host "Using solution: $($solution.FullName)" -ForegroundColor Cyan

# Restore + build
dotnet restore $solution.FullName --nologo --verbosity minimal
if ($LASTEXITCODE -ne 0) { exit 1 }

dotnet build   $solution.FullName -c Release -m --no-restore --verbosity minimal
exit $LASTEXITCODE