using Common;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
using Moq;
using NUnit.Framework;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Common.ExceptionMessages;

namespace Tests.ServicesTests
{
    [TestFixture]
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> mockBookRepository = new Mock<IBookRepository>();
        private readonly Mock<IAuthorService> mockAuthorService = new Mock<IAuthorService>();
        private readonly Mock<IGenreService> mockGenreService = new Mock<IGenreService>();
        private readonly Mock<IAuthorsBooksService> mockAuthorsBooksService = new Mock<IAuthorsBooksService>();
        private readonly Mock<IGenresBooksService> mockGenresBooksService = new Mock<IGenresBooksService>();
        private readonly Mock<IBlobService> mockBlobService = new Mock<IBlobService>();

        private BookService? bookService;

        private AddBookDto addBookDto = new AddBookDto();
        private AddBookDto addBookDtoNoAuthors = new AddBookDto();
        private AddBookDto addBookDtoNoGenres = new AddBookDto();
        private AddBookDto updateBookHaveLessThenZeroQuantity = new AddBookDto();
        private BookEntity bookEntity = new BookEntity();
        private AuthorEntity authorEntity = new AuthorEntity();
        private BookOutput bookOutput = new BookOutput();

        [SetUp]
        public void Setup()
        {
            bookEntity = new BookEntity
            {
                Title = "WonderLand",
            };

            authorEntity = new AuthorEntity
            {
                 AuthorName = "Ivan Petrov"
            };

            addBookDto = new AddBookDto
            {
                BookTitle = "Alice",
                BookAuthors = new List<string>() { "Ivan Ivanov" },
                BookCover = null,
                Genres = new List<string> { "Romantic" },
                TotalQuantity = 2
            };

            addBookDtoNoAuthors = new AddBookDto
            {
                BookTitle = "Alice",
                BookAuthors = null,
                BookCover = null,
                Genres = new List<string> { "Romantic" },
            };

            addBookDtoNoGenres = new AddBookDto
            {
                BookTitle = "Alice",
                BookAuthors = new List<string>() { "Ivan Ivanov" },
                BookCover = null,
                Genres = null,
            };

            updateBookHaveLessThenZeroQuantity = new AddBookDto
            {
                BookTitle = "Alice",
                BookCover = null,
                BookAuthors = new List<string> { "Maria Petrova" },
                Genres = new List<string> { "Romantic" },
                TotalQuantity = -2
            };

            bookOutput = new BookOutput
            {
                Title = "Some book",
                AllAuthors = "Author name",
                AllGenres = "All genres",
                IsAvailable = true,
            };

            mockGenreService.Setup(x => x.FindGenreByName(It.IsAny<string>())).Returns(new GenreEntity());
            mockAuthorService.Setup(x => x.FindAuthorByName(It.IsAny<string>())).Returns(authorEntity);

            bookService = new BookService(mockBookRepository.Object, 
                                          mockAuthorService.Object, 
                                          mockGenreService.Object, 
                                          mockAuthorsBooksService.Object,
                                          mockGenresBooksService.Object,
                                          mockBlobService.Object,
                                          null!);
        }

        [Test]
        public async Task Should_ReturnSuccessfull_When_Book_Is_Added()
        {
            mockBookRepository.Setup(x => x.InsertAsync(It.IsAny<BookEntity>())).Returns(Task.FromResult(bookEntity));

            var result = await bookService!.AddBookAsync(addBookDto);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ReturnError_When_BookQuantity_Is_NotValid()
        {
            mockBookRepository.Setup(x => x.InsertAsync(It.IsAny<BookEntity>()))!.ReturnsAsync(default(BookEntity));

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.AddBookAsync(updateBookHaveLessThenZeroQuantity));
            Assert.AreEqual(BOOK_QUANTITY_IS_INVALID, result!.Message);
        }

