<#
.SYNOPSIS
    Ejecuta automáticamente el proyecto ASP.NET Core y abre el navegador en el puerto correcto.

.DESCRIPTION
    Detecta el proyecto ejecutable, lee launchSettings.json para obtener el puerto,
    ejecuta dotnet run y abre el navegador en la URL correspondiente.

.NOTES
    Requiere .NET SDK 6.0 o superior.
#>

$ErrorActionPreference = "Stop"

Write-Host "Buscando proyecto ejecutable..." -ForegroundColor Cyan

# Buscar todos los archivos Program.cs
$programFiles = Get-ChildItem -Path . -Filter Program.cs -Recurse -ErrorAction SilentlyContinue

foreach ($programFile in $programFiles) {
    $projectDir = $programFile.Directory.FullName
    $csproj = Get-ChildItem -Path $projectDir -Filter *.csproj | Select-Object -First 1

    if ($csproj) {
        $csprojContent = Get-Content $csproj.FullName -Raw

        if ($csprojContent -match '<Project Sdk="Microsoft.NET.Sdk.Web">' -or
            $csprojContent -match '<OutputType>\s*Exe\s*</OutputType>') {

            Write-Host "Proyecto ejecutable encontrado: $($csproj.Name)" -ForegroundColor Green

            # Buscar launchSettings.json
            $launchSettingsPath = Join-Path $projectDir "Properties\launchSettings.json"
            $launchUrl = ""

            if (Test-Path $launchSettingsPath) {
                try {
                    $json = Get-Content $launchSettingsPath -Raw | ConvertFrom-Json
                    $profiles = $json.profiles.PSObject.Properties.Value

                    foreach ($profile in $profiles) {
                        if ($profile.applicationUrl) {
                            $launchUrl = $profile.applicationUrl.Split(";")[0]
                            break
                        }
                    }
                } catch {
                    Write-Warning "No se pudo leer launchSettings.json. Se usará el puerto por defecto."
                }
            }

            if ([string]::IsNullOrWhiteSpace($launchUrl)) {
                $launchUrl = "https://localhost:5001"
            }

            Write-Host "Ejecutando dotnet run..." -ForegroundColor Cyan
            Start-Process "dotnet" -ArgumentList "run --project `"$($csproj.FullName)`"" -WorkingDirectory $projectDir

            Start-Sleep -Seconds 3
            Write-Host "Abriendo navegador en: $launchUrl" -ForegroundColor Yellow
            Start-Process $launchUrl

            exit 0
        }
    }
}

Write-Error "No se encontro ningun proyecto ejecutable con Program.cs y configuracion valida."
exit 1