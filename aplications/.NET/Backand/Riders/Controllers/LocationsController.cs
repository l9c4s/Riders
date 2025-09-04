using System.Security.Claims;
using Aplication.Contracts;
using Domain.Dto.Generics;
using Domain.Dto.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Riders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsUseCases _locationUseCases;

        public LocationsController(ILocationsUseCases locationUseCases)
        {
            _locationUseCases = locationUseCases;
        }

        [HttpPost("SendLocations")]
        public async Task<IActionResult> SendLocations(UpdateLocationDto locationDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _locationUseCases.UpdateUserLocationAsync(userId, locationDto.Latitude, locationDto.Longitude);
            return Ok(new { message = "Location updated successfully." });
        }

        [HttpPost("GrantShare")]
        public async Task<IActionResult> GrantShare(GetByIdGenerics observerId)
        {
            var sharerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(sharerId)) return Unauthorized();

            await _locationUseCases.GrantLocationShareAsync(sharerId, observerId.Id);
            return Ok(new { message = "Location sharing granted." });
        }

        [HttpPost("RevokeShare")]
        public async Task<IActionResult> RevokeShare(GetByIdGenerics observerId)
        {
            var sharerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(sharerId)) return Unauthorized();

            await _locationUseCases.RevokeLocationShareAsync(sharerId, observerId.Id);
            return Ok(new { message = "Location sharing revoked." });
        }

    }
}
