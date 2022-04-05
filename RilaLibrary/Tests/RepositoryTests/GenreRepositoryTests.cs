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
    public class GenreRepositoryTests
    {
        private LibraryDbContext? inMemoryContext;
        private GenreRepository? genreRepository;

        [SetUp]
        public void Init()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            genreRepository = new GenreRepository(inMemoryContext, null!);

            List<GenreEntity> genres = new List<GenreEntity>
            {
                new GenreEntity { Name = "Thriller", Id = Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreEntity { Name = "Sci-Fi", Id = Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreEntity { Name = "Fantasy", Id = Guid.Parse("8cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreEntity { Name = "Genre 1", Id = Guid.Parse("291f7510-6f1c-4240-b5d9-883d9733ea77") },
                new GenreEntity { Name = "Genre 2", Id = Guid.Parse("ea916f3f-2b6c-484f-abde-31e810f667e4") },
                new GenreEntity { Name = "Genre 3", Id = Guid.Parse("64ecbe9c-e38a-43d4-b808-76ee728d811f") },
            };

            List<BookEntity> books = new List<BookEntity>
            {
                new BookEntity { Id = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 1" },
                new BookEntity { Id = Guid.Parse("2cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 2" },
                new BookEntity { Id = Guid.Parse("3cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), Title = "Book 3" },
            };

            List<GenresBooks> bookGenres = new List<GenresBooks>
            {
                new GenresBooks {
                    BookEntityId = Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    BooksEntity = books[0],
                    GenreEntityId = Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"),
                    GenresEntity = genres[0]
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
            };

            inMemoryContext.Genres.AddRange(genres);
            inMemoryContext.Books.AddRange(books);
            inMemoryContext.GenresBooks.AddRange(bookGenres);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public void Should_ReturnTrue_When_SearchingForAnExistingName()
        {
            var result = genreRepository!.ContainsGenreName("Thriller");

            Assert.IsTrue(result);
        }

        [Test]
        public void Should_ReturnFalse_When_SearchingForNotExistingName()
        {
            var result = genreRepository!.ContainsGenreName("Action");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Should_ReturnTrue_When_SearchingForAnExistingNameWithADifferentGuid()
        {
            var result = await genreRepository!.ContainsGenreName(Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Thriller");

            Assert.IsTrue(result);
        }

        [Test]
        public void Should_ReturnGenre_When_SearchingForAnExistingName()
        {
            var expectedResult = new GenreEntity { Name = "Thriller", Id = Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") };
            var result = genreRepository!.FindGenreByName("Thriller");

            Assert.AreEqual(expectedResult.Id, result.Id);
            Assert.AreEqual(expectedResult.Name, result.Name);
        }

        [Test]
        public void Should_ReturnDefault_When_SearchingForNotExistingName()
        {
            var result = genreRepository!.FindGenreByName("Action");

            Assert.AreEqual(default(GenreEntity), result);
        }

        [Test]
        public void Should_ReturnGenresNames_When_SearchingForAnExistingBook()
        {
            var expectedResult = new List<string> { "Thriller" };
            var result = genreRepository!.FindGenresByBookId(Guid.Parse("1cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"));

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task Should_ReturnFalse_When_SearchingForExistingNameWithExistingGuid()
        {
            var result = await genreRepository!.ContainsGenreName(Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce"), "Thriller");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task Should_ReturnAllGenres_When_CalledGetAllGenres()
        {
            var expectedOutput = new List<GenreOutput>
            {
                new GenreOutput { Name = "Thriller", Id = Guid.Parse("6cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreOutput { Name = "Sci-Fi", Id = Guid.Parse("7cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreOutput { Name = "Fantasy", Id = Guid.Parse("8cd7baea-311f-4387-9b9b-ef4c6ec8b5ce") },
                new GenreOutput { Name = "Genre 1", Id = Guid.Parse("291f7510-6f1c-4240-b5d9-883d9733ea77") },
                new GenreOutput { Name = "Genre 2", Id = Guid.Parse("ea916f3f-2b6c-484f-abde-31e810f667e4") },
                new GenreOutput { Name = "Genre 3", Id = Guid.Parse("64ecbe9c-e38a-43d4-b808-76ee728d811f") },
            };

            var result = await genreRepository!.GetAllGenresAsync();

            Assert.IsTrue(expectedOutput.SequenceEqual(result, new MyGenreEqualityComparer()));
        }

        [Test]
        public async Task GetCountOfAllGenresAsync_ShouldReturnCorrectNumberOfExistingGenres()
        {
            // Arrange
            int expectedResult = 3;

            // Act
            var actualResult = await genreRepository!.GetCountOfAllGenresAsync();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingFirstPageByGenreName()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 2
            };

            var searcher = new SearchGenreDto
            {
                Name = "Genre",
            };

            var expectedGenres = new List<GenreEntity>
            {
                new GenreEntity { Name = "Genre 1", Id = Guid.Parse("291f7510-6f1c-4240-b5d9-883d9733ea77") },
                new GenreEntity { Name = "Genre 2", Id = Guid.Parse("ea916f3f-2b6c-484f-abde-31e810f667e4") },
                new GenreEntity { Name = "Genre 3", Id = Guid.Parse("64ecbe9c-e38a-43d4-b808-76ee728d811f") },
            };

            int expectedCount = 3;

            var (resultEntities, resultCount) = await genreRepository!.SearchForGenresAsync(searcher, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedGenres![0].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedGenres![0].Name, resultEntities[0].Name);
            Assert.AreEqual(expectedGenres![1].Id, resultEntities[1].Id);
            Assert.AreEqual(expectedGenres![1].Name, resultEntities[1].Name);
        }

        [Test]
        public async Task Should_ReturnList_When_SearchingSecondPageByGenreName()
        {
            var pager = new PaginatorInputDto
            {
                Page = 2,
                PageSize = 2
            };

            var searcher = new SearchGenreDto
            {
                Name = "Genre",
            };

            var expectedGenres = new List<GenreEntity>
            {
                new GenreEntity { Name = "Genre 1", Id = Guid.Parse("291f7510-6f1c-4240-b5d9-883d9733ea77") },
                new GenreEntity { Name = "Genre 2", Id = Guid.Parse("ea916f3f-2b6c-484f-abde-31e810f667e4") },
                new GenreEntity { Name = "Genre 3", Id = Guid.Parse("64ecbe9c-e38a-43d4-b808-76ee728d811f") },
            };

            int expectedCount = 3;

            var (resultEntities, resultCount) = await genreRepository!.SearchForGenresAsync(searcher, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedGenres![2].Id, resultEntities[0].Id);
            Assert.AreEqual(expectedGenres![2].Name, resultEntities[0].Name);
        }

        [Test]
        public async Task Should_ReturnBlankList_When_SearchingForNonExistentGenreName()
        {
            var pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 10
            };

            var searcher = new SearchGenreDto
            {
                Name = "This is a genre that will not be found",
            };

            int expectedCount = 0;

            var (resultEntities, resultCount) = await genreRepository!.SearchForGenresAsync(searcher, pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(expectedCount, resultEntities.Count);
        }
    }
}
