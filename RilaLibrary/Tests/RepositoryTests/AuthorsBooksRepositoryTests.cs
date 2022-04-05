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
    public class AuthorsBooksRepositoryTests
    {
        private LibraryDbContext? inMemoryContext;
        private AuthorsBooksRepository? authorsBooksRepository;

        [SetUp]
        public void Init()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            authorsBooksRepository = new AuthorsBooksRepository(inMemoryContext);


            List<AuthorEntity> authors = new List<AuthorEntity>
            {
                new AuthorEntity { AuthorName = "John Smith",  Id = Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Manuel Alvarez",  Id = Guid.Parse("2227baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Author 1", Id = Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Unused author", Id = Guid.Parse("bdd34a5b-8785-4433-8bdf-1c8b16c2ab15") },
                new AuthorEntity { AuthorName = "Author with three books", Id = Guid.Parse("32b9f53e-a9e7-4a9d-8e7f-bda264011e1a") },
            };

            List<BookEntity> books = new List<BookEntity>
            {
                new BookEntity { Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 1" },
                new BookEntity { Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 2" },
                new BookEntity { Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 3" },
                new BookEntity { Id = Guid.Parse("c6c77b4c-d165-45a9-931e-4ed77ca6f57b"), Title = "Book 4" },
                new BookEntity { Id = Guid.Parse("8dea0226-2655-4f90-aa28-249814ca215b"), Title = "Book 5" },
                new BookEntity { Id = Guid.Parse("6cdc8257-2985-49e5-9ee8-b8bde9a86ce9"), Title = "Book 6" },
            };

            List<AuthorsBooks> authorsBooks = new List<AuthorsBooks>
            {
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[0],
                    AuthorEntityId = Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[0]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[1],
                    AuthorEntityId = Guid.Parse("2227baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[1]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[2],
                    AuthorEntityId = Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[2]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[2],
                    AuthorEntityId = Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    AuthorsEntity = authors[0]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("c6c77b4c-d165-45a9-931e-4ed77ca6f57b"),
                    BooksEntity = books[3],
                    AuthorEntityId = Guid.Parse("32b9f53e-a9e7-4a9d-8e7f-bda264011e1a"),
                    AuthorsEntity = authors[4]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("8dea0226-2655-4f90-aa28-249814ca215b"),
                    BooksEntity = books[4],
                    AuthorEntityId = Guid.Parse("32b9f53e-a9e7-4a9d-8e7f-bda264011e1a"),
                    AuthorsEntity = authors[4]
                },
                new AuthorsBooks {
                    BookEntityId = Guid.Parse("6cdc8257-2985-49e5-9ee8-b8bde9a86ce9"),
                    BooksEntity = books[5],
                    AuthorEntityId = Guid.Parse("32b9f53e-a9e7-4a9d-8e7f-bda264011e1a"),
                    AuthorsEntity = authors[4]
                },
            };

            inMemoryContext.Authors.AddRange(authors);
            inMemoryContext.Books.AddRange(books);
            inMemoryContext.AuthorsBooks.AddRange(authorsBooks);

            inMemoryContext.AuthorsBooks.AddRange(authorsBooks);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public async Task Should_Delete_OneRecord_When_SearchingForExistingBookIdWithOneAuthor()
        {
            var initialCount = inMemoryContext!.AuthorsBooks.Count();
            authorsBooksRepository!.DeleteAuthorEntriesForBook(Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"));
            await authorsBooksRepository.SaveAsync();

            var resultCount = inMemoryContext!.AuthorsBooks.Count();

            Assert.AreEqual(initialCount, resultCount + 1);
        }

        [Test]
        public async Task Should_Delete_TwoRecords_When_SearchingForExistingBookIdWithTwoAuthors()
        {
            var initialCount = inMemoryContext!.AuthorsBooks.Count();
            authorsBooksRepository!.DeleteAuthorEntriesForBook(Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"));
            await authorsBooksRepository.SaveAsync();

            var resultCount = inMemoryContext!.AuthorsBooks.Count();

            Assert.AreEqual(initialCount, resultCount + 2);
        }

        [Test]
        public async Task Should_NotDelete_When_SearchingForNonExistingBookId()
        {
            var initialCount = inMemoryContext!.AuthorsBooks.Count();
            authorsBooksRepository!.DeleteAuthorEntriesForBook(Guid.Parse("1243baea-311f-4387-9b9b-ef4c6ec8b5ce"));
            await authorsBooksRepository.SaveAsync();

            var resultCount = inMemoryContext!.AuthorsBooks.Count();

            Assert.AreEqual(initialCount, resultCount);
        }

        [Test]
        public async Task Should_ReturnZero_When_GettingBooksNumberForUnusedAuthor()
        {
            int expectedResult = 0;

            var result = await authorsBooksRepository!.GetBooksNumberForAuthorAsync(Guid.Parse("bdd34a5b-8785-4433-8bdf-1c8b16c2ab15"));

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task Should_ReturnOne_When_GettingBooksNumberForAuthorUsedInOneBook()
        {
            int expectedResult = 1;

            var result = await authorsBooksRepository!.GetBooksNumberForAuthorAsync(Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce"));

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task Should_ReturnThree_When_GettingBooksNumberForAuthorUsedInThreeBooks()
        {
            int expectedResult = 3;

            var result = await authorsBooksRepository!.GetBooksNumberForAuthorAsync(Guid.Parse("32b9f53e-a9e7-4a9d-8e7f-bda264011e1a"));

            Assert.AreEqual(expectedResult, result);
        }
    }
}
