using MediatR;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Customers;

namespace NotificationService.Application.Customers.UpsertNotificationCustomer
{
    public sealed class UpsertNotificationCustomerHandler : IRequestHandler<UpsertNotificationCustomerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationCustomerRepository _customerRepository;

        public UpsertNotificationCustomerHandler(IUnitOfWork unitOfWork, INotificationCustomerRepository customerRepository)
        {
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
        }

        public async Task Handle(UpsertNotificationCustomerCommand request, CancellationToken ct)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(request.UserId, ct);

            if (existingCustomer == null)
            {
                existingCustomer = NotificationCustomer.Create(request.UserId, request.Name, request.Email);
                await _customerRepository.AddAsync(existingCustomer, ct);
            }
            else
            {
                existingCustomer.Update(request.Name, request.Email);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
