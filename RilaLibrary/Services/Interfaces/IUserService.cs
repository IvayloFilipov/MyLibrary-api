using Microsoft.AspNetCore.Identity;
using Common.Models.InputDTOs;
using DataAccess.Entities;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(UserEntity user);

        Task<LoginUserWithRolesDto> LoginAsync(LoginUserDto user);

        Task CreateCallbackUriAsync(ForgotPasswordDto forgotPasswordDto);

        Task SaveNewPasswordAsync(ResetPasswordDto resetPasswordDto);

        Task<int> GetCountOfAllReadersAsync();

        Task<UserEntity?> GetByUserIdRepoAsync(string userId);
    }
}
