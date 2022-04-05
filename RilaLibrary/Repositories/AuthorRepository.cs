using Microsoft.EntityFrameworkCore;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;
using Repositories.Mappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Repositories
{
    public class AuthorRepository : GenericRepository<AuthorEntity>, IAuthorRepository
    {
        private readonly IMapper mapper;

        public AuthorRepository(LibraryDbContext context, IMapper mapper)
           : base(context)
        {
            this.mapper = mapper;
        }

        public async Task<AuthorEntity> GetByIdAsync(Guid id)
        {
            var existAuthor = await context.Authors
                .FirstOrDefaultAsync(x => x.Id == id);

            return existAuthor!;
        }

        public async Task<List<AuthorOutput>> GetAllAuthorsAsync()
        {
            var authors = await context.Authors
                //.Select(x => Mapper.ToAuthorOutput(x)) //was
                .ProjectTo<AuthorOutput>(mapper.ConfigurationProvider)
                .ToListAsync();

            return authors;
        }

        public AuthorEntity FindAuthorByName(string author)
        {
            var authors = context.Authors
                .Where(x => x.AuthorName == author)
                .FirstOrDefault()!;

            return authors;
        }

        public async Task<List<Guid>> FindMultipleAuthorsByNameAsync(string name)
        {
            var result = await context.Authors
                .Where(x => x.AuthorName.Contains(name))
                .Select(x => x.Id)
                .ToListAsync();

            return result;
        }

        public List<string> FindAuthorsByBookId(Guid bookId)
        {
            var authorNames = new List<string>();

            var authorIds = context.AuthorsBooks
                .Where(x => x.BookEntityId == bookId)
                .Select(x => x.AuthorEntityId)
                .ToList();

            foreach (Guid id in authorIds)
            {
                var authorName = context.Authors
                    .FirstOrDefault(a => a.Id == id)!
                    .AuthorName;

                authorNames.Add(authorName);
            }

            return authorNames;
        }

        public Task<bool> ContainsAuthor(Guid id, string name)
        {
            var author = FindAuthorByName(name);

            if (author is null || author.Id == id)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task<(List<AuthorOutput>, int)> SearchForAuthorsAsync(SearchAuthorDto input, PaginatorInputDto pagination)
        {
            var query = context.Authors
                .Where(x => input.AuthorName == null || x.AuthorName.Contains(input.AuthorName));

            var result = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsSplitQuery()
                //.Select(x => Mapper.ToAuthorOutput(x)) //was
                .ProjectTo<AuthorOutput>(mapper.ConfigurationProvider)
                .ToListAsync();

            var totalCount = query.Count();

            return (result, totalCount);
        }
    }
}
