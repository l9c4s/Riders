using Domain.Dto.Friendship;


namespace Aplication.Contracts
{
    public interface IFriendshipUseCases
    {

        Task<RequestFriendshipDto> RequestFriendship(RequestFriendshipDto request);
        Task<RespondToFriendshipRequestDto> RespondToFriendshipRequest(FriendRequestWithDTO request);
        Task<IEnumerable<ListFriendsDto>> GetFriendships(Guid userId);
        Task<IEnumerable<ListFriendsDto>> GetPendingFriendshipRequests(Guid userId);
        Task<bool> BlockedFriendUsers(BlockFriendDto blockFriendDto, Guid userId);
        Task DeleteFriendship(Guid friendshipId);
    }
}
