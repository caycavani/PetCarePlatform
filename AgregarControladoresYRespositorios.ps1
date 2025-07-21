param (
    [string]$BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Review"
)

$service = "Review"
$entity = "Review"
$useCase = "SubmitReviewUseCase"
$repoInterface = "IReviewRepository"
$params = "Guid BookingId, int Rating, string Comment"

$prefix = "PetCare.$service"
$apiPath = Join-Path $BasePath "$prefix.Api"
$infraPath = Join-Path $BasePath "$prefix.Infrastructure\Repositories"
$controllerPath = Join-Path $apiPath "Controllers"
$programFile = Join-Path $apiPath "Program.cs"

# Crear carpetas si no existen
if (-not (Test-Path $infraPath)) { New-Item -ItemType Directory -Path $infraPath -Force | Out-Null }
if (-not (Test-Path $controllerPath)) { New-Item -ItemType Directory -Path $controllerPath -Force | Out-Null }

# Crear repositorio en memoria
$repoClass = "InMemory$($entity)Repository"
$repoFile = Join-Path $infraPath "$repoClass.cs"
$repoCode = @"
using $prefix.Domain.Entities;
using $prefix.Domain.Repositories;

namespace $prefix.Infrastructure.Repositories;

public class $repoClass : $repoInterface
{
    private readonly List<Review> _store = new();

    public Task AddAsync(Review review)
    {
        _store.Add(review);
        return Task.CompletedTask;
    }

    public Task<Review?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_store.FirstOrDefault(x => x.Id == id));
    }

    public Task<IEnumerable<Review>> GetByBookingIdAsync(Guid bookingId)
    {
        return Task.FromResult(_store.Where(x => x.BookingId == bookingId));
    }
}
"@
Set-Content -Path $repoFile -Value $repoCode -Encoding UTF8

# Crear controlador
$controllerFile = Join-Path $controllerPath "$service`Controller.cs"
$controllerCode = @"
using Microsoft.AspNetCore.Mvc;
using $prefix.Application.UseCases;

namespace $prefix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class $service`Controller : ControllerBase
{
    private readonly $useCase _useCase;

    public $service`Controller($useCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Request request)
    {
        var id = await _useCase.ExecuteAsync(request.BookingId, request.Rating, request.Comment);
        return Ok(new { Id = id });
    }

    public record Request($params);
}
"@
Set-Content -Path $controllerFile -Value $controllerCode -Encoding UTF8

# Registrar en Program.cs
if (Test-Path $programFile) {
    $lines = Get-Content $programFile
    $registration = @"
builder.Services.AddScoped<$prefix.Domain.Repositories.$repoInterface, $prefix.Infrastructure.Repositories.$repoClass>();
builder.Services.AddScoped<$prefix.Application.UseCases.$useCase>();
"@ -split "`n"

    if (-not ($lines -join "`n" -match $repoClass)) {
        $insertIndex = ($lines | Select-String "AddControllers" -SimpleMatch).LineNumber
        $lines = $lines[0..($insertIndex - 1)] + $registration + $lines[$insertIndex..($lines.Length - 1)]
        Set-Content -Path $programFile -Value $lines -Encoding UTF8
        Write-Host "[OK] Registrado en Program.cs: $service"
    }
}

Write-Host "`n✅ Controlador y repositorio en memoria generados para $service." -ForegroundColor Green