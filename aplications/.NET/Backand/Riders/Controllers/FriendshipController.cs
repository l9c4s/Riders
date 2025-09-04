using Aplication.Contracts;
using Domain.Dto.Friendship;
using Domain.Dto.Generics;
using Domain.Dto.RequestResultsDto;
using Domain.Enum;
using Domain.Utils.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Riders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipUseCases _friendshipUseCases;

        public FriendshipController(IFriendshipUseCases friendshipUseCases)
        {
            _friendshipUseCases = friendshipUseCases;
        }

        [Authorize(Policy = nameof(AccessLevel.CommonUser))]
        [HttpPost("AddFriendship")]
        public async Task<IActionResult> RequestFriendship(RequestFriendDto request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token Required");
            }

            RequestFriendshipDto requestFriendshipDto = new RequestFriendshipDto
            {
                RequesterId = Converts_Utils.StringToGuid(userId),
                AddresseeId = request.AddresseeId
            };
            var friendship = await _friendshipUseCases.RequestFriendship(requestFriendshipDto);
            return CreatedAtAction(nameof(RequestFriendship), friendship);
        }


        [Authorize(Policy = nameof(AccessLevel.CommonUser))]
        [HttpPost("RespondToFriendshipRequest")]
        public async Task<IActionResult> RespondToFriendshipRequest(RespondToFriendshipRequestDto request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token Required");
            }
            FriendRequestWithDTO friendRequestWithDto = new FriendRequestWithDTO
            {
                FriendshipId = request.FriendshipId,
                Accepted = request.Accepted,
                Guest = Converts_Utils.StringToGuid(userId) // Assuming the user responding is the guest
            };
            var friendship = await _friendshipUseCases.RespondToFriendshipRequest(friendRequestWithDto);
            return Ok(new RequestResultsDto
            {
                Message = "Friendship request responded successfully.",
                Success = true,
                Data = friendship
            });

        }


        [Authorize(Policy = nameof(AccessLevel.CommonUser))]
        [HttpGet("GetFriendships")]
        public async Task<IActionResult> GetFriendships()
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token Required");
            }
            var friendships = await _friendshipUseCases.GetFriendships(Converts_Utils.StringToGuid(userId));
            return Ok(new RequestResultsDto
            {
                Message = "List of friendships retrieved successfully.",
                Success = true,
                Data = friendships
            });
             
        }


        [Authorize(Policy = nameof(AccessLevel.CommonUser))]
        [HttpGet("GetPendingFriendshipRequests")]
        public async Task<IActionResult> GetPendingFriendshipRequests()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token Required");
            }

            var pendingRequests = await _friendshipUseCases.GetPendingFriendshipRequests(Converts_Utils.StringToGuid(userId));
            return Ok(new RequestResultsDto
            {
                Message = "List of pending friendship requests retrieved successfully.",
                Success = true,
                Data = pendingRequests
            });

        }

        [Authorize(Policy = nameof(AccessLevel.CommonUser))]
        [HttpPost("BlockedFriendUsers")]
        public async Task<IActionResult> BlockedFriendUsers(BlockFriendDto blockFriendDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _friendshipUseCases.BlockedFriendUsers(blockFriendDto, Converts_Utils.StringToGuid(userId));


            return Ok(new RequestResultsDto
            {
                Message = "Friend user blocked successfully.",
                Success = true,
                Data = result
            });

        }

        [Authorize(Policy = nameof(AccessLevel.Admin))]
        [HttpPost("DeleteFriendship")]
        public async Task<IActionResult> DeleteFriendship(GetByIdGenerics friendshipId)
        {
            
            await _friendshipUseCases.DeleteFriendship(Converts_Utils.StringToGuid(friendshipId.Id));
            return Ok(new RequestResultsDto
            {
                Message = "Friendship deleted successfully.",
                Success = true,
                Data = null
            });
        }

    }
}
