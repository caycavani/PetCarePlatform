$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api\wwwroot"
$logDir = Join-Path $basePath "LogsDevOps"
$launcherPs1 = Join-Path $basePath "AlertLoggerLauncher.ps1"
$loggerPs1 = Join-Path $basePath "RegistrarAlerta.ps1"
$launcherBat = Join-Path $basePath "AlertLoggerLauncher.bat"

# Crear carpeta de logs si no existe
if (-not (Test-Path $logDir)) {
    New-Item -ItemType Directory -Path $logDir | Out-Null
    Write-Host "📂 Carpeta LogsDevOps creada." -ForegroundColor Green
}
$registrarAlertaContenido = @'
param(
    [string]$entorno,
    [string]$servicio,
    [string]$tipo,
    [string]$detalle
)

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$fecha = Get-Date -Format "yyyy-MM-dd"
$linea = "[$timestamp] [$entorno] $servicio ⚠️ $tipo ($detalle)"
$logPath = Join-Path (Join-Path "$PSScriptRoot" "LogsDevOps") "alertas-$fecha.log"
Add-Content -Path $logPath -Value $linea
'@

Set-Content -Path $loggerPs1 -Value $registrarAlertaContenido -Encoding UTF8
Write-Host "✅ RegistrarAlerta.ps1 creado." -ForegroundColor Green