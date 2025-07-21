<#
.SYNOPSIS
  Crea clase de configuración para registrar mocks en el contenedor de dependencias.
#>

function Write-File {
  param($Path, $Content)
  $dir = Split-Path $Path
  if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
  Set-Content -Path $Path -Value $Content -Encoding UTF8
}

$infra = "Payment\PetCare.Payment.Infrastructure"

Write-File "$infra\Configuration\PaymentModuleMockConfiguration.cs" @"
using Microsoft.Extensions.DependencyInjection;
using PetCare.Payment.Domain.Repositories;
using PetCare.Payment.Domain.Services;
using PetCare.Payment.Infrastructure.Repositories;
using PetCare.Payment.Infrastructure.Services;

namespace PetCare.Payment.Infrastructure.Configuration;

public static class PaymentModuleMockConfiguration
{
    public static IServiceCollection AddPaymentMocks(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
        services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
        return services;
    }
}
"@

Write-Host ""
Write-Host "Registro de mocks generado exitosamente." -ForegroundColor Green
Write-Host "Ubicacion: $infra\\Configuration" -ForegroundColor Cyan