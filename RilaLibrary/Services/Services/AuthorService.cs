using log4net;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
using Repositories.Interfaces;
using Repositories.Mappers;
using Services.Interfaces;
using AutoMapper;

using static Common.ExceptionMessages;

namespace Services.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IAuthorsBooksService authorsBooksService;
        private readonly IMapper mapper;
        private readonly ILog log = LogManager.GetLogger(typeof(AuthorService));

        public AuthorService(IAuthorRepository authorRepository, IAuthorsBooksService authorsBooksService, IMapper mapper)
        {
            this.authorRepository = authorRepository;
            this.authorsBooksService = authorsBooksService;
            this.mapper = mapper;
        }

        public async Task<AuthorOutput> AddAuthorAsync(AuthorDto input)
        {
            CheckAuthorNameOnAdd(input);

            //var authorEntity = Mapper.ToAuthorEntity(input.AuthorName); //was
            var authorEntity = mapper.Map<AuthorEntity>(input);

            await authorRepository.InsertAsync(authorEntity);
            await authorRepository.SaveAsync();

            //var authorOutput = Mapper.ToAuthorOutput(authorEntity); //was
            var authorOutput = mapper.Map<AuthorOutput>(authorEntity);

            return authorOutput;
        }

        public async Task<List<AuthorOutput>> GetAllAuthorsAsync()
        {
            var authors = await authorRepository.GetAllAuthorsAsync();

            CheckAuthorsErrorByTotalCount(authors.Count);
            
            return authors;
        }

        public async Task<(List<AuthorOutput>, int)> GetAuthorsAsync(PaginatorInputDto input)
        {
            var (entities, totalCount) = await authorRepository.GetEntityPageAsync(input);

            CheckAuthorsErrorByTotalCount(totalCount);

            var result = new List<AuthorOutput>();

            //entities.ForEach(entity => result.Add(Mapper.ToAuthorOutput(entity))); //was
            entities.ForEach(entity => result.Add(mapper.Map<AuthorOutput>(entity)));

            return (result, totalCount);
        }

        public async Task<AuthorOutput> GetAuthorByIdAsync(Guid authorId)
        {
            var entity = await authorRepository.GetByIdAsync(authorId);

            CheckNullAuthorByEntity(entity);

            //var authorOutput = Mapper.ToAuthorOutput(entity!); //was
            var authorOutput = mapper.Map<AuthorOutput>(entity);

            return authorOutput;
        }

        public async Task<AuthorOutput> UpdateAuthorAsync(AuthorDto input, Guid authorId)
        {
            input.AuthorName = input.AuthorName.Trim();

            var entity = await authorRepository.GetByIdAsync(authorId);

            CheckNullAuthorByEntity(entity);

            await CheckAuthorNameOnUpdateAsync(input, authorId);

            entity!.AuthorName = input.AuthorName;
            var updated = await authorRepository.UpdateAsync(entity);
            await authorRepository.SaveAsync();

            //var authorOutput = Mapper.ToAuthorOutput(updated); //was
            var authorOutput = mapper.Map<AuthorOutput>(updated);

            return authorOutput;
        }

        public async Task<(List<AuthorOutput>, int)> SearchForAuthorsAsync(SearchAuthorDto input, PaginatorInputDto pagination)
        {
            var (result, totalCount) = await authorRepository.SearchForAuthorsAsync(input, pagination);

            CheckAuthorsErrorByTotalCount(totalCount);

            return (result, totalCount);
        }

        public async Task DeleteAuthorAsync(Guid authorId)
        {
            var entity = await authorRepository.GetByIdAsync(authorId);

            CheckNullAuthorByEntity(entity);

            await CheckAuthorBooksBeforeDeletingAsync(authorId);

            await authorRepository.DeleteAsync(authorId);
            await authorRepository.SaveAsync();
        }

        public async Task<int> GetBooksNumberForAuthorAsync(Guid authorId)
        {
            var result = await authorsBooksService.GetBooksNumberForAuthorAsync(authorId);

            return result;
        }

        public AuthorEntity FindAuthorByName(string author)
        {
            var result = authorRepository.FindAuthorByName(author);
            return result;
        }

        public List<string> FindAuthorsByBookId(Guid bookId)
        {
            var result = authorRepository.FindAuthorsByBookId(bookId);
            return result;
        }

        public async Task<List<Guid>> FindMultipleAuthorsByNameAsync(string name)
        {
            var result = await authorRepository.FindMultipleAuthorsByNameAsync(name);
            return result;
        }

        public void CheckAuthorsErrorByTotalCount(int count)
        {
            if (count == 0)
            {
                log.Error($"Authors are 0. Exception is thrown: {NO_AUTHORS_FOUND}");
                throw new NullReferenceException(NO_AUTHORS_FOUND);
            }
        }

        public void CheckNullAuthorByEntity(AuthorEntity? entity)
        {
            if (entity is null)
            {
                log.Error($"Author is null. Exception is thrown: {AUTHOR_NOT_FOUND}");
                throw new NullReferenceException(AUTHOR_NOT_FOUND);
            }
        }

        public AuthorDto CheckAuthorNameOnAdd(AuthorDto input)
        {
            input.AuthorName = input.AuthorName?.Trim()!;

            if (FindAuthorByName(input.AuthorName) is not null)
            {
                log.Error($"There is author with that name. Exception is thrown: {AUTHOR_EXISTS}");
                throw new ArgumentException(AUTHOR_EXISTS);
            }

            return input;
        }

        public async Task CheckAuthorNameOnUpdateAsync(AuthorDto input, Guid authorId)
        {
            var isContainsAuthor = await authorRepository.ContainsAuthor(authorId, input.AuthorName);

            if (isContainsAuthor)
            {
                log.Error($"Authot is found. Exception is thrown: {AUTHOR_EXISTS}");
                throw new ArgumentException(AUTHOR_EXISTS);
            }
        }

        public async Task CheckAuthorBooksBeforeDeletingAsync(Guid authorId)
        {
            var authorBooks = await GetBooksNumberForAuthorAsync(authorId);

            if (authorBooks > 0)
            {
                log.Error($"Exception is thrown: {AUTHOR_HAS_BOOKS}");
                throw new ArgumentException(AUTHOR_HAS_BOOKS);
            }
        }
    }
}
