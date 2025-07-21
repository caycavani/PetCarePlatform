$basePath = "C:\Proyecto App Movil Cuidadores Pets\Backend\PetCarePlatform\Application\UseCases\CrearReserva"
New-Item -ItemType Directory -Path $basePath -Force | Out-Null

# Crear CrearReservaCommand.cs
@'
namespace Application.UseCases.CrearReserva;

public class CrearReservaCommand
{
    public Guid IdUsuario { get; set; }
    public Guid IdCuidador { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string TipoServicio { get; set; } = default!;
    public string NotasAdicionales { get; set; } = default!;
}
'@ | Set-Content -Path "$basePath\CrearReservaCommand.cs" -Encoding UTF8

# Crear CrearReservaCommandValidator.cs
@'
using FluentValidation;

namespace Application.UseCases.CrearReserva;

public class CrearReservaCommandValidator : AbstractValidator<CrearReservaCommand>
{
    public CrearReservaCommandValidator()
    {
        RuleFor(x => x.IdUsuario).NotEmpty();
        RuleFor(x => x.IdCuidador).NotEmpty();
        RuleFor(x => x.TipoServicio).NotEmpty();
        RuleFor(x => x.FechaInicio).LessThan(x => x.FechaFin);
        RuleFor(x => x.NotasAdicionales).MaximumLength(500);
    }
}
'@ | Set-Content -Path "$basePath\CrearReservaCommandValidator.cs" -Encoding UTF8

Write-Host "✅ Componente CrearReservaCommand y su validador generados en: $basePath" -ForegroundColor Green