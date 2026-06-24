using AutoMapper;
using CustomerService.Api.Contracts.Requests;
using CustomerService.Api.Contracts.Responses;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CustomerService.Api.Controllers.External
{
    [ApiController]
    [Route("api/customers/me/addresses")]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ICustomerAddressRepository _repository;
        private readonly IMapper _mapper;

        public CustomerAddressesController(ICustomerAddressRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerAddressResponse>>> Get(CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var id))
                return Unauthorized();

            var addresses = await _repository.GetByCustomerIdAsync(id, true, ct);

            if (addresses == null)
                return NotFound();

            return Ok(_mapper.Map<IReadOnlyList<CustomerAddressResponse>>(addresses));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpsertCustomerAddressRequest request, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var id))
                return Unauthorized();

            var address = CustomerAddress.Create(id, request.City, request.Street, request.House, request.Apartment);

            await _repository.AddAsync(address, ct);

            var response = _mapper.Map<CustomerAddressResponse>(address);

            return CreatedAtRoute(
                routeName: null,
                routeValues: new { addressId = address.Id },
                value: response);
        }

        [Authorize]
        [HttpPut("{addressId:guid}")]
        public async Task<IActionResult> Update([FromBody] UpsertCustomerAddressRequest request, [FromRoute] Guid addressId, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var id))
                return Unauthorized();

            var address = await _repository.GetByIdAsync(addressId, ct);

            if (address == null || address.CustomerId != id || !address.IsActive)
                return NotFound();

            address.Update(request.City, request.Street, request.House, request.Apartment);

            await _repository.UpdateAsync(address, ct);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{addressId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid addressId, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var id))
                return Unauthorized();

            var address = await _repository.GetByIdAsync(addressId, ct);

            if (address == null || address.CustomerId != id)
                return NotFound();

            if (!address.IsActive)
                return NoContent();

            address.Deactivate();

            await _repository.UpdateAsync(address, ct);

            return NoContent();
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var idClaim = User.FindFirstValue("sub")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.Identity?.Name;

            return Guid.TryParse(idClaim, out userId);
        }
    }
}
