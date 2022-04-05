using API.Validation;
using Common.Models.InputDTOs;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using static Common.GlobalConstants;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class GenreValidatorTests
    {
        private IValidator<Genre> genreValidator;
        private Genre genre;

        [SetUp]
        public void Init()
        {
            this.genreValidator = new GenreValidator();
            this.genre = new Genre();
        }

        [Test]
        [TestCase("Thriller")]
        [TestCase("Sci-Fi")]
        public void Should_ReturnValid_When_PassedValidGenreName(string genreName)
        {
            genre.Name = genreName;

            ValidationResult result = genreValidator.Validate(genre);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Should_ReturnInvalid_When_PassedNoGenreName(string genreName)
        {
            genre.Name = genreName;

            ValidationResult result = genreValidator.Validate(genre);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(GENRE_NAME_REQUIRED_ERROR_MESSAGE));
        }

        [Test]
        [TestCase("This is a veeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeery long genre name")]
        public void Should_ReturnInvalid_When_PassedLongGenreName(string genreName)
        {
            genre.Name = genreName;

            ValidationResult result = genreValidator.Validate(genre);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(GENRE_NAME_LENGTH_ERROR_MESSAGE));
        }
    }
}
