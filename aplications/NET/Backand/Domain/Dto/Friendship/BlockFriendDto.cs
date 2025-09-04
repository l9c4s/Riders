using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Friendship;

public class BlockFriendDto
{

     [Required (ErrorMessage = "FriendshipId is required.")]
    public Guid FriendId { get; set; }
}
