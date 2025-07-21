# Script: Build-IsolatedAuth.ps1
# Descripcion: Compila PetCare.Auth.Api de forma aislada y genera la imagen Docker sin tocar el entorno local

param (
    [string]$SolutionRoot = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform",
    [string]$TempBuildDir = "$env:TEMP\PetCareAuthBuild",
    [string]$ImageName = "petcare-auth-api"
)

Write-Host ""
Write-Host "Limpieza del entorno temporal: $TempBuildDir" -ForegroundColor Cyan
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $TempBuildDir
New-Item -ItemType Directory -Path $TempBuildDir | Out-Null

Write-Host "Copiando codigo fuente necesario..." -ForegroundColor Cyan
Copy-Item -Recurse -Path "$SolutionRoot\Auth\PetCare.Auth.Api" -Destination "$TempBuildDir\PetCare.Auth.Api"
Copy-Item -Recurse -Path "$SolutionRoot\Auth\PetCare.Auth.Infrastructure" -Destination "$TempBuildDir\PetCare.Auth.Infrastructure"
Copy-Item -Recurse -Path "$SolutionRoot\Auth\PetCare.Auth.Domain" -Destination "$TempBuildDir\PetCare.Auth.Domain"
Copy-Item -Path "$SolutionRoot\Auth\PetCare.Auth.Api\Dockerfile" -Destination "$TempBuildDir\Dockerfile"

Write-Host "Eliminando carpetas bin y obj..." -ForegroundColor Cyan
Get-ChildItem -Path $TempBuildDir -Include bin,obj -Recurse | Remove-Item -Force -Recurse

Write-Host "Ejecutando docker build..." -ForegroundColor Cyan
Set-Location $TempBuildDir
docker build -t $ImageName .

Write-Host ""
Write-Host "Imagen '$ImageName' creada correctamente." -ForegroundColor Green
docker images $ImageName
