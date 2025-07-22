# Ruta donde está el proyecto Auth.Api
$projectPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Auth\PetCare.Auth.Api"

# Puerto HTTP deseado
$port = 5001

# Establecer variable de entorno local
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Ejecutar el microservicio sin HTTPS
dotnet run --project $projectPath --urls "http://localhost:$port"
