using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Infrastructure.Repositories
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;

        public CustomerRepository(DatabaseContext context) 
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(Customer customer, CancellationToken ct = default)
        {
            await _context.AddAsync(customer, ct);
            await _context.SaveChangesAsync(ct);

            return customer.Id;
        }

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
        {
            _context.Entry(customer).State = EntityState.Modified;

            await _context.SaveChangesAsync(ct);
        }
    }
}
