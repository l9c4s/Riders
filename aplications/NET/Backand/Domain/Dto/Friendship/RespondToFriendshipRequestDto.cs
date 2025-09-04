using System.ComponentModel.DataAnnotations;

namespace Domain.Dto.Friendship
{
    public class RespondToFriendshipRequestDto : IValidatableObject
    {
        [Required (ErrorMessage = "FriendshipId is required.")]
        public Guid FriendshipId { get; set; }
        [Required (ErrorMessage = "Response is required.")]
        public bool Accepted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FriendshipId == Guid.Empty)
            {
                yield return new ValidationResult("FriendshipId is required.", new[] { nameof(FriendshipId) });
            }
        }
    }
}
