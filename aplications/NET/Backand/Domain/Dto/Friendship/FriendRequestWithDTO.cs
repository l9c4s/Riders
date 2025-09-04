

namespace Domain.Dto.Friendship;

public class FriendRequestWithDTO : RespondToFriendshipRequestDto
{
    public Guid Guest { get; set; }
}
