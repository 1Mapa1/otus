using AutoMapper;
using CustomerService.Api.Contracts.Requests;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Api.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/customers")]
    public class InternalCustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repository;

        public InternalCustomersController(ICustomerRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomerRequest request, CancellationToken ct)
        {
            var existingCustomer = await _repository.GetByIdAsync(request.Id, ct);

            if (existingCustomer is not null)
                return Ok();

            var customer = Customer.Create(request.Id, request.Name, request.Email, request.DateOfBirth);

            await _repository.AddAsync(customer, ct);

            return Ok();
        }
    }
}
