using Domain.Dto.User;
using Domain.Dto.RequestResultsDto;
using Domain.Dto.Token;


namespace Aplication.Contracts
{
    public interface IUsersUseCases
    {
       
        Task<TokenResponse> LoginUser(LoginUserDto LoginUserDto);
       
        Task<CreateUserDto> CreateUser(CreateUserDto createUserDto);

        Task<CreateAnyLevelUser> CreateAnyLevelUser(CreateAnyLevelUser createUserDto);

        Task<bool> RemoveUser(CreateUserDto createUserDto);

        Task<RequestResultsDto> UpdateUser(UpdateUserDto createUserDto);

        Task<bool> ResetPassword(UpdatePasswordDto resetPasswordDto);
        Task<CreateAnyLevelUser> GetUserById(string id);

        Task<IEnumerable<ListUsersDto>> GetAllUsers();
    }
}
