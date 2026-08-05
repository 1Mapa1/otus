using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    public class WellKnownController : ControllerBase
    {
        private readonly IJwksProvider _jwksProvider;

        public WellKnownController(IJwksProvider jwksProvider)
        {
            _jwksProvider = jwksProvider;
        }

        [HttpGet("/.well-known/jwks.json")]
        public IActionResult GetJwks()
        {
            var jwks = _jwksProvider.Get();
            return Ok(jwks);
        }
    }
}
