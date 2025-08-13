# 🌐 Ruta al microservicio Auth
$authPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Auth\PetCare.Auth.Api"
$authPort = 5001

# 📅 Ruta al microservicio Booking
$bookingPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Booking\PetCare.Booking.Api"
$bookingPort = 5003

# 🧪 Establecer entorno común
$env:ASPNETCORE_ENVIRONMENT = "Development"

# 🧵 Ejecutar Auth
Start-Job -ScriptBlock {
    param($path, $port)
    dotnet run --project $path --urls "http://localhost:$port"
} -ArgumentList $authPath, $authPort

# 🧵 Ejecutar Booking
Start-Job -ScriptBlock {
    param($path, $port)
    dotnet run --project $path --urls "http://localhost:$port"
} -ArgumentList $bookingPath, $bookingPort

Write-Host "✅ Microservicios Auth y Booking corriendo en paralelo:"
Write-Host "- Auth en http://localhost:$authPort"
Write-Host "- Booking en http://localhost:$bookingPort"
