using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using Repositories.Mappers;
using Services.Interfaces;

using static Common.GlobalConstants;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtAuthService jwtAuthService;

        public UsersController(IUserService userService, IJwtAuthService jwtAuthService)
        {
            this.userService = userService;
            this.jwtAuthService = jwtAuthService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto input)
        {
            var userEntity = Mapper.ToUserEntity(input);
            var result = await this.userService.RegisterAsync(userEntity);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginUserDto userDto)
        {
            LoginUserWithRolesDto loginUserWithRoles = await this.userService.LoginAsync(userDto);

            if (loginUserWithRoles is null)
            {
                return BadRequest();
            }

            var jwtResult = this.jwtAuthService.GenerateTokens(loginUserWithRoles);

            return Ok(new LoginResult
            {
                Id = loginUserWithRoles.Id,
                Email = loginUserWithRoles.Email,
                Roles = loginUserWithRoles.Roles,
                AccessToken = jwtResult.AccessToken,
            });
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            await this.userService.CreateCallbackUriAsync(forgotPasswordDto);

            return Ok(StatusCode(200));
        }

        [Authorize(Roles = $"{LIBRARIAN_ROLE_NAME}, {ADMIN_ROLE_NAME}, {READER_ROLE_NAME}")]
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                await this.userService.SaveNewPasswordAsync(resetPasswordDto);

                return Ok(StatusCode(200));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


