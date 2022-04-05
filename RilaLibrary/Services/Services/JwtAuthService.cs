using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using log4net;
using Common.Models.InputDTOs;
using Common.Models.JWT;
using Services.Interfaces;

namespace Services.Services
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly JwtTokenConfig jwtTokenConfig;
        private readonly byte[] secret;
        private readonly ILog log = LogManager.GetLogger(typeof(JwtAuthService));


        public JwtAuthService(JwtTokenConfig jwtTokenConfig)
        {
            this.jwtTokenConfig = jwtTokenConfig;
            secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        public JwtAuthResult GenerateTokens(LoginUserWithRolesDto userDto)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, userDto.Email));
            claims.Add(new Claim(ClaimTypes.Hash, userDto.Password));

            foreach (var role in userDto.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = new JwtSecurityToken(
                jwtTokenConfig.Issuer,
                jwtTokenConfig.Audience,
                claims,
                expires: now.AddDays(jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken
            };

            log.Info("Token was generating successfully.");
        }
    }
}
