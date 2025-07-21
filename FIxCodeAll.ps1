<#
.SYNOPSIS
  Limpia archivos temporales, desbloquea archivos y restaura la solución.
.DESCRIPTION
  - Elimina carpetas bin/, obj/ y packages/
  - Quita atributos de solo lectura
  - Limpia la caché de NuGet
  - Ejecuta dotnet restore sobre la solución
#>

function Write-Log {
  param($Message, $Level='INFO')
  $colors = @{ INFO='White'; WARN='Yellow'; ERROR='Red' }
  Write-Host "[$Level] $Message" -ForegroundColor $colors[$Level]
}

# 1. Eliminar bin/, obj/, packages/
Write-Log "Eliminando carpetas bin/, obj/ y packages/..."
Get-ChildItem -Recurse -Directory -Include bin,obj,packages -ErrorAction SilentlyContinue |
  ForEach-Object {
    try {
      Remove-Item $_.FullName -Recurse -Force -ErrorAction Stop
      Write-Log "  Eliminado: $($_.FullName)"
    } catch {
      Write-Log "  No se pudo eliminar: $($_.FullName)" WARN
    }
  }

# 2. Desbloquear archivos de solo lectura
Write-Log "Desbloqueando archivos de solo lectura..."
Get-ChildItem -Recurse -File | Where-Object { $_.IsReadOnly } | ForEach-Object {
  $_.IsReadOnly = $false
  Write-Log "  Desbloqueado: $($_.FullName)"
}

# 3. Limpiar caché de NuGet
Write-Log "Limpiando caché de NuGet..."
dotnet nuget locals all --clear

# 4. Restaurar solución
$solution = Get-ChildItem -Filter *.sln | Select-Object -First 1
if ($solution) {
  Write-Log "Restaurando solución: $($solution.Name)..."
  dotnet restore $solution.FullName
} else {
  Write-Log "No se encontró archivo .sln en esta carpeta." ERROR
}