using Domain.Dto.RequestResultsDto;
using Domain.Dto.Token;
using Domain.Dto.User;

namespace Aplication.Contracts
{
    public interface IUsersUseCases
    {
       
        Task<TokenResponse> LoginUser(LoginUserDto LoginUserDto);
       
        Task<CreateUserDto> CreateUser(CreateUserDto createUserDto);

        Task<CreateAnyLevelUser> CreateAnyLevelUser(CreateAnyLevelUser createUserDto);

        Task<bool> RemoveUser(CreateUserDto createUserDto);

        Task<RequestResultsDto> UpdateUser(UpdateUserDto createUserDto);

        Task<CreateAnyLevelUser> GetUserById(string id);

        Task<IEnumerable<ListUsersDto>> GetAllUsers();
    }
}
