using AutoMapper;
using CustomerService.Api.Contracts.Requests;
using CustomerService.Api.Contracts.Responses;
using CustomerService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Api.Controllers.External
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CustomerResponse>> GetMe(CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var id))
                return Unauthorized();

            var customer = await _repository.GetByIdAsync(id, ct);

            if (customer == null)
                return NotFound();

            return Ok(_mapper.Map<CustomerResponse>(customer));
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateCustomerRequest request, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var id))
                return Unauthorized();

            var customer = await _repository.GetByIdAsync(id, ct);

            if (customer == null)
                return NotFound();

            customer.SetName(request.Name);
            customer.SetEmail(request.Email);
            customer.SetDateOfBirth(request.DateOfBirth);

            await _repository.UpdateAsync(customer, ct);

            return NoContent();
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var idClaim = User.Identity?.Name;
            return Guid.TryParse(idClaim, out userId);
        }
    }
}
