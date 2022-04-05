using API.Validation;
using Common.Models.InputDTOs;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class AuthorValidatorTests
    {
        private IValidator<AuthorDto> validator;
        private AuthorDto author;

        [SetUp]
        public void SetUp()
        {
            this.validator = new AuthorValidator();

            this.author = new AuthorDto()
            {
                AuthorName = "Mark Twain"
            };
        }

        [Test]
        public void AuthorValidatorWorksCorrectly()
        {
            ValidationResult result = validator.Validate(this.author);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        [TestCase("", "Please enter an author name!")]
        [TestCase(" ", "Please enter an author name!")]
        [TestCase(null , "Please enter an author name!")]
        public void AuthorValidatorReturnsInvalidWhenStringIsEmpty(string authorName, string message)
        {
            author.AuthorName = authorName;

            ValidationResult result = validator.Validate(this.author);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(message));
        }

        [Test]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" +
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 
            "Author name must contain minimum of 1 and maximum of 256 characters.")]
        public void AuthorValidatorReturnsInvalidWhenStringIsInvalid(string authorName, string message)
        {
            author.AuthorName = authorName;

            ValidationResult result = validator.Validate(this.author);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(message));
        }
    }
}
