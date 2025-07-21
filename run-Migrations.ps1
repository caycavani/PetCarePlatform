$Service = "Payment"
$MigrationName = "InitialCreate"
$csprojInfra = ".\PetCare.$Service.Infrastructure\PetCare.$Service.Infrastructure.csproj"
$csprojApi = ".\PetCare.$Service.Api\PetCare.$Service.Api.csproj"

Push-Location ".\PetCare.$Service.Api"
try {
    dotnet ef migrations add $MigrationName `
        --project $csprojInfra `
        --startup-project $csprojApi `
        --output-dir "Persistence/Migrations"

    dotnet ef database update `
        --project $csprojInfra `
        --startup-project $csprojApi

    Write-Host "Migraciones aplicadas correctamente para ${Service}"
} catch {
    Write-Host "ERROR: Falló la migración para ${Service}: $($_.Exception.Message)"
}
Pop-Location