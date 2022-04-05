using NUnit.Framework;
using Common.Models.InputDTOs;

using FluentValidation;
using FluentValidation.Results;
using API.Validation;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class ForgotPasswordValidatorTests
    {
        private IValidator<ForgotPasswordDto> forgotPassValidator;
        private ForgotPasswordDto userEmail;

        [SetUp]
        public void SetUp()
        {
            this.forgotPassValidator = new ForgotPasswordValidator();

            userEmail = new ForgotPasswordDto()
            {
                Email = "petar@abv.bg",
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
        public void ForgotPasswordValidator_ReturnFalseWhenEmailIsInvalid(string email, string errorMessage)
        {

            userEmail.Email = email;

            ValidationResult result = forgotPassValidator.Validate(userEmail);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }
    }
}
