$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform"
$microservicios = @(
    @{ Nombre = "auth-api"; Ruta = "Auth/PetCare.Auth.Api" },
    @{ Nombre = "booking-api"; Ruta = "Booking/PetCare.Booking.Api" },
    @{ Nombre = "notification-api"; Ruta = "Notification/PetCare.Notification.Api" },
    @{ Nombre = "payment-api"; Ruta = "Payment/PetCare.Payment.Api" },
    @{ Nombre = "pets-api"; Ruta = "Pets/PetCare.Pets.Api" },
    @{ Nombre = "review-api"; Ruta = "Review/PetCare.Review.Api" }
)

foreach ($svc in $microservicios) {
    $nombre = $svc.Nombre
    $programPath = Join-Path $basePath "$($svc.Ruta)\Program.cs"

    Write-Host "`n[$nombre] - Program.cs"

    if (!(Test-Path $programPath)) {
        Write-Host "❌ Program.cs no encontrado"
        continue
    }

    $contenido = Get-Content $programPath -Raw

    if ($contenido -match 'AddHealthChecks\s*\(') {
        Write-Host "✅ AddHealthChecks registrado"
    } else {
        Write-Host "⚠️ FALTA: AddHealthChecks()"
    }

    if ($contenido -match 'MapHealthChecks\s*\(\s*"/health"\s*\)') {
        Write-Host "✅ MapHealthChecks('/health') registrado"
    } else {
        Write-Host "⚠️ FALTA: MapHealthChecks('/health')"
    }

    if ($contenido -match 'UseRouting\s*\(') {
        Write-Host "✅ Usa UseRouting()"
    } else {
        Write-Host "⚠️ FALTA: UseRouting()"
    }

    if ($contenido -match 'MapControllers\s*\(') {
        Write-Host "✅ Usa MapControllers()"
    } else {
        Write-Host "⚠️ FALTA: MapControllers()"
    }
}