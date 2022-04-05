using DataAccess;
using Repositories;
using DataAccess.Entities;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.InputDTOs;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class AuthorRepositoryTests
    {
        private LibraryDbContext? inMemoryContext;
        private AuthorRepository? authorRepository;

        [SetUp]
        public void Init()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            authorRepository = new AuthorRepository(inMemoryContext, null!);

            List<AuthorEntity> authors = new List<AuthorEntity>
            {
                new AuthorEntity { AuthorName = "John Smith",  Id = Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Manuel Alvarez",  Id = Guid.Parse("2227baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Author 1", Id = Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Author X", Id = Guid.Parse("77ec329c-bc58-431c-8ab5-2aa66b5be951") },
                new AuthorEntity { AuthorName = "Author Y", Id = Guid.Parse("3c78ebfa-dffc-4a18-8ca2-5ebf90623773") },
            };

            List<BookEntity> books = new List<BookEntity>
            {
                new BookEntity { Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 1" },
                new BookEntity { Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 2" },
                new BookEntity { Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 3" },
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
            };

            inMemoryContext.Authors.AddRange(authors);
            inMemoryContext.Books.AddRange(books);
            inMemoryContext.AuthorsBooks.AddRange(authorsBooks);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public void Should_ReturnAuthor_When_SearchingForAnExistingName()
        {
            var expectedResult = new AuthorEntity { AuthorName = "John Smith", Id = Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce") };
            var result = authorRepository!.FindAuthorByName("John Smith");

            Assert.AreEqual(expectedResult.Id, result.Id);
            Assert.AreEqual(expectedResult.AuthorName, result.AuthorName);
        }

        [Test]
        public void Should_ReturnDefault_When_SearchingForNotExistingName()
        {
            var result = authorRepository!.FindAuthorByName("Non-existing author");

            Assert.AreEqual(default(AuthorEntity), result);
        }

        [Test]
        public void Should_ReturnAuthorsNames_When_SearchingForAnExistingBook()
        {
            var expectedResult = new List<string> { "John Smith" };
            var result = authorRepository!.FindAuthorsByBookId(Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"));

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Should_ReturnAuthor_When_SearchingForAnExistingId()
        {
            var expectedResult = new AuthorEntity { AuthorName = "John Smith", Id = Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce") };
            var result = authorRepository!.GetByIdAsync(Guid.Parse("1117baea-311f-4387-9b9b-ef4c6ec8b5ce"));

            Assert.AreEqual(expectedResult.Id, Task.FromResult(result).Result.Result.Id);
            Assert.AreEqual(expectedResult.AuthorName, Task.FromResult(result).Result.Result.AuthorName);
        }

        [Test]
        public void Should_ReturnDefault_When_SearchingForNotExistingId()
        {
            var result = authorRepository!.GetByIdAsync(Guid.Parse("8888baea-311f-4387-9b9b-ef4c6ec8b5ce"));

            Assert.AreEqual(default(AuthorEntity), Task.FromResult(result).Result.Result);
        }

        [Test]
        public void ContainsAuthor_Return_True_When_AuthorName_Exists()
        {
            var result = authorRepository!.ContainsAuthor(Guid.Parse("5337baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Author 1");

            Assert.IsTrue(result.Result);
        }

        [Test]
        public void ContainsAuthor_Return_False_When_Author_Not_Exists()
        {
            var result = authorRepository!.ContainsAuthor(Guid.Parse("5337baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Author 2");

            Assert.IsFalse(result.Result);
        }

        [Test]
        public void ContainsAuthor_Return_False_When_Find_Current_Author()
        {
            var result = authorRepository!.ContainsAuthor(Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Author 1");

            Assert.IsFalse(result.Result);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingFirstPageByAuthorName()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 2
            };

            var searcher = new SearchAuthorDto
            {
                AuthorName = "Author",
            };

            var expectedAuthors = new List<AuthorEntity>
            {
                new AuthorEntity { AuthorName = "Author 1", Id = Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Author X", Id = Guid.Parse("77ec329c-bc58-431c-8ab5-2aa66b5be951") },
                new AuthorEntity { AuthorName = "Author Y", Id = Guid.Parse("3c78ebfa-dffc-4a18-8ca2-5ebf90623773") },
            };

            int expectedCount = 3;

            var (resultEntities, resultCount) = await authorRepository!.SearchForAuthorsAsync(searcher, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedAuthors![0].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedAuthors![0].AuthorName, resultEntities[0].AuthorName);
            Assert.AreEqual(expectedAuthors![1].Id, resultEntities[1].Id);
            Assert.AreEqual(expectedAuthors![1].AuthorName, resultEntities[1].AuthorName);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingSecondPageByAuthorName()
        {
            var pager = new PaginatorInputDto
            {
                Page = 2,
                PageSize = 2
            };

            var searcher = new SearchAuthorDto
            {
                AuthorName = "Author",
            };

            var expectedAuthors = new List<AuthorEntity>
            {
                new AuthorEntity { AuthorName = "Author 1", Id = Guid.Parse("3337baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new AuthorEntity { AuthorName = "Author X", Id = Guid.Parse("77ec329c-bc58-431c-8ab5-2aa66b5be951") },
                new AuthorEntity { AuthorName = "Author Y", Id = Guid.Parse("3c78ebfa-dffc-4a18-8ca2-5ebf90623773") },
            };

            int expectedCount = 3;

            var (resultEntities, resultCount) = await authorRepository!.SearchForAuthorsAsync(searcher, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedAuthors![2].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedAuthors![2].AuthorName, resultEntities[0].AuthorName);
        }

        [Test]
        public async Task Should_ReturnBlankList_When_SearchingForNonExistentAuthorName()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 10
            };

            var searcher = new SearchAuthorDto
            {
                AuthorName = "This is an author that will not be found",
            };

            int expectedCount = 0;

            var (resultEntities, resultCount) = await authorRepository!.SearchForAuthorsAsync(searcher, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedCount, resultEntities.Count);
        }
    }
}
