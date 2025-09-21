using System.Runtime.Intrinsics.Arm;
using Aplication.Contracts;
using Domain.Contracts;
using Domain.Dto.RequestResultsDto;
using Domain.Dto.Token;
using Domain.Dto.User;
using Domain.Password.Utils;
using Domain.Utils.Mappers;

namespace Aplication.UseCases.User
{
    public class UserCasesImplements : IUsersUseCases
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenUseCases _tokenUseCases;

        public UserCasesImplements(IUserRepository userRepository, ITokenUseCases tokenUseCases)
        {
            _userRepository = userRepository;
            _tokenUseCases = tokenUseCases;
        }

        public async Task<CreateUserDto> CreateUser(CreateUserDto createUserDto)
        {
            await IsValidEmailAndUser(createUserDto.Email, createUserDto.UserName);
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                PasswordHash = Utils.GenerateHash(createUserDto.Password),
                AccessLevel = Domain.Enum.AccessLevel.CommonUser
            };

            await _userRepository.AddAsync(user);
            return createUserDto;
        }

        public async Task<bool> RemoveUser(CreateUserDto createUserDto)
        {
            var user = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (user == null)
            {
                throw new ArgumentException("Password or Email is wrong !!");
            }

            if (!Utils.VerifyHash(createUserDto.Password, user.PasswordHash))
            {
                throw new ArgumentException("Password or Email is wrong !!");
            }

            await _userRepository.DeleteAsync(user.Id);
            return true;
        }

        public async Task<RequestResultsDto> UpdateUser(UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            if (!Utils.VerifyHash(updateUserDto.Password, user.PasswordHash))
            {
                throw new ArgumentException("Password or Email is wrong !!");
            }

            bool userNameChanged = user.UserName != updateUserDto.UserName;
            bool emailChanged = user.Email != updateUserDto.Email;

            var changedFields = new
            {
                UserName = userNameChanged ? user.UserName : null,
                Email = emailChanged ? user.Email : null
            };


            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;
            await _userRepository.UpdateAsync(user);


            return new RequestResultsDto(
                   "User updated successfully",
                   true,
                   changedFields
               );
        }

        public async Task<CreateAnyLevelUser> GetUserById(string id)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(id));
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            
            return new CreateAnyLevelUser
            {
                UserName = user.UserName,
                Email = user.Email,
                AccessLevel = user.AccessLevel,
            };
        }

        public async Task<IEnumerable<ListUsersDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
            {
                throw new ArgumentException("No users found");
            }

            var usersMaps = MapperUtils.MapList<Domain.Entities.User, ListUsersDto>(users);

            return usersMaps;
        }

        public async Task<TokenResponse> LoginUser(LoginUserDto LoginUserDto)
        {
            var user = await _userRepository.GetByEmailAsync(LoginUserDto.Email);

            if (user == null || !Utils.VerifyHash(LoginUserDto.Password, user.PasswordHash))
            {
                throw new ArgumentException("Password or Email is wrong !!");
            }

            var token = await _tokenUseCases.GenerateToken(user.Id.ToString(), user.Email);

            if (token == null)
            {
                throw new ArgumentException("Failed to generate token");
            }
            return token;
        }

        public async Task<CreateAnyLevelUser> CreateAnyLevelUser(CreateAnyLevelUser createUserDto)
        {
            await IsValidEmailAndUser(createUserDto.Email, createUserDto.UserName);
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                PasswordHash = Utils.GenerateHash(createUserDto.Password),
                AccessLevel = createUserDto.AccessLevel
            };

            await _userRepository.AddAsync(user);
            return createUserDto;
        }

        private async Task<bool> IsValidEmailAndUser(string email, string UserName)
        {
            // Check if the user exists
            var user = await _userRepository.GetByUserNameAndEmailAsync(UserName, email);
            if (user != null)
            {
                throw new ArgumentException("User with this email or username already exists");
            }
            return false;
            
        }

        public async Task<bool> ResetPassword(UpdatePasswordDto resetPasswordDto)
        {
            var user = await _userRepository.GetByUserNameAndEmailAsync(resetPasswordDto.Name, resetPasswordDto.Email) ?? throw new ArgumentException("User or Email is wrong !!");

            user.PasswordHash = Utils.GenerateHash(resetPasswordDto.NewPassword);

            await _userRepository.ResetPasswordAsync(user.Id, user.PasswordHash);
            return true;
        }
    }
}
