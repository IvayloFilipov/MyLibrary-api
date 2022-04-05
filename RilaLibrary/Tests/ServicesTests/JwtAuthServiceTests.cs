using Common.Models.InputDTOs;
using Common.Models.JWT;
using NUnit.Framework;
using Services.Services;

namespace Tests.ServicesTests
{
    [TestFixture]
    public class JwtAuthServiceTests
    {
        private readonly JwtTokenConfig mockJwtTokenConfig = new JwtTokenConfig
        {
            Issuer = "http://localhost:7216",
            Audience = "http://localhost:4200",
            Secret = "UnitTestSecretKey",
            AccessTokenExpiration = 7,
        };

        JwtAuthService? authService;

        [SetUp]
        public void Setup()
        {
            authService = new JwtAuthService(mockJwtTokenConfig);

        }

        [Test]
        public void Should_ReturnToken_When_PassedLoginUserDto()
        {
            var inputUser = new LoginUserWithRolesDto
            {
                Email = "testemail@gmail.com",
                Id = "21193a23-5ff8-4af1-98a1-41c09b732647",
                Password = "AQAAAAEAACcQAAAAEAZEAqx+ScU6ijMFfqdWDNjHNc3y0orlW0SuOJAlJ0anLsp4Nu+4/VnnOjbXtJsDOQ==",
                Roles = { "Reader", "Librarian" }
            };

            var result = authService!.GenerateTokens(inputUser);

            Assert.That(result, Is.TypeOf<JwtAuthResult>());
            Assert.IsNotNull(result.AccessToken);
        }
    }
}
