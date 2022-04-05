using Common.Models.InputDTOs;
using Common.Models.JWT;

namespace Services.Interfaces
{
    public interface IJwtAuthService
    {
        JwtAuthResult GenerateTokens(LoginUserWithRolesDto userDto);
    }
}
