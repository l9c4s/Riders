using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Friendship;

public class RequestFriendDto
{
    [Required(ErrorMessage = "AddresseeId is required.")]
    public Guid AddresseeId { get; set; }

}
