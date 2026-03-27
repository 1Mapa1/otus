using CustomerService.Domain.Entities;

namespace CustomerService.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default);

        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<Guid> AddAsync(Customer customer, CancellationToken ct = default);

        Task UpdateAsync(Customer customer, CancellationToken ct = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
