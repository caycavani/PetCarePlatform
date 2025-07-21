$BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"
$ComposePath = Join-Path $BasePath "docker-compose.yml"

$Microservicios = @(
    @{ Nombre = "auth-api"; Ruta = "Auth/PetCare.Auth.Api"; ComposeKey = "auth-api" },
    @{ Nombre = "booking-api"; Ruta = "Booking/PetCare.Booking.Api"; ComposeKey = "booking-api" },
    @{ Nombre = "notification"; Ruta = "Notification/PetCare.Notification.Api"; ComposeKey = "notification-api" },
    @{ Nombre = "payment-api"; Ruta = "Payment/PetCare.Payment.Api"; ComposeKey = "payment-api" },
    @{ Nombre = "pets-api"; Ruta = "Pets/PetCare.Pets.Api"; ComposeKey = "pets-api" },
    @{ Nombre = "review-api"; Ruta = "Review/PetCare.Review.Api"; ComposeKey = "review-api" }
)

# Validación de Program.cs
foreach ($svc in $Microservicios) {
    $nombre = $svc.Nombre
    $programPath = Join-Path $BasePath "$($svc.Ruta)\Program.cs"
    Write-Host "`n[$nombre] - Validando Program.cs"
    if (!(Test-Path $programPath)) {
        Write-Host "FALTA: Program.cs no encontrado"
        continue
    }
    $contenido = Get-Content $programPath -Raw
    if ($contenido -match 'UseUrls\s*\(\s*"http://\*:80"\s*\)') { Write-Host "OK: Usa puerto 80" } else { Write-Host "FALTA: UseUrls(\"http://*:80\")" }
    if ($contenido -match 'AddHealthChecks\s*\(') { Write-Host "OK: AddHealthChecks registrado" } else { Write-Host "FALTA: AddHealthChecks()" }
    if ($contenido -match 'MapHealthChecks\s*\(\s*"/health"\s*\)') { Write-Host "OK: MapHealthChecks(\"/health\")" } else { Write-Host "FALTA: MapHealthChecks(\"/health\")" }
    if ($contenido -match 'UseRouting\s*\(') { Write-Host "OK: Usa UseRouting()" } else { Write-Host "FALTA: UseRouting()" }
    if ($contenido -match 'MapControllers\s*\(') { Write-Host "OK: Usa MapControllers()" } else { Write-Host "FALTA: MapControllers()" }
}