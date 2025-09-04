using Domain.Enum;

namespace Domain.Dto.User;

public class ListUsersDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public AccessLevel AccessLevel { get; set; }
}
    