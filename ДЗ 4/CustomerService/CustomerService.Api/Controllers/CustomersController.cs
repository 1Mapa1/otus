using AutoMapper;
using CustomerService.Api.Contracts.Requests;
using CustomerService.Api.Contracts.Responses;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> Get(CancellationToken ct)
        {
            var customers = await _repository.GetAllAsync(ct);

            return Ok(_mapper.Map<IEnumerable<CustomerResponse>>(customers));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var customer = await _repository.GetByIdAsync(id, ct);

            if (customer == null)
                return NotFound();

            return Ok(_mapper.Map<CustomerResponse>(customer));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomerRequest request, CancellationToken ct)
        {
            var customer = _mapper.Map<Customer>(request);

            var id = await _repository.AddAsync(customer, ct);

            var response = _mapper.Map<CustomerResponse>(customer);

            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken ct)
        {
            var customer = await _repository.GetByIdAsync(id, ct);

            if (customer == null)
                return NotFound();

            customer.SetName(request.Name);
            customer.SetEmail(request.Email);
            customer.SetDateOfBirth(request.DateOfBirth);

            await _repository.UpdateAsync(customer, ct);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _repository.DeleteAsync(id, ct);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
