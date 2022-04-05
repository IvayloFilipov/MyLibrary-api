using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.InputDTOs;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Repositories.Interfaces;
using Services.EmailSender;
using Services.Services;

namespace Tests.ServicesTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> mockReaderRepository = new ();
        private Mock<UserManager<UserEntity>> mockUserManager = new();
        private LoginUserDto loginUserDto = new LoginUserDto();
        private UserEntity userEntity = new UserEntity();

        [SetUp]
        public void Setup()
        {
            this.loginUserDto = new LoginUserDto
            {
                Email = "ivanov@gmail.com"
            };

            this.userEntity = new UserEntity
            {
                Email = "ivanov@gmail.com",
            };

            this.mockUserManager = MockUserManager(userEntity);
        }

        [Test]
        public async Task Should_ReturnSuccessfull_When_UserIsFoundOnLogin()
        {
            var exprectedResult = new LoginUserWithRolesDto
            {
                Email = "ivanov@gmail.com",
                Roles = new List<string> { Role.Reader.ToString() }
            };

            this.mockUserManager.Setup(x => x.FindByEmailAsync("ivanov@gmail.com")).ReturnsAsync(this.userEntity);
            this.mockUserManager.Setup(x => x.CheckPasswordAsync(this.userEntity, It.IsAny<string>())).ReturnsAsync(true);
            this.mockUserManager.Setup(x => x.GetRolesAsync(this.userEntity)).ReturnsAsync(new List<string> { Role.Reader.ToString() });

            var userService = new UserService(this.mockUserManager.Object, null!, null!, null!);
            var actualResult = await userService.LoginAsync(this.loginUserDto);

            Assert.IsNotNull(actualResult);
            Assert.That(actualResult.Email == exprectedResult.Email);
            Assert.That(actualResult.Roles.Count == 1);
        }

        [Test]
        public async Task Should_ReturnNull_When_UserIsNotFoundOnLogin()
        {
            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))!.ReturnsAsync(default(UserEntity));

            var userService = new UserService(this.mockUserManager.Object, null!, null!, null!);
            var actualResult = await userService.LoginAsync(new LoginUserDto());

            Assert.IsNull(actualResult);
        }

        [Test]
        public async Task Should_ReturnNull_When_WrongPasswordOnLogin()
        {
            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(this.userEntity);
            this.mockUserManager.Setup(x => x.CheckPasswordAsync(this.userEntity, It.IsAny<string>())).ReturnsAsync(false);

            var userService = new UserService(this.mockUserManager.Object, null!, null!, null!);
            var actualResult = await userService.LoginAsync(new LoginUserDto());

            Assert.IsNull(actualResult);
        }

        [Test]
        public async Task Should_ReturnSuccessfull_When_UserIsRegistered()
        {
            this.mockUserManager.Setup(x => x.CreateAsync(this.userEntity, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            this.mockUserManager.Setup(x => x.AddToRoleAsync(this.userEntity, Role.Reader.ToString())).ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(this.mockUserManager.Object, null!, null!, null!);
            var actualResult = await userService.RegisterAsync(this.userEntity);

            Assert.IsNotNull(actualResult);
            Assert.True(actualResult.Succeeded);
        }

        [Test]
        public async Task Should_ReturnFailed_When_UserIsNotRegistered()
        {
            this.mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var userService = new UserService(this.mockUserManager.Object, null!, null!, null!);
            var actualResult = await userService.RegisterAsync(new UserEntity());

            Assert.IsFalse(actualResult.Succeeded);
        }

        [Test]
        public async Task Should_ReturnFailed_When_WhenRoleIsNotAddedToUser()
        {
            this.mockUserManager.Setup(x => x.CreateAsync(this.userEntity, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            this.mockUserManager.Setup(x => x.AddToRoleAsync(this.userEntity, string.Empty)).ReturnsAsync(IdentityResult.Failed());

            var userService = new UserService(this.mockUserManager.Object, null!, null!, null!);
            var actualResult = await userService.RegisterAsync(this.userEntity);

            Assert.IsNull(actualResult);
        }

        [Test]
        public async Task Should_NotSendEmail_When_EmailDoesNotExist()
        {
            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<UserEntity>(null!));
            var configurationMock = new Mock<IConfiguration>();
            var mailServiceMock = new Mock<IMailSender>();
            var userService = new UserService(this.mockUserManager.Object, configurationMock.Object, mailServiceMock.Object, null!);

            await userService.CreateCallbackUriAsync(new ForgotPasswordDto());

            mailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Should_SendEmail_When_EmailExists()
        {
            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity());
            this.mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<UserEntity>())).ReturnsAsync("LongLongToken");

            var configurationMock = new Mock<IConfiguration>();
            var mailServiceMock = new Mock<IMailSender>();
            var userService = new UserService(this.mockUserManager.Object, configurationMock.Object, mailServiceMock.Object, null!);

            await userService.CreateCallbackUriAsync(new ForgotPasswordDto());

            mailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(x => x.Contains("reset-password?email"))), Times.Once);
        }

        [Test]
        public async Task Should_NotChangePassword_When_EmailDoesNotExist()
        {
            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<UserEntity>(null!));
            this.mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<UserEntity>())).ReturnsAsync("LongLongToken");
            var configurationMock = new Mock<IConfiguration>();
            var mailServiceMock = new Mock<IMailSender>();

            var userService = new UserService(this.mockUserManager.Object, configurationMock.Object, mailServiceMock.Object, null!);

            await userService.SaveNewPasswordAsync(new ResetPasswordDto());

            mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Should_ChangePassword_When_UserExistsAndTokenIsValid()
        {
            var userDto = new ResetPasswordDto
            {
                Email = "ivanov@gmail.com",
                Token = "tokenHere",
                Password = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity());
            this.mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var configurationMock = new Mock<IConfiguration>();
            var mailServiceMock = new Mock<IMailSender>();

            var userService = new UserService(this.mockUserManager.Object, configurationMock.Object, mailServiceMock.Object, null!);

            await userService.SaveNewPasswordAsync(userDto);

            mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Should_ThrowArgumentException_When_UserIsValidButResetPasswordFails()
        {
            var userDto = new ResetPasswordDto
            {
                Email = "ivanov@gmail.com",
                Token = "Q2ZESjhDb0pldGlCRi9kTGo0VGZ1Y3pBQTNpUUk1b0dRenZ4RmdlODhKOVFxNTF4MWZPUWxsMWcyNTBjaUlqYUdOVDd6blB6OGhSckZqQWFvWFRqVVQwZldVV3JQVEpwMVlFVmVHWVQxMUxtcldCRFZyeHk4NGRwSEtQbEU1SVZYeG9kd0l1UHRuL1VZc1lPZDFVd0t5elhvaVFQRldzTUtFNUQ0MnRua3NKQ3RleDI0dWIzTXlERDJyUmxKTkRqclJyN3dQbnlydUljMnBPOW9pamFCb2k5cTY0T3ZNQ1RmUkZUN1F5anVPOXVHTGNo",
                Password = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            this.mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity());
            this.mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var configurationMock = new Mock<IConfiguration>();
            var mailServiceMock = new Mock<IMailSender>();

            var userService = new UserService(this.mockUserManager.Object, configurationMock.Object, mailServiceMock.Object, null!);

            Exception e = null!;

            try
            {
                await userService.SaveNewPasswordAsync(userDto);
            }
            catch (Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
            mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Should_ReturnCorrectNumberOfAllReaders_When_GettingCountOfAll()
        {
            int expectedCount = 0;
            mockReaderRepository.Setup(x => x.GetCountOfAllReadersAsync()).ReturnsAsync(expectedCount);
            var configurationMock = new Mock<IConfiguration>();
            var userRepositoryMock = new Mock<IUserRepository>();

            var userService = new UserService(mockUserManager.Object, configurationMock.Object, null!, userRepositoryMock.Object);

            var actualCount = await userService.GetCountOfAllReadersAsync();

            Assert.AreEqual(expectedCount, actualCount);
        }

        [Test]
        public async Task Should_ReturnResult_When_CallingGetByIdAsync()
        {
            var userResult = new UserEntity
            {
                FirstName = "Firstname",
                LastName = "Lastname",
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var userService = new UserService(this.mockUserManager.Object, null!, null!, userRepositoryMock.Object);

            userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(userResult);

            var result = await userService.GetByUserIdRepoAsync("string-id-here");
            Assert.IsNotNull(result);
        }

        public static Mock<UserManager<T>> MockUserManager<T>(T input)
            where T : class
        {
            var store = new Mock<IUserStore<T>>();
            var mgr = new Mock<UserManager<T>>(store.Object, null!, null!, null!, null!, null!, null!, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<T>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<T>());

            return mgr;
        }
    }
}
