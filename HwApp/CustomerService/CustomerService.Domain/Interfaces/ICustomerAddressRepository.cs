using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Interfaces
{
    public interface ICustomerAddressRepository
    {
        Task AddAsync(CustomerAddress customerAddress, CancellationToken ct = default);

        Task<IReadOnlyList<CustomerAddress>> GetByCustomerIdAsync(Guid customerId, bool? active = null, CancellationToken ct = default);

        Task<CustomerAddress?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task UpdateAsync(CustomerAddress customer, CancellationToken ct = default);
    }
}
