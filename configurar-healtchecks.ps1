param (
    [string]$RutaBase = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform",
    [string]$ApiMonitor = "Auth",
    [int]$PuertoBase = 5000,
    [string[]]$Microservicios = @("Auth", "Pets", "Booking", "Review", "Notification", "Payment")
)

# 1. Agregar paquetes NuGet
$proyecto = Join-Path $RutaBase "$ApiMonitor\PetCare.$ApiMonitor.Api\PetCare.$ApiMonitor.Api.csproj"
Write-Host "`n📦 Instalando paquetes NuGet en $ApiMonitor..."
dotnet add "$proyecto" package AspNetCore.HealthChecks.UI
dotnet add "$proyecto" package AspNetCore.HealthChecks.UI.Client

# 2. Actualizar appsettings.json
$appsettings = Join-Path $RutaBase "$ApiMonitor\PetCare.$ApiMonitor.Api\appsettings.json"
$healthChecks = @()

for ($i = 0; $i -lt $Microservicios.Count; $i++) {
    $nombre = $Microservicios[$i]
    $puerto = $PuertoBase + $i
    $healthChecks += @{
        Name = "PetCare.$nombre.Api"
        Uri  = "http://localhost:$puerto/health"
    }
}

if (Test-Path $appsettings) {
    $config = Get-Content $appsettings | ConvertFrom-Json
} else {
    $config = @{}
}
$config.HealthChecksUI = @{ HealthChecks = $healthChecks }

$config | ConvertTo-Json -Depth 5 | Set-Content $appsettings -Encoding UTF8
Write-Host "✔️ appsettings.json actualizado con endpoints de monitoreo."

# 3. Instrucciones para modificar Program.cs
$programPath = Join-Path $RutaBase "$ApiMonitor\PetCare.$ApiMonitor.Api\Program.cs"
Write-Host "`n✏️ Inserta manualmente las siguientes líneas en tu archivo Program.cs:"
Write-Host "   Después de: builder.Services.AddHealthChecks();" -ForegroundColor Cyan
Write-Host '       builder.Services.AddHealthChecksUI().AddInMemoryStorage();' -ForegroundColor Green

Write-Host "`n   Después de: app.UseHealthChecks(\"/health\");" -ForegroundColor Cyan
Write-Host '       app.UseHealthChecksUI(options => options.UIPath = "/health-ui");' -ForegroundColor Green

Write-Host "`n📄 Archivo objetivo: $programPath" -ForegroundColor Gray