<#
.SYNOPSIS
  Automatiza la creación del flujo de pagos: caso de uso, DTOs, interfaces y entidad.
#>

function Write-File {
  param($Path, $Content)
  $dir = Split-Path $Path
  if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
  Set-Content -Path $Path -Value $Content -Encoding UTF8
}

$base = "Payment"
$app = "$base\PetCare.Payment.Application"
$domain = "$base\PetCare.Payment.Domain"

# 1. DTOs
Write-File "$app\DTOs\PaymentRequest.cs" @"
namespace PetCare.Payment.Application.DTOs;

public class PaymentRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
}
"@

Write-File "$app\DTOs\PaymentResponse.cs" @"
namespace PetCare.Payment.Application.DTOs;

public class PaymentResponse
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
"@

# 2. Interfaces
Write-File "$app\Contracts\IUseCase.cs" @"
namespace PetCare.Payment.Application.Contracts;

public interface IUseCase<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}
"@

Write-File "$domain\Repositories\IPaymentRepository.cs" @"
using PetCare.Payment.Domain.Entities;

namespace PetCare.Payment.Domain.Repositories;

public interface IPaymentRepository
{
    Task SaveAsync(Payment payment);
}
"@

Write-File "$domain\Services\IPaymentGateway.cs" @"
using PetCare.Payment.Domain.Entities;

namespace PetCare.Payment.Domain.Services;

public interface IPaymentGateway
{
    Task<PaymentResult> ProcessAsync(Payment payment);
}
"@

# 3. Entidad de dominio
Write-File "$domain\Entities\Payment.cs" @"
namespace PetCare.Payment.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Method { get; private set; }

    public Payment(Guid userId, decimal amount, string method)
    {
        UserId = userId;
        Amount = amount;
        Method = method;
    }
}
"@

Write-File "$domain\Entities\PaymentResult.cs" @"
namespace PetCare.Payment.Domain.Entities;

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
"@

# 4. Caso de uso
Write-File "$app\UseCases\ProcessPaymentUseCase.cs" @"
using PetCare.Payment.Application.Contracts;
using PetCare.Payment.Application.DTOs;
using PetCare.Payment.Domain.Entities;
using PetCare.Payment.Domain.Repositories;
using PetCare.Payment.Domain.Services;

namespace PetCare.Payment.Application.UseCases;

public class ProcessPaymentUseCase : IUseCase<PaymentRequest, PaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;

    public ProcessPaymentUseCase(IPaymentRepository paymentRepository, IPaymentGateway paymentGateway)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<PaymentResponse> ExecuteAsync(PaymentRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("El monto debe ser mayor a cero.");

        var payment = new Payment(request.UserId, request.Amount, request.Method);
        var result = await _paymentGateway.ProcessAsync(payment);

        if (result.IsSuccess)
            await _paymentRepository.SaveAsync(payment);

        return new PaymentResponse
        {
            Success = result.IsSuccess,
            TransactionId = result.TransactionId,
            Message = result.Message
        };
    }
}
"@

Write-Host "`n✅ Flujo de pagos generado exitosamente." -ForegroundColor Green
Write-Host "📂 Ubicación base: $base" -ForegroundColor Cyan