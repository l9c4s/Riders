using Aplication.Contracts;
using Domain.Dto.Generics;
using Domain.Dto.RequestResultsDto;
using Domain.Dto.User;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Riders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersUseCases _usersUseCases;
        public UsersController(IUsersUseCases usersUseCases)
        {
            _usersUseCases = usersUseCases;
        }


        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser(LoginUserDto loginUserDto)
        {
            var result = await _usersUseCases.LoginUser(loginUserDto);
            return Ok(
                new RequestResultsDto
                {
                    Message = "Login success.",
                    Success = true,
                    Data = result
                }
            );
        }


        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(CreateUserDto createUser)
        {
            var result = await _usersUseCases.CreateUser(createUser);
            return CreatedAtAction(nameof(AddUser), new { id = result.UserName }, new RequestResultsDto
            {
                Message = "User created successfully.",
                Success = true,
                Data = result
            });
        }

        [Authorize(Policy = nameof(AccessLevel.Admin))]
        [HttpPost("addUsersAllLevels")]
        public async Task<IActionResult> addUsersAllLevels(CreateAnyLevelUser createUser)
        {
            var result = await _usersUseCases.CreateAnyLevelUser(createUser);
            return CreatedAtAction(nameof(AddUser), new { id = result.UserName }, new RequestResultsDto
            {
                Message = "User created successfully.",
                Success = true,
                Data = result
            });
        }


        [Authorize(Policy = nameof(AccessLevel.Admin))]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser(CreateUserDto createUser)
        {
            var ResquestAction = await _usersUseCases.RemoveUser(createUser);
            RequestResultsDto Resultado = new RequestResultsDto
            {
                Message = "Usuário removido com sucesso.",
                Success = true,
                Data = createUser
            };
            return Ok(Resultado);
        }

        [Authorize(Policy = nameof(AccessLevel.Admin))]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            var result = await _usersUseCases.UpdateUser(updateUserDto);
            return Ok(result);
        }


        [Authorize(Policy = nameof(AccessLevel.CommonUser))]
        [HttpPost("GetUserById")]
        public async Task<IActionResult> GetUserById(GetByIdGenerics IdUser)
        {
            var user = await _usersUseCases.GetUserById(IdUser.Id);
            return Ok(new RequestResultsDto
            {
                Message = "User found successfully.",
                Success = true,
                Data = user
            });

        }

        [Authorize(Policy = nameof(AccessLevel.Employee))]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _usersUseCases.GetAllUsers();
            return Ok(new RequestResultsDto
            {
                Message = "All Users.",
                Success = true,
                Data = users
            });
        }

    }
}
