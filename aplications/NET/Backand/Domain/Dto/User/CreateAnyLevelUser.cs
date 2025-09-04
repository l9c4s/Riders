using System;
using Domain.Enum;

namespace Domain.Dto.User;

public class CreateAnyLevelUser : CreateUserDto
{

    public AccessLevel AccessLevel { get; set; }

}
