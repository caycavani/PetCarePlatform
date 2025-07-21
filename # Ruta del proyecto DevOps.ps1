# Ruta del proyecto DevOps
$proyecto = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\DevOpsPanel\PetCare.DevOpsPanel.Api"
$exeName = "PetCare.DevOpsPanel.Api"

Write-Host "`n🛑 Deteniendo procesos anteriores si están en ejecución..."
Get-Process -Name $exeName -ErrorAction SilentlyContinue | Stop-Process -Force

Start-Sleep -Seconds 1

Write-Host "🔧 Compilando el microservicio..."
dotnet build "$proyecto"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Hubo errores durante la compilación. Revisa la consola." -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Iniciando microservicio..."
Start-Process "dotnet" -ArgumentList "run --project `"$proyecto`""

Start-Sleep -Seconds 4

Write-Host "🌐 Abriendo el navegador..."
Start-Process 'http://localhost:5050/landing.html'