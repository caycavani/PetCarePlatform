using Microsoft.Extensions.DependencyInjection;
using PetCare.Payment.Infrastructure.Gateways.PSE;
using PetCare.Payment.Infrastructure.Gateways.Wompi;
using PetCare.Payment.Infrastructure.Gateways.Mock;

namespace PetCare.Payment.Infrastructure.Gateways
{
    public class PaymentGatewaySelector
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentGatewaySelector(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentGatewayClient Resolve(string paymentMethodId)
        {
            return paymentMethodId.ToLower() switch
            {
                "stripe" => _serviceProvider.GetRequiredService<StripeGatewayClient>() as IPaymentGatewayClient,
                "wompi" => _serviceProvider.GetRequiredService<WompiGatewayClient>() as IPaymentGatewayClient,
                "pse" => _serviceProvider.GetRequiredService<PseGatewayClient>() as IPaymentGatewayClient,
                "mock" => _serviceProvider.GetRequiredService<MockPaymentGatewayClient>() as IPaymentGatewayClient,
                _ => throw new NotSupportedException($"Método de pago no soportado: {paymentMethodId}")
            };
        }
    }
}
