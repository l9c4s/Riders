using Aplication.Contracts;
using Domain.Contracts;
using Domain.Dto.Friendship;
using Domain.Entities;
using Domain.Enum;
using Domain.Utils.Mappers.Friends;

namespace Aplication.UseCases.Friendship
{
    public class FriendshipCasesImplements : IFriendshipUseCases
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;

        public FriendshipCasesImplements(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
        }

        public async Task<RequestFriendshipDto> RequestFriendship(RequestFriendshipDto request)
        {
            var addressee = await _userRepository.GetByIdAsync(request.AddresseeId);

            if (addressee == null)
                throw new ArgumentException("User not found");

            var existingFriendship = await _friendshipRepository.FirstOrDefaultAsync(f =>
                (f.RequesterId == request.RequesterId && f.AddresseeId == request.AddresseeId) ||
                (f.RequesterId == request.AddresseeId && f.AddresseeId == request.RequesterId));

            if (existingFriendship != null)
                throw new ArgumentException("Friendship request already exists");

            var friendship = new Domain.Entities.Friendship
            {
                RequesterId = request.RequesterId,
                AddresseeId = request.AddresseeId,
                Status = FriendshipStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _friendshipRepository.AddAsync(friendship);
            return request;
        }

        public async Task<RespondToFriendshipRequestDto> RespondToFriendshipRequest(FriendRequestWithDTO request)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.FriendshipId);

            if (friendship == null)
                throw new KeyNotFoundException("Friendship request not found");

            if (request.Guest.CompareTo(friendship.AddresseeId) != 0)
                throw new UnauthorizedAccessException("Unauthorized");

            if (friendship.Status != FriendshipStatus.Pending)
                throw new AppDomainUnloadedException("Friendship request already responded");

            friendship.Status = request.Accepted ? FriendshipStatus.Accepted : FriendshipStatus.Rejected;
            friendship.RespondedAt = DateTime.UtcNow;

            await _friendshipRepository.UpdateAsync(friendship);
            return request;
        }

        public async Task<IEnumerable<ListFriendsDto>> GetFriendships(Guid userId)
        {
            IEnumerable<Domain.Entities.Friendship> friendships = await _friendshipRepository.FindAsync(f => (f.RequesterId == userId || f.AddresseeId == userId) && (f.Status == FriendshipStatus.Accepted || f.Status == FriendshipStatus.Pending));
            if (friendships == null || !friendships.Any())
                throw new KeyNotFoundException("No friendships found for the user");

            // Map the friendships to DTOs
            IEnumerable<ListFriendsDto> ListFriendsDtos = FriendshipMapper.ToDto(friendships);
            return ListFriendsDtos;
        }

        public async Task<IEnumerable<ListFriendsDto>> GetPendingFriendshipRequests(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));


            var RequestedFriendships = await _friendshipRepository.GetFriendRequests(f => f.AddresseeId == userId && f.Status == FriendshipStatus.Pending);
            if (RequestedFriendships == null || !RequestedFriendships.Any())
            {
                throw new KeyNotFoundException("No pending friendship requests found");
            }
            // Map the friendships to DTOs
            IEnumerable<ListFriendsDto> RequestedFriendshipsMap = FriendshipMapper.ToDtoFriendRequests(RequestedFriendships);

            return RequestedFriendshipsMap;
        }

        public async Task DeleteFriendship(Guid friendshipId)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(friendshipId);

            if (friendship == null)
                throw new Exception("Friendship not found");

            await _friendshipRepository.DeleteAsync(friendship.Id);
        }

        public async Task<bool> BlockedFriendUsers(BlockFriendDto blockFriendDto, Guid userId)
        {
            if (blockFriendDto == null)
                throw new ArgumentNullException(nameof(blockFriendDto));

            var friendship = await _friendshipRepository.GetByIdAsync(blockFriendDto.FriendId);
            if (friendship == null)
                throw new KeyNotFoundException("Friendship not found");

            friendship.Status = FriendshipStatus.Blocked;
            await _friendshipRepository.UpdateAsync(friendship);
            return true;

        }
    }
}