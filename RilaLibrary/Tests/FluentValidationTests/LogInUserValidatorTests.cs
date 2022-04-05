using NUnit.Framework;
using API.Validation;
using FluentValidation;
using FluentValidation.Results;
using Common.Models.InputDTOs;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class LogInUserValidatorTests
    {
        private IValidator<LoginUserDto> loginValidator;
        private LoginUserDto user;

        [SetUp]
        public void SetUp()
        {
            this.loginValidator = new LoginUserValidator();

            user = new LoginUserDto()
            {
                Email = "petar@abv.bg",
                Password = "peterPeter-1",
            };
        }

        [Test]
        [TestCase("", "Please enter your email!")]
        [TestCase(" ", "Please enter your email!")]
        [TestCase(null, "Please enter your email!")]
        [TestCase("petar@@abv.bg", "An invalid email address has been entered into the email field.")]
        [TestCase("ala.bala.abv.com", "An invalid email address has been entered into the email field.")]
        [TestCase("ala.bala@abv..com", "An invalid email address has been entered into the email field.")]
        [TestCase("ala.bala@abvcom", "An invalid email address has been entered into the email field.")]
        public void ValidatorReturnFalseWhenEmailIsInvalid(string email, string errorMessage)
        {
            user.Email = email;

            ValidationResult result = loginValidator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your password!")]
        [TestCase("", "Please enter your password!")]
        [TestCase(" ", "Please enter your password!")]
        public void ValidatorReturnFalseWhenPasswordIsInvalid(string password, string errorMessage)
        {
            user.Password = password;

            ValidationResult result = loginValidator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }
    }
}
