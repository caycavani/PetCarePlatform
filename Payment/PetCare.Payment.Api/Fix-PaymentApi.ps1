<#
.SYNOPSIS
  Busca y ejecuta Fix-PaymentApi.ps1 desde cualquier ubicación.
#>

function Write-Log {
  param($msg, $level='INFO')
  $colors = @{ INFO='White'; WARN='Yellow'; ERROR='Red' }
  Write-Host "[$Level] $msg" -ForegroundColor $colors[$Level]
}

# 1. Buscar el script
Write-Log "Buscando Fix-PaymentApi.ps1..."
$script = Get-ChildItem -Recurse -Filter Fix-PaymentApi.ps1 -ErrorAction SilentlyContinue | Select-Object -First 1

if (-not $script) {
  Write-Log "No se encontró Fix-PaymentApi.ps1 en esta solución." ERROR
  exit 1
}

# 2. Ejecutar el script
Write-Log "Ejecutando: $($script.FullName)"
try {
  powershell -NoProfile -ExecutionPolicy Bypass -File $script.FullName
  Write-Log "✅ Script ejecutado correctamente." INFO
} catch {
  Write-Log "❌ Error al ejecutar el script: $_" ERROR
}