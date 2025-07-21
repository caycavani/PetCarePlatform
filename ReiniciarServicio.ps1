param (
    [Parameter(Mandatory)]
    [string]$Servicio
)

try {
    Write-Host "🔄 Reiniciando servicio: $Servicio" -ForegroundColor Cyan
    Restart-Service -Name $Servicio -Force -ErrorAction Stop
    Write-Host "✅ Servicio $Servicio reiniciado correctamente." -ForegroundColor Green
} catch {
    Write-Host "❌ Error al reiniciar servicio ${Servicio}:`n$($_.Exception.Message)" -ForegroundColor Red
}