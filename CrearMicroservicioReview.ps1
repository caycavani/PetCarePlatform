param (
    [string]$BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Review",
    [string]$ServiceName = "Review"
)

$solutionPath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\PetCarePlatform.sln"
$servicePrefix = "PetCare.$ServiceName"
$folders = @("Api", "Application", "Domain", "Infrastructure")

Write-Host "`n[+] Generando microservicio '$ServiceName'..." -ForegroundColor Cyan

# Crear proyectos
foreach ($folder in $folders) {
    $projectPath = Join-Path $BasePath "$servicePrefix.$folder"
    dotnet new classlib -n "$servicePrefix.$folder" -o $projectPath | Out-Null
    Write-Host "[OK] Proyecto creado: $servicePrefix.$folder"
}

# Crear proyecto API
$apiPath = Join-Path $BasePath "$servicePrefix.Api"
dotnet new webapi -n "$servicePrefix.Api" -o $apiPath --no-https | Out-Null
Remove-Item "$apiPath\WeatherForecast.cs","$apiPath\Controllers\WeatherForecastController.cs" -ErrorAction SilentlyContinue
Write-Host "[OK] Proyecto API generado: $servicePrefix.Api"

# Verificar solución
if (-not (Test-Path $solutionPath)) {
    Write-Host "[ERROR] La solución no existe en: $solutionPath" -ForegroundColor Red
    exit 1
}

# Agregar proyectos a la solución
foreach ($folder in $folders) {
    $projPath = Join-Path $BasePath "$servicePrefix.$folder\$servicePrefix.$folder.csproj"
    if (Test-Path $projPath) {
        Write-Host "→ Agregando a solución: $projPath"
        & dotnet sln "$solutionPath" add "$projPath"
    } else {
        Write-Host "[ERROR] No se encontró el proyecto: $projPath" -ForegroundColor Red
    }
}

$apiProj = Join-Path $BasePath "$servicePrefix.Api\$servicePrefix.Api.csproj"
if (Test-Path $apiProj) {
    Write-Host "→ Agregando a solución: $apiProj"
    & dotnet sln "$solutionPath" add "$apiProj"
} else {
    Write-Host "[ERROR] No se encontró el proyecto API: $apiProj" -ForegroundColor Red
}

# Referencias entre proyectos
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

# Review.cs
Set-Content "$domainPath\Entities\Review.cs" @"
namespace $servicePrefix.Domain.Entities;

public class Review
{
    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Review(Guid id, Guid bookingId, int rating, string comment, DateTime createdAt)
    {
        Id = id;
        BookingId = bookingId;
        Rating = rating;
        Comment = comment;
        CreatedAt = createdAt;
    }
}
"@ -Encoding UTF8

# IReviewRepository.cs
Set-Content "$domainPath\Repositories\IReviewRepository.cs" @"
using $servicePrefix.Domain.Entities;

namespace $servicePrefix.Domain.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id);
    Task<IEnumerable<Review>> GetByBookingIdAsync(Guid bookingId);
    Task AddAsync(Review review);
}
"@ -Encoding UTF8

# SubmitReviewUseCase.cs
Set-Content "$applicationPath\UseCases\SubmitReviewUseCase.cs" @"
using $servicePrefix.Domain.Entities;
using $servicePrefix.Domain.Repositories;

namespace $servicePrefix.Application.UseCases;

public class SubmitReviewUseCase
{
    private readonly IReviewRepository _repository;

    public SubmitReviewUseCase(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(Guid bookingId, int rating, string comment)
    {
        var review = new Review(Guid.NewGuid(), bookingId, rating, comment, DateTime.UtcNow);
        await _repository.AddAsync(review);
        return review.Id;
    }
}
"@ -Encoding UTF8

# Program.cs básico
Set-Content "$apiPath\Program.cs" @"
using $servicePrefix.Application.UseCases;
using $servicePrefix.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Registrar implementación real de IReviewRepository
builder.Services.AddScoped<SubmitReviewUseCase>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
"@ -Encoding UTF8

Write-Host "`n[OK] Microservicio '$ServiceName' generado y agregado completamente a la solución." -ForegroundColor Green