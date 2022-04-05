using API.Validation;
using Common.Models.InputDTOs;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.FluentValidationTests
{
    [TestFixture]
    public class AddBookValidatorTests
    {
        private IValidator<AddBookDto> bookValidator;
        private AddBookDto book;

        [SetUp]
        public void SetUp()
        {
            this.bookValidator = new AddBookValidator();
          

            book = new AddBookDto
            {
                BookTitle = "Alice",
                Description = "It is a nice story",
                BookCover = null,
                BookAuthors = new List<string>() { "Romantic, Comedy" }
            };
        }

        [Test]
        [TestCase("", "Please enter a book title!")]
        [TestCase(" ", "Please enter a book title!")]
        [TestCase(null, "Please enter a book title!")]
        [TestCase("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
            "Book title must contain minimum of 1 and maximum of 256 characters.")]
        public void Should_ReturnError_When_BookTitle_Is_Invalid(string bookTitle, string errorMessage)
        {
            book.BookTitle = bookTitle;
            ValidationResult result = bookValidator.Validate(book);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        [Test]
        [TestCase("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss" +
            "ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
            "Description must contain max length of 1028 characters.")]
        public void Should_ReturnError_When_BookDescription_Is_Invalid(string description, string errorMessage)
        {
            book.Description = description;
            ValidationResult result = bookValidator.Validate(book);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }

        //[Test]
        //[TestCase("", "Please insert your book cover!")]
        //[TestCase(" ", "Please insert your book cover!")]
        //[TestCase(null, "Please insert your book cover!")]
        //public void Should_ReturnError_When_BookCover_Is_Invalid(string bookCover, string errorMessage)
        //{
        //    book.BookCover = bookCover;
        //    ValidationResult result = bookValidator.Validate(book);

        //    Assert.IsFalse(result.IsValid);
        //    Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        //}

        [Test]
        [TestCase(null, "Please insert authors!")]
        public void Should_ReturnError_When_BookAuthors_Is_Invalid(List<string> bookAuthors, string errorMessage)
        {
            book.BookAuthors = bookAuthors;
            ValidationResult result = bookValidator.Validate(book);

            Assert.IsFalse(result.IsValid);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
        }
    }
}
