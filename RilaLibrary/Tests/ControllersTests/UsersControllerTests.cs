using API.Controllers;
using Common.Models.InputDTOs;
using Common.Models.JWT;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Tests.ControllersTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> mockUserService = new Mock<IUserService>();
        private readonly Mock<IJwtAuthService> mockJwtService = new Mock<IJwtAuthService>();
        private UsersController usersController;

        [SetUp]
        public void Init()
        {
            usersController = new UsersController(mockUserService.Object, mockJwtService.Object);
        }

        [Test]
        public async Task Should_Return_Ok_When_Passed_Valid_User_Registration()
        {
            mockUserService.Setup(x => x.RegisterAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Success);

            var result = await usersController.RegisterUser(new RegisterUserDto());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.That(okResult?.StatusCode == 200);
        }

        [Test]
        public async Task Should_Return_BadRequest_When_Passed_Invalid_User_Registration()
        {
            mockUserService.Setup(x => x.RegisterAsync(It.IsAny<UserEntity>())).ReturnsAsync(IdentityResult.Failed());

            var result = await usersController.RegisterUser(new RegisterUserDto());
            var badResult = result as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.That(badResult?.StatusCode == 400);
        }

        [Test]
        public async Task Should_Return_Ok_When_Passed_Valid_User_Login()
        {
            mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>())).ReturnsAsync(new LoginUserWithRolesDto());
            mockJwtService.Setup(x => x.GenerateTokens(It.IsAny<LoginUserWithRolesDto>())).Returns(new JwtAuthResult());

            var result = await usersController.LoginUser(new LoginUserDto());
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.That(okResult?.StatusCode == 200);
        }

        [Test]
        public async Task Should_Return_BadRequest_When_Passed_Invalid_User_Login()
        {
            mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginUserDto>()))!.ReturnsAsync(default(LoginUserWithRolesDto));

            var result = await usersController.LoginUser(new LoginUserDto());
            var badResult = result.Result as BadRequestResult;

            Assert.IsNotNull(badResult);
            Assert.That(badResult?.StatusCode == 400);
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnOkWhenEmailExists()
        {
            mockUserService.Setup(x => x.CreateCallbackUriAsync(It.IsAny<ForgotPasswordDto>()));

            var result = await usersController.ForgotPassword(new ForgotPasswordDto());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.That(okResult?.StatusCode == 200);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnOkWhenEmailExists()
        {
            mockUserService.Setup(x => x.SaveNewPasswordAsync(It.IsAny<ResetPasswordDto>()));

            var result = await usersController.ResetPassword(new ResetPasswordDto());
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.That(okResult?.StatusCode == 200);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnBadRequestWhenEmailNotExists()
        {
            mockUserService.Setup(x => x.SaveNewPasswordAsync(It.IsAny<ResetPasswordDto>())).ThrowsAsync(new ArgumentException());

            var result = await usersController.ResetPassword(new ResetPasswordDto());
            var badResult = result as BadRequestObjectResult;

            Assert.IsNotNull(badResult);
            Assert.That(badResult?.StatusCode == 400);
        }
    }
}
