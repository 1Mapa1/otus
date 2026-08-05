using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Infrastructure.Repositories
{
    internal class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly DatabaseContext _context;

        public CustomerAddressRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CustomerAddress customerAddress, CancellationToken ct = default)
        {
            await _context.AddAsync(customerAddress, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<CustomerAddress>> GetByCustomerIdAsync(Guid customerId, bool? active = null, CancellationToken ct = default)
            => await _context.CustomerAddresses
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId && (!active.HasValue || c.IsActive == active.Value))
            .ToListAsync(ct);

        public async Task<CustomerAddress?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.CustomerAddresses.FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task UpdateAsync(CustomerAddress customer, CancellationToken ct = default)
        {
            _context.Entry(customer).State = EntityState.Modified;

            await _context.SaveChangesAsync(ct);
        }
    }
}
