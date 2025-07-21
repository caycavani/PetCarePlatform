param (
    [string]$BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Pets",
    [string]$SolutionName = "PetCarePlatform",
    [string]$ServiceName = "Pets"
)

$servicePrefix = "PetCare.$ServiceName"
$folders = @("Api", "Application", "Domain", "Infrastructure")

Write-Host "`n[+] Generando microservicio '$ServiceName'..." -ForegroundColor Cyan

# Crear proyectos
foreach ($folder in $folders) {
    $projectPath = Join-Path $BasePath "$servicePrefix.$folder"
    dotnet new classlib -n "$servicePrefix.$folder" -o $projectPath | Out-Null
    Write-Host "[OK] Proyecto creado: $servicePrefix.$folder"
}

# Crear proyecto API como Web API
$apiPath = Join-Path $BasePath "$servicePrefix.Api"
Remove-Item "$apiPath\Class1.cs" -ErrorAction SilentlyContinue
dotnet new webapi -n "$servicePrefix.Api" -o $apiPath --no-https | Out-Null
Write-Host "[OK] Proyecto API generado: $servicePrefix.Api"

# Ruta de la solución
$solutionPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\PetCarePlatform.sln"


# Verificar que la solución exista
if (-not (Test-Path $solutionPath)) {
    Write-Host "[ERROR] La solución '$SolutionName.sln' no existe en $BasePath" -ForegroundColor Red
    exit 1
}

# Agregar proyectos a la solución
foreach ($folder in $folders) {
    $projPath = Join-Path $BasePath "$servicePrefix.$folder\$servicePrefix.$folder.csproj"
    dotnet sln $solutionPath add $projPath | Out-Null
    Write-Host "[OK] Agregado a solución: $servicePrefix.$folder"
}

$apiProj = Join-Path $BasePath "$servicePrefix.Api\$servicePrefix.Api.csproj"
dotnet sln $solutionPath add $apiProj | Out-Null
Write-Host "[OK] Agregado a solución: $servicePrefix.Api"

# Agregar referencias entre proyectos
dotnet add "$apiPath" reference `
    (Join-Path $BasePath "$servicePrefix.Application\$servicePrefix.Application.csproj") `
    (Join-Path $BasePath "$servicePrefix.Domain\$servicePrefix.Domain.csproj") | Out-Null

dotnet add "$BasePath\$servicePrefix.Application" reference `
    (Join-Path $BasePath "$servicePrefix.Domain\$servicePrefix.Domain.csproj") | Out-Null

dotnet add "$BasePath\$servicePrefix.Infrastructure" reference `
    (Join-Path $BasePath "$servicePrefix.Domain\$servicePrefix.Domain.csproj") | Out-Null

# Crear carpetas base
$domainPath = Join-Path $BasePath "$servicePrefix.Domain"
New-Item -ItemType Directory -Path "$domainPath\Entities","$domainPath\Repositories" -Force | Out-Null

$applicationPath = Join-Path $BasePath "$servicePrefix.Application"
New-Item -ItemType Directory -Path "$applicationPath\UseCases" -Force | Out-Null

# Crear archivo Pet.cs
$petFilePath = Join-Path $domainPath "Entities\Pet.cs"
$petContent = @"
namespace $servicePrefix.Domain.Entities;

public class Pet
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Species { get; private set; }
    public string Breed { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Guid OwnerId { get; private set; }

    public Pet(Guid id, string name, string species, string breed, DateTime birthDate, Guid ownerId)
    {
        Id = id;
        Name = name;
        Species = species;
        Breed = breed;
        BirthDate = birthDate;
        OwnerId = ownerId;
    }

    public int GetAgeInYears()
    {
        return (int)((DateTime.UtcNow - BirthDate).TotalDays / 365.25);
    }
}
"@
Set-Content -Path $petFilePath -Value $petContent -Encoding UTF8

# Crear archivo IPetRepository.cs
$repoFilePath = Join-Path $domainPath "Repositories\IPetRepository.cs"
$repoContent = @"
using $servicePrefix.Domain.Entities;

namespace $servicePrefix.Domain.Repositories;

public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(Guid id);
    Task<IEnumerable<Pet>> GetByOwnerIdAsync(Guid ownerId);
    Task AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(Guid id);
}
"@
Set-Content -Path $repoFilePath -Value $repoContent -Encoding UTF8

# Crear archivo RegisterPetUseCase.cs
$useCaseFilePath = Join-Path $applicationPath "UseCases\RegisterPetUseCase.cs"
$useCaseContent = @"
using $servicePrefix.Domain.Entities;
using $servicePrefix.Domain.Repositories;

namespace $servicePrefix.Application.UseCases;

public class RegisterPetUseCase
{
    private readonly IPetRepository _repository;

    public RegisterPetUseCase(IPetRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(string name, string species, string breed, DateTime birthDate, Guid ownerId)
    {
        var pet = new Pet(Guid.NewGuid(), name, species, breed, birthDate, ownerId);
        await _repository.AddAsync(pet);
        return pet.Id;
    }
}
"@
Set-Content -Path $useCaseFilePath -Value $useCaseContent -Encoding UTF8

Write-Host "`n[OK] Microservicio '$ServiceName' generado y listo para desarrollar." -ForegroundColor Green