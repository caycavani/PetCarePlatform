param (
    [string]$BasePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Notification",
    [string]$ServiceName = "Notification"
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

# Notification.cs
Set-Content "$domainPath\Entities\Notification.cs" @"
namespace $servicePrefix.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public string Recipient { get; private set; }
    public string Message { get; private set; }
    public DateTime SentAt { get; private set; }

    public Notification(Guid id, string recipient, string message, DateTime sentAt)
    {
        Id = id;
        Recipient = recipient;
        Message = message;
        SentAt = sentAt;
    }
}
"@ -Encoding UTF8

# INotificationRepository.cs
Set-Content "$domainPath\Repositories\INotificationRepository.cs" @"
using $servicePrefix.Domain.Entities;

namespace $servicePrefix.Domain.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
}
"@ -Encoding UTF8

# SendNotificationUseCase.cs
Set-Content "$applicationPath\UseCases\SendNotificationUseCase.cs" @"
using $servicePrefix.Domain.Entities;
using $servicePrefix.Domain.Repositories;

namespace $servicePrefix.Application.UseCases;

public class SendNotificationUseCase
{
    private readonly INotificationRepository _repository;

    public SendNotificationUseCase(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(string recipient, string message)
    {
        var notification = new Notification(Guid.NewGuid(), recipient, message, DateTime.UtcNow);
        await _repository.AddAsync(notification);
        return notification.Id;
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

// TODO: Registrar implementación real de INotificationRepository
builder.Services.AddScoped<SendNotificationUseCase>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
"@ -Encoding UTF8

Write-Host "`n[OK] Microservicio '$ServiceName' generado y agregado completamente a la solución." -ForegroundColor Green