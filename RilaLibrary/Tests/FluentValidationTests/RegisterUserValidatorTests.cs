using API.Validation;
using Common.Models.InputDTOs;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class RegisterUserValidatorTests
    {
        private IValidator<RegisterUserDto> validator;
        private RegisterUserDto user;

        [SetUp]
        public void SetUp()
        {
            this.validator = new RegisterUserValidator();

            user = new RegisterUserDto()
            {
                FirstName = "Peter",
                LastName = "Petrov",
                Email = "petar@abv.bg",
                PhoneNumber = "+(359)882555555",
                Password = "peterPeter-1",
                ConfirmPassword = "peterPeter-1",
                Address = new Address()
                {
                    Country = "Bulgaria",
                    City = "Sofia",
                    Street = "Vasil Levski",
                    StreetNumber = "55"
                }
            };
        }

        [Test]
        public void RegisterValidatorReturnTrueWithCorrectData()
        {
            ValidationResult result = validator.Validate(user);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        [TestCase("", "Please enter your first name!")]
        [TestCase(" ", "Please enter your first name!")]
        [TestCase(null, "Please enter your first name!")]
        [TestCase("ssssssssssssssSSSSSSSSSSSSSsssssssssssSSSSSSSSssssssssSSSSSSSSSSSs", 
            "First name must contain minimum of 1 and maximum of 65 characters.")]
        public void ValidatorReturnFalseWhenFirstNameIsInvalid(string firstName, string errorMessage)
        {
            user.FirstName = firstName;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase("", "Please enter your last name!")]
        [TestCase(" ", "Please enter your last name!")]
        [TestCase(null, "Please enter your last name!")]
        [TestCase("ssssssssssssssSSSSSSSSSSSSSsssssssssssSSSSSSSSssssssssSSSSSSSSSSSs",
            "Last name must contain minimum of 1 and maximum of 65 characters.")]
        public void ValidatorReturnFalseWhenLastNameIsInvalid(string lastName, string errorMessage)
        {
            user.LastName = lastName;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
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

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your phone number!")]
        [TestCase("", "Please enter your phone number!")]
        [TestCase(" ", "Please enter your phone number!")]
        [TestCase("+", "Invalid phone number!")]
        [TestCase("+35933j3999", "Invalid phone number!")]
        [TestCase("+333333333333333335555555553333333333333333322222222229999999999999",
            "Invalid phone number!")]
        public void ValidatorReturnFalseWhenPhoneIsInvalid(string phone, string errorMessage)
        {
            user.PhoneNumber = phone;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your password!")]
        [TestCase("", "Please enter your password!")]
        [TestCase(" ", "Please enter your password!")]
        [TestCase("peterpeter-1", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]
        [TestCase("peterPeter121", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]
        [TestCase("peterPeter-pe", "Password must contain minimum of 10 and maximum of 65 characters." +
            " Password must contain at least one upper-case letter, one lower-case letter, one number and one symbol")]
        public void ValidatorReturnFalseWhenPasswordIsInvalid(string password, string errorMessage)
        {
            user.Password = password;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please confirm your password!")]
        [TestCase("", "Please confirm your password!")]
        [TestCase(" ", "Please confirm your password!")]
        [TestCase("asdfgh", "Password and Confirm password do not match!")]
        [TestCase("1223hjjk", "Password and Confirm password do not match!")]
        public void ValidatorReturnFalseWhenConfirmPasswordIsInvalid(string password, string errorMessage)
        {
            user.ConfirmPassword = password;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your country!")]
        [TestCase("", "Please enter your country!")]
        [TestCase(" ", "Please enter your country!")]
        [TestCase("Pd", "Country must contain minimum of 3 and maximum of 56 characters.")]
        [TestCase("p", "Country must contain minimum of 3 and maximum of 56 characters.")]
        public void ValidatorReturnFalseWhenCountryIsInvalid(string country, string errorMessage)
        {
            user.Address.Country = country;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your city!")]
        [TestCase("", "Please enter your city!")]
        [TestCase(" ", "Please enter your city!")]
        [TestCase("xxxccccccccccccccccccccccccccccccjsdsssssssssssssssssssssssssddddddddddddccccccccccccccccc" +
            "ccccccccccccccccccccccccccccccccccbnnnn", 
            "City must contain minimum of 1 and maximum of 128 characters.")]
        public void ValidatorReturnFalseWhenCityIsInvalid(string city, string errorMessage)
        {
            user.Address.City = city;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your street!")]
        [TestCase("", "Please enter your street!")]
        [TestCase(" ", "Please enter your street!")]
        [TestCase("xxxccccccccccccccccccccccccccccccjsdsssssssssssssssssssssssssddddddddddddccccccccccccccccc" +
            "ccccccccccccccccccccccccccccccccccbnnnn",
            "Street name must contain minimum of 1 and maximum of 128 characters.")]
        public void ValidatorReturnFalseWhenStreetIsInvalid(string street, string errorMessage)
        {
            user.Address.Street = street;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase(null, "Please enter your street number!")]
        [TestCase("", "Please enter your street number!")]
        [TestCase(" ", "Please enter your street number!")]
        [TestCase("xxxccccccccccccccccccccccccccccccjsdsssssssssssssssssssssssssddddd",
            "Streen number must contain minimum of 1 and maximum of 65 characters.")]
        public void ValidatorReturnFalseWhenStreetNumberIsInvalid(string streetNumber, string errorMessage)
        {
            user.Address.StreetNumber = streetNumber;

            ValidationResult result = validator.Validate(user);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }
    }
}
