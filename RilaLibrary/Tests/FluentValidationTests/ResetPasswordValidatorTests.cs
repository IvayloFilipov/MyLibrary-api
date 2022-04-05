using NUnit.Framework;
using API.Validation;
using Common.Models.InputDTOs;

using FluentValidation;
using FluentValidation.Results;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class ResetPasswordValidatorTests
    {
        private IValidator<ResetPasswordDto> resetPassValidator;
        private ResetPasswordDto user;

        [SetUp]
        public void SetUp()
        {
            this.resetPassValidator = new ResetPasswordValidator();

            user = new ResetPasswordDto()
            {
                Email = "petar@abv.bg",
                Token = "token123token",
                Password = "Password123@",
                ConfirmPassword = "Password123@",
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
        public void ResetPasswordValidator_ReturnFalseWhenEmailIsInvalid(string email, string errorMessage)
        {

            user.Email = email;

            ValidationResult result = resetPassValidator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Token can not be null")]
        [TestCase("", "Token can not be empty string")]
        [TestCase(" ", "Token can not be white space")]
        public void ResetPasswordValidator_ReturnFalseWhenTokenIsInvalid(string token, string errorMessage)
        {
            user.Token = token;

            ValidationResult result = resetPassValidator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result, Is.Not.Null, errorMessage);
        }

        [Test]
        [TestCase("token123token", "Token is correct")]
        public void ResetPasswordValidator_ReturnTrueWhenHasToken(string token, string errorMessage)
        {
            user.Token = token;

            ValidationResult result = resetPassValidator.Validate(user);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        [TestCase(null, "Please enter your password!")]
        [TestCase("", "Please enter your password!")]
        [TestCase(" ", "Please enter your password!")]
        [TestCase("password123@", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]
        [TestCase("Password123", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]
        [TestCase("Password@@", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]
        /*[TestCase("Passwor1@", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]*/
        public void ResetPasswordValidator_ReturnFalseWhenNewPasswordIsInvalid(string newPassword, string errorMessage)
        {
            user.Password = newPassword;

            ValidationResult result = resetPassValidator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please confirm your password!")]
        [TestCase("", "Please confirm your password!")]
        [TestCase(" ", "Please confirm your password!")]
        [TestCase("123qwer", "Password and Confirm password do not match!")]
        [TestCase("123qwerty", "Password and Confirm password do not match!")]
        public void ResetPasswordValidator_ReturnFalseWhenConfirmNewPasswordIsInvalid(string confirmNewPassword, string errorMessage)
        {
            user.ConfirmPassword = confirmNewPassword;

            ValidationResult result = resetPassValidator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }
    }
}
