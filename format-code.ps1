<#
.SYNOPSIS
    Aplica formato a la solución C# usando dotnet format con detección automática del archivo .sln.

.DESCRIPTION
    Este script busca el archivo .sln en el directorio actual o subdirectorios,
    y aplica las reglas de estilo definidas en .editorconfig y stylecop.json.

.NOTES
    Requiere .NET SDK 6.0 o superior.
#>

$ErrorActionPreference = "Stop"

Write-Host "Buscando archivo .sln..."

# Buscar el primer archivo .sln en el directorio actual o subdirectorios
$solution = Get-ChildItem -Path . -Filter *.sln -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1

if (-not $solution) {
    Write-Error "No se encontró ningún archivo .sln en este directorio ni en subdirectorios."
    exit 1
}

$solutionPath = $solution.FullName
Write-Host "Solución encontrada: $($solution.Name)"

# Ejecutar dotnet format
Write-Host "Aplicando dotnet format a la solución..."
$formatResult = dotnet format "$solutionPath" --verify-no-changes --severity info

if ($LASTEXITCODE -eq 0) {
    Write-Host "El código está correctamente formateado."
} else {
    Write-Warning "Se encontraron archivos sin formatear."
    Write-Host $formatResult
    exit 1
}