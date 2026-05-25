using MediatR;
using OrderService.Application.Abstractions;
using OrderService.Application.Billing;
using OrderService.Domain.Orders;

namespace OrderService.Application.Orders.CreateOrder
{
    internal sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBillingServiceClient _billingServiceClient;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IOrderRepository orderRepository, IBillingServiceClient billingServiceClient, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _billingServiceClient = billingServiceClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(request.UserId, request.Price);

            await _orderRepository.AddAsync(order, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var billingResult = await _billingServiceClient.WithdrawAsync(order.UserId, order.Id, order.Price, cancellationToken);

            switch (billingResult.Status)
            {
                case BillingWithdrawStatus.Success:
                    order.MarkAsPaid();
                    break;
                case BillingWithdrawStatus.InsufficientFunds:
                    order.MarkAsRejected(OrderFailureReason.InsufficientFunds);
                    break;
                case BillingWithdrawStatus.AccountNotFound:
                    order.MarkAsRejected(OrderFailureReason.AccountNotFound);
                    break;
                case BillingWithdrawStatus.InvalidAmount:
                    order.MarkAsRejected(OrderFailureReason.InvalidAmount);
                    break;
                default:
                    order.MarkAsRejected(OrderFailureReason.UnknownError);
                    break;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateOrderResult(order.Id, order.Status, order.FailureReason);
        }
    }
}