        [Test]
        public void Should_ReturnError_When_BookName_Is_Found()
        {
            mockBookRepository.Setup(x => x.FindBookByTitle(addBookDto)).Returns(bookEntity);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.AddBookAsync(addBookDto));
            Assert.AreEqual(BOOKTITLE_FOUND, result!.Message);
        }

        [Test]
        public async Task Should_ReturnSuccessfull_When_BookName_Is_NotFound()
        {
            mockBookRepository.Setup(x => x.FindBookByTitle(It.IsAny<AddBookDto>())).Returns(default(BookEntity)!);

            var result = await bookService!.AddBookAsync(addBookDto);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Should_ReturnSuccessfull_When_AuthorName_Is_Found()
        {
            mockAuthorService.Setup(x => x.FindAuthorByName(It.IsAny<string>())).Returns(authorEntity);

            var result = await bookService!.AddBookAsync(addBookDto);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ReturnError_When_AuthorName_Is_NotFound()
        {
            mockAuthorService.Setup(x => x.FindAuthorByName(It.IsAny<string>())).Returns(new AuthorEntity());

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.AddBookAsync(addBookDtoNoAuthors));
            Assert.AreEqual(NO_AUTHORS_FOUND, result!.Message);
        }

        [Test]
        public async Task Should_ReturnSuccessfull_When_Genre_Is_Found()
        {
            var result = await bookService!.AddBookAsync(addBookDto);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ReturnError_When_UpdateBook_Is_NotFound()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookEntity));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookService!.UpdateBookAsync(Guid.NewGuid(), new AddBookDto()));
            Assert.AreEqual(BOOK_NOT_FOUND, result!.Message);
        }

        [Test]
        public void Should_ReturnError_When_UpdateBook_Has_DuplicateName()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockBookRepository.Setup(x => x.ContainsBookName(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(true);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.UpdateBookAsync(Guid.NewGuid(), new AddBookDto()));
            Assert.AreEqual(BOOK_EXISTS, result!.Message);
        }

        [Test]
        public void Should_ReturnError_When_UpdateBook_Has_NoGenres()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockBookRepository.Setup(x => x.ContainsBookName(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.UpdateBookAsync(Guid.NewGuid(), addBookDtoNoGenres));
            Assert.AreEqual(NO_GENRES_FOUND, result!.Message);
        }

        [Test]
        public void Should_ReturnError_When_UpdateBook_Has_NoAuthors()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockBookRepository.Setup(x => x.ContainsBookName(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.UpdateBookAsync(Guid.NewGuid(), addBookDtoNoAuthors));
            Assert.AreEqual(NO_AUTHORS_FOUND, result!.Message);
        }

        [Test]
        public void Should_ReturnError_When_UpdateBook_Has_Less_Than_ZeroQuantity()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockBookRepository.Setup(x => x.ContainsBookName(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(false);
            mockAuthorService.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());
            mockGenreService.Setup(x => x.FindGenresByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.UpdateBookAsync(Guid.NewGuid(), updateBookHaveLessThenZeroQuantity));
            Assert.AreEqual(BOOK_QUANTITY_IS_LESS_THAN_ZERO, result!.Message);
        }
        
        [Test]
        public async Task Should_ReturnOk_When_UpdateBook_Is_ValidWithExistingAuthor()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockBookRepository.Setup(x => x.ContainsBookName(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(false);
            mockAuthorService.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());
            mockGenreService.Setup(x => x.FindGenresByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var result = await bookService!.UpdateBookAsync(Guid.NewGuid(), addBookDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(addBookDto.BookTitle, result.Title);
        }

        [Test]
        public async Task Should_ReturnOk_When_UpdateBook_Is_ValidWithNewAuthor()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockBookRepository.Setup(x => x.ContainsBookName(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(false);
            mockAuthorService.Setup(x => x.FindAuthorByName(It.IsAny<string>())).Returns(authorEntity);
            mockAuthorService.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());
            mockGenreService.Setup(x => x.FindGenresByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var result = await bookService!.UpdateBookAsync(Guid.NewGuid(), addBookDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(addBookDto.BookTitle, result.Title);
        }

        [Test] 
        public void Should_Complete_When_DeletingExistingBook()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());

            var result = bookService!.DeleteBookAsync(Guid.NewGuid());

            Assert.That(result.IsCompleted, Is.True);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_DeletingNonExistingBook()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookEntity));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookService!.DeleteBookAsync(Guid.NewGuid()));
            Assert.AreEqual(BOOK_NOT_FOUND, result!.Message);
        }

        [Test]
        public void Should_ThrowArgumentException_When_DeletingReservedBook()
        {
            var book = new BookEntity
            {
                Title = "Test book",
                TotalQuantity = 2,
                CurrentQuantity = 1,
            };

            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(book);

            var result = Assert.ThrowsAsync<ArgumentException>(async () => await bookService!.DeleteBookAsync(Guid.NewGuid()));
            Assert.AreEqual(BOOK_HAS_APPROVED_RESERVATIONS, result!.Message);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_NoBooksFound()
        {
            mockBookRepository.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(new List<BookOutput>());

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookService!.GetAllBooksAsync());
            Assert.AreEqual(NO_BOOKS_FOUND, result!.Message);
        }

        [Test]
        public async Task Should_Complete_When_FoundBooks()
        {
            mockBookRepository.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(new List<BookOutput> { bookOutput });
            mockAuthorService.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());
            mockGenreService.Setup(x => x.FindGenresByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var result = await bookService!.GetAllBooksAsync();

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_BookNotFound()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(BookEntity));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookService!.GetBookByIdAsync(Guid.NewGuid()));
            Assert.AreEqual(BOOK_NOT_FOUND, result!.Message);
        }

        [Test]
        public async Task Should_Complete_When_FoundBook()
        {
            mockBookRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookEntity());
            mockAuthorService.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());
            mockGenreService.Setup(x => x.FindGenresByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var result = await bookService!.GetBookByIdAsync(Guid.NewGuid());
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Should_Return_BooksAndCount_When_GettingBooksPaged()
        {
            mockBookRepository.Setup(x => x.GetEntityPageAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<BookEntity> { bookEntity }, 1));

            var (resultList, resultCount) = await bookService!.GetBooksAsync(new PaginatorInputDto());
            Assert.IsNotNull(resultList);
            Assert.IsNotNull(resultCount);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_NoBooksFoundPaged()
        {
            mockBookRepository.Setup(x => x.GetEntityPageAsync(It.IsAny<PaginatorInputDto>()))!.ReturnsAsync((default(List<BookEntity>), default(int)));
            
            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookService!.GetBooksAsync(new PaginatorInputDto()));
            Assert.AreEqual(NO_BOOKS_FOUND, result!.Message);
        }

        [Test]
        public async Task GetCountOfAllBooksAsync_ShouldReturnCorrectNumberOfAllBooks()
        {
            // Arrange
            int count = 3;
            mockBookRepository.Setup(x => x.GetCountOfAllBooksAsync()).ReturnsAsync(count);

            // Act
            var result = await bookService!.GetCountOfAllBooksAsync();

            // Assert
            Assert.AreEqual(count, result);
        }

        [Test]
        public async Task Should_Return_BooksAndCount_When_SearchingBooksPaged()
        {
            mockAuthorService.Setup(x => x.FindMultipleAuthorsByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<Guid>());
            mockGenreService.Setup(x => x.FindMultipleGenresByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<Guid>());
            mockBookRepository.Setup(x => x.SearchForBooksAsync(It.IsAny<SearchBookDto>(), It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>(), It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<BookOutput> { bookOutput }, 1));
            mockAuthorService.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());
            mockGenreService.Setup(x => x.FindGenresByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var (resultList, resultCount) = await bookService!.SearchForBooksAsync(new SearchBookDto(), new PaginatorInputDto());
            Assert.IsNotNull(resultList);
            Assert.IsNotNull(resultCount);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_SearchNoBooksFoundPaged()
        {
            var searchDto = new SearchBookDto
            {
                Author = "Author name",
                Genre = "Genre name",
            };

            mockAuthorService.Setup(x => x.FindMultipleAuthorsByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<Guid>());
            mockGenreService.Setup(x => x.FindMultipleGenresByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<Guid>());
            mockBookRepository.Setup(x => x.SearchForBooksAsync(It.IsAny<SearchBookDto>(), It.IsAny<List<Guid>>(), It.IsAny<List<Guid>>(), It.IsAny<PaginatorInputDto>()))!.ReturnsAsync((default(List<BookOutput>), default(int)));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await bookService!.SearchForBooksAsync(searchDto, new PaginatorInputDto()));
            Assert.AreEqual(NO_BOOKS_FOUND, result!.Message);
        }
    }
}
