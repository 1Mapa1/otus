using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Orders.Saga;
using OrderService.Application.Orders.Saga.Steps;

namespace OrderService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddScoped<IOrderSagaProcessor, OrderSagaProcessor>();

            services.AddScoped<IOrderSagaStepHandler, AuthorizePaymentStepHandler>();
            services.AddScoped<IOrderSagaStepHandler, ReserveStockStepHandler>();
            services.AddScoped<IOrderSagaStepHandler, ReserveDeliveryStepHandler>();
            services.AddScoped<IOrderSagaStepHandler, CapturePaymentStepHandler>();
            services.AddScoped<IOrderSagaStepHandler, CompensatingStepHandler>();

            return services;
        }
    }
}