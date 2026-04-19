using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<Guid> AddAsync(Customer customer, CancellationToken ct = default);

        Task UpdateAsync(Customer customer, CancellationToken ct = default);
    }
}
