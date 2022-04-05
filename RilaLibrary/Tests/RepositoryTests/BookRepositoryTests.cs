using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess;
using DataAccess.Entities;
using NUnit.Framework;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class BookRepositoryTests
    {
        private LibraryDbContext? inMemoryContext;
        private BookRepository? booksRepository;

        [SetUp]
        public void Init()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            booksRepository = new BookRepository(inMemoryContext, null!);

            List<GenreEntity> genres = new List<GenreEntity>
            {
                new GenreEntity { Name = "Thriller", Id = Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreEntity { Name = "Sci-Fi", Id = Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreEntity { Name = "Fantasy", Id = Guid.Parse("8cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
            };

            List<BookEntity> books = new List<BookEntity>
            {
                new BookEntity { Title = "Book 1",
                    Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    Description = "Sample description"
                },
                new BookEntity { Title = "Book 2",
                    Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    Description = "Lorem ipsum"
                },
                new BookEntity { Title = "Book 3",
                    Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    Description = "Another one"
                },
            };

            List<AuthorEntity> authors = new List<AuthorEntity>
            {
                new AuthorEntity { AuthorName = "Author 1", Id = Guid.Parse("1ad7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new AuthorEntity { AuthorName = "Author 2", Id = Guid.Parse("2ad7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new AuthorEntity { AuthorName = "Author 3", Id = Guid.Parse("3ad7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
            };
            
            List<AuthorsBooks> bookAuthors = new List<AuthorsBooks>
            {
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[0],
                    AuthorEntityId = Guid.Parse("2ad7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[1]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[1],
                    AuthorEntityId = Guid.Parse("2ad7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[1]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[2],
                    AuthorEntityId = Guid.Parse("3ad7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[2]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[2],
                    AuthorEntityId = Guid.Parse("2ad7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[1]
                },
            };

            List<GenresBooks> bookGenres = new List<GenresBooks>
            {
                new GenresBooks {
                    BookEntityId = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[0],
                    GenreEntityId = Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    GenresEntity = genres[1]
                },
                new GenresBooks {
                    BookEntityId = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[1],
                    GenreEntityId = Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    GenresEntity = genres[1]
                },
                new GenresBooks {
                    BookEntityId = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[2],
                    GenreEntityId = Guid.Parse("8cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    GenresEntity = genres[2]
                },
                new GenresBooks {
                    BookEntityId = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[2],
                    GenreEntityId = Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    GenresEntity = genres[1]
                },
            };

            inMemoryContext.Books.AddRange(books);
            inMemoryContext.Genres.AddRange(genres);
            inMemoryContext.GenresBooks.AddRange(bookGenres);
            inMemoryContext.Authors.AddRange(authors);
            inMemoryContext.AuthorsBooks.AddRange(bookAuthors);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public void Should_ReturnTrue_When_SearchingForAnExistingName()
        {
            var result = booksRepository!.FindBookByTitle(new AddBookDto() { BookTitle = "Book 1"});

            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ReturnFalse_When_SearchingForNotExistingName()
        {
            var result = booksRepository!.FindBookByTitle(new AddBookDto() { BookTitle = "Book 123" });

            Assert.AreEqual(default(BookEntity), result);
        }

        [Test]
        public async Task Should_ReturnTrue_When_SearchingForAnExistingNameWithADifferentGuid()
        {
            var result = await booksRepository!.ContainsBookName(Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Book 1");

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Should_ReturnFalse_When_SearchingForExistingNameWithExistingGuid()
        {
            var result = await booksRepository!.ContainsBookName(Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Book 1");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Should_ReturnAllBooks_When_CalledGetAllBooks()
        {
            var expectedOutput = new List<BookOutput>
            {
                new BookOutput { Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 1" },
                new BookOutput { Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 2" },
                new BookOutput { Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 3" },
            };

            var result = await booksRepository!.GetAllBooksAsync();

            Assert.IsTrue(expectedOutput.SequenceEqual(result, new MyBookEqualityComparer()));
        }

        [Test]
        public async Task GetCountOfAllBooksAsync_ShouldReturnCorrectNumberOfExistingBooks()
        {
            // Arrange
            int expectedResult = 3;

            // Act
            var actualResult = await booksRepository!.GetCountOfAllBooksAsync();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task GetBooksForLastTwoWeeksAsync_ShouldNotReturnBooksIfInLastTwoWeeksNewBooksWereNotAdded()
        {
            // Arrange
            var inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();

            inMemoryContext.Books.AddRange(new BookEntity(){ CreatedOn = new DateTime(2021, 10, 14), Title = "OldBook" });

            inMemoryContext.SaveChanges();

            var booksRepository = new BookRepository(inMemoryContext, null!);

            // Act
            var (books, count) = await booksRepository.GetBooksForLastTwoWeeksAsync(new PaginatorInputDto() { Page = 1, PageSize = 10 });

            // Assert
            Assert.IsEmpty(books);
            Assert.AreEqual(0, count);
        }

        [Test]
        public async Task GetBooksForLastTwoWeeksAsync_ShouldReturnCorrectCountOfBooksAddedInLastTwoWeeks()
        {
            // Arrange
            var inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();

            inMemoryContext.Books.AddRange(
                new BookEntity() { CreatedOn = new DateTime(2021, 10, 14), Title = "OldBook" },
                new BookEntity() { CreatedOn = DateTime.Today.AddDays(-10), Title = "NewBook-1" },
                new BookEntity() { CreatedOn = DateTime.Today.AddDays(-5), Title = "NewBook-2" });

            inMemoryContext.SaveChanges();

            var booksRepository = new BookRepository(inMemoryContext, null!);

            var expectedCount = 2;

            // Act
            var (books, actualCount) = await booksRepository.GetBooksForLastTwoWeeksAsync(new PaginatorInputDto() { Page = 1, PageSize = 10 });

            // Assert
            Assert.IsNotEmpty(books);
            Assert.AreEqual(expectedCount, actualCount);
        }

        [Test]
        public async Task GetBooksForLastTwoWeeksAsync_ShouldGetCorrectCountOfNewBooks()
        {
            // Arrange
            var inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();

            inMemoryContext.Books.AddRange(
                new BookEntity() { CreatedOn = new DateTime(2021, 10, 14), Title = "OldBook" },
                new BookEntity() { CreatedOn = DateTime.Today.AddDays(-10), Title = "NewBook-1" },
                new BookEntity() { CreatedOn = DateTime.Today.AddDays(-5), Title = "NewBook-2" });

            inMemoryContext.SaveChanges();

            var booksRepository = new BookRepository(inMemoryContext, null!);

            var expectedCountPerPage = 1;

            // Act
            var (books, actualCountPerPage) = await booksRepository.GetBooksForLastTwoWeeksAsync(new PaginatorInputDto() { Page = 1, PageSize = 10 });

            // Assert
            Assert.IsNotEmpty(books);
            Assert.AreNotEqual(expectedCountPerPage, actualCountPerPage);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingFirstPageByTitle()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 2
            };

            var searcher = new SearchBookDto
            {
                Title = "Book",
            };

            var expectedBooks = new List<BookEntity>
            {
                new BookEntity { Title = "Book 1", Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new BookEntity { Title = "Book 2", Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new BookEntity { Title = "Book 3", Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
            };

            int expectedCount = 3;

            var (resultEntities, resultCount) = await booksRepository!.SearchForBooksAsync(searcher, new List<Guid>(), new List<Guid>(), pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedBooks![0].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedBooks![0].Title, resultEntities[0].Title);
            Assert.AreEqual(expectedBooks![1].Id, resultEntities[1].Id);
            Assert.AreEqual(expectedBooks![1].Title, resultEntities[1].Title);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingSecondPageByTitle()
        {
            var pager = new PaginatorInputDto
            {
                Page = 2,
                PageSize = 2
            };

            var searcher = new SearchBookDto
            {
                Title = "Book",
            };

            var expectedBooks = new List<BookEntity>
            {
                new BookEntity { Title = "Book 1", Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new BookEntity { Title = "Book 2", Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new BookEntity { Title = "Book 3", Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
            };

            int expectedCount = 3;

            var (resultEntities, resultCount) = await booksRepository!.SearchForBooksAsync(searcher, new List<Guid>(), new List<Guid>(), pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedBooks![2].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedBooks![2].Title, resultEntities[0].Title);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingByAuthorGenreAndDescription()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 10
            };

            var validAuthors = new List<Guid>
            {
                Guid.Parse("2ad7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
            };

            var validGenres = new List<Guid>
            {
                Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
            };

            var searcher = new SearchBookDto
            {
                Description = "Lorem",
            };

            var expectedBooks = new List<BookEntity>
            {
                new BookEntity { Title = "Book 1", Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new BookEntity { Title = "Book 2", Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
                new BookEntity { Title = "Book 3", Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce")},
            };

            int expectedCount = 1;

            var (resultEntities, resultCount) = await booksRepository!.SearchForBooksAsync(searcher, validAuthors, validGenres, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedBooks![1].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedBooks![1].Title, resultEntities[0].Title);
        }

        [Test]
        public async Task Should_ReturnBlankList_When_SearchingForNonExistent()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 10
            };

            var searcher = new SearchBookDto
            {
                Description = "This is a description that will not be found",
            };

            int expectedCount = 0;

            var (resultEntities, resultCount) = await booksRepository!.SearchForBooksAsync(searcher, new List<Guid>(), new List<Guid>(), pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedCount, resultEntities.Count);
        }
    }
}
