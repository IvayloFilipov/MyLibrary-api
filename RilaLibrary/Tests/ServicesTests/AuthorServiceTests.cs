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
    public class AuthorServiceTests
    {
        readonly Mock<IAuthorRepository> mockAuthorRepo = new Mock<IAuthorRepository>();
        readonly Mock<IAuthorsBooksService> mockAuthorsBooksService = new Mock<IAuthorsBooksService>();
        AuthorService? authorService;
        AuthorDto input = new AuthorDto();

        [SetUp]
        public void SetUp()
        {
            authorService = new AuthorService(mockAuthorRepo.Object, mockAuthorsBooksService.Object, null!);

            input = new AuthorDto();
        }

        [Test]
        public async Task Should_ReturnAuthor_When_AddingNewAuthor()
        {
            mockAuthorRepo.Setup(x => x.InsertAsync(It.IsAny<AuthorEntity>())).ReturnsAsync(new AuthorEntity());

            AuthorOutput output = await authorService!.AddAuthorAsync(input);

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<AuthorOutput>(output);
        }

        [Test]
        public void Should_ThrowExpection_When_InputAuthorExists()
        {
            mockAuthorRepo.Setup(x => x.FindAuthorByName(It.IsAny<string>())).Returns(new AuthorEntity());

            Assert.ThrowsAsync<ArgumentException>(async Task () => await authorService!.AddAuthorAsync(input), "Author already exists");
        }

        [Test]
        public async Task Should_ReturnResult_When_CallingGetAll()
        {
            mockAuthorRepo.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(new List<AuthorOutput>() { new AuthorOutput()});

            List<AuthorOutput> output = await authorService!.GetAllAuthorsAsync();

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<List<AuthorOutput>>(output);
        }

        [Test]
        public void Should_ThrowException_When_CallingGetAllAuthorsNoAuthorsAreFound()
        {
            mockAuthorRepo.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(new List<AuthorOutput>());

            var result = Assert.ThrowsAsync<NullReferenceException>(async Task () => await authorService!.GetAllAuthorsAsync());
            Assert.AreEqual(NO_AUTHORS_FOUND, result!.Message);
        }

        [Test]
        public async Task Should_ReturnResult_When_GettingAuthors()
        {
            List<AuthorEntity> outputList = new List<AuthorEntity> { new AuthorEntity() };
            mockAuthorRepo.Setup(x => x.GetEntityPageAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((outputList, 25));

            (List<AuthorOutput> output, int count) = await authorService!.GetAuthorsAsync(new PaginatorInputDto());

            Assert.IsNotNull(output);
            Assert.IsNotNull(count);
        }

        [Test]
        public void Should_ThrowException_When_CallingGetAuthorsNoAuthorsAreFound()
        {
            mockAuthorRepo.Setup(x => x.GetEntityPageAsync(It.IsAny<PaginatorInputDto>()))!.ReturnsAsync((default(List<AuthorEntity>), 25));

            Assert.ThrowsAsync<NullReferenceException>(async Task () => await authorService!.GetAuthorsAsync(new PaginatorInputDto()), "No authors found.");
        }

        [Test]
        public async Task Should_ReturnResult_When_CallingGetAuthorById()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new AuthorEntity());

            AuthorOutput output = await authorService!.GetAuthorByIdAsync(Guid.NewGuid());

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<AuthorOutput>(output);
        }

        [Test]
        public void Should_ThrowException_When_CallingGetAuthorByIdNoAuthorsAreFound()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(AuthorEntity));

            Assert.ThrowsAsync<NullReferenceException>(async Task () => await authorService!.GetAuthorByIdAsync(Guid.NewGuid()), "Author not found");
        }

        [Test]
        public async Task Should_ReturnResult_When_UpdatingAuthor()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new AuthorEntity());
            mockAuthorRepo.Setup(x => x.UpdateAsync(It.IsAny<AuthorEntity>())).ReturnsAsync(new AuthorEntity());

            AuthorOutput output = await authorService!.UpdateAuthorAsync(input, Guid.NewGuid());

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<AuthorOutput>(output);
        }

        [Test]
        public void Should_ThrowException_When_UpdatingAuthorIsNotFound()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(AuthorEntity));

            Assert.ThrowsAsync<NullReferenceException>(async Task () => await authorService!.UpdateAuthorAsync(input, Guid.NewGuid()), "Author not found");
        }

        [Test]
        public void Should_Complete_When_DeletingAuthor()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new AuthorEntity());
            mockAuthorsBooksService.Setup(x => x.GetBooksNumberForAuthorAsync(It.IsAny<Guid>())).ReturnsAsync(0);

            Assert.That(authorService!.DeleteAuthorAsync(Guid.NewGuid()).IsCompleted);
        }

        [Test]
        public void Should_ThrowException_When_DeletingAuthorIsNotFound()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(default(AuthorEntity));

            var result = Assert.ThrowsAsync<NullReferenceException>(async Task () => await authorService!.DeleteAuthorAsync(Guid.NewGuid()));
            Assert.AreEqual(AUTHOR_NOT_FOUND, result!.Message);
        }

        [Test]
        public void Should_ThrowException_When_DeletingAuthorIsAssignedToBooks()
        {
            mockAuthorRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new AuthorEntity());
            mockAuthorsBooksService.Setup(x => x.GetBooksNumberForAuthorAsync(It.IsAny<Guid>())).ReturnsAsync(1);

            var result = Assert.ThrowsAsync<ArgumentException>(async Task () => await authorService!.DeleteAuthorAsync(Guid.NewGuid()));
            Assert.AreEqual(AUTHOR_HAS_BOOKS, result!.Message);
        }

        [Test]
        public async Task Should_Return_AuthorsAndCount_When_SearchingAuthorsPaged()
        {
            var authorOutput = new AuthorOutput
            {
                AuthorName = "My author",
                Id = Guid.Parse("453629f8-71c9-457f-a460-433fe9ed14ee")
            };

            mockAuthorRepo.Setup(x => x.SearchForAuthorsAsync(It.IsAny<SearchAuthorDto>(), It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<AuthorOutput> { authorOutput }, 1));

            var (resultList, resultCount) = await authorService!.SearchForAuthorsAsync(new SearchAuthorDto(), new PaginatorInputDto());
            Assert.IsNotNull(resultList);
            Assert.IsNotNull(resultCount);
        }

        [Test]
        public void Should_ThrowNullReferenceException_When_SearchNoAuthorsFoundPaged()
        {
            var searchDto = new SearchAuthorDto
            {
                AuthorName = "Author name"
            };

            mockAuthorRepo.Setup(x => x.SearchForAuthorsAsync(It.IsAny<SearchAuthorDto>(), It.IsAny<PaginatorInputDto>()))!.ReturnsAsync((default(List<AuthorOutput>), default(int)));

            var result = Assert.ThrowsAsync<NullReferenceException>(async () => await authorService!.SearchForAuthorsAsync(searchDto, new PaginatorInputDto()));
            Assert.AreEqual(NO_AUTHORS_FOUND, result!.Message);
        }

        [Test]
        public void Should_ReturnResult_When_SearchingAuthorByName()
        {
            mockAuthorRepo.Setup(x => x.FindAuthorByName(It.IsAny<string>())).Returns(new AuthorEntity());

            var result = authorService!.FindAuthorByName("Some author");
            Assert.IsNotNull(result);
        }

        [Test]
        public void Should_ReturnResult_When_SearchingAuthorByBookId()
        {
            mockAuthorRepo.Setup(x => x.FindAuthorsByBookId(It.IsAny<Guid>())).Returns(new List<string>());

            var result = authorService!.FindAuthorsByBookId(Guid.NewGuid());
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Should_ReturnResult_When_SearchingMultipleAuthorsByName()
        {
            mockAuthorRepo.Setup(x => x.FindMultipleAuthorsByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<Guid>());

            var result = await authorService!.FindMultipleAuthorsByNameAsync("Some author");
            Assert.IsNotNull(result);
        }
    }
}
