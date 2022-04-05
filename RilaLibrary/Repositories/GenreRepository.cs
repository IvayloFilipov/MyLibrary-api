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
    public class GenreRepository : GenericRepository<GenreEntity>, IGenreRepository
    {
        private readonly IMapper mapper;

        public GenreRepository(LibraryDbContext context, IMapper mapper) 
            : base(context)
        {
            this.mapper = mapper;
        }

        public bool ContainsGenreName(string genreName)
        {
            var result = context.Genres
                .Any(x => x.Name == genreName);

            return result;
        }

        public GenreEntity FindGenreByName(string genre)
        {
            var currGenre = context.Genres
                .Where(x => x.Name == genre)
                .FirstOrDefault()!;

            return currGenre;
        }

        public List<string> FindGenresByBookId(Guid bookId)
        {
            var genreNames = new List<string>();

            var genreIds = context.GenresBooks
                .Where(x => x.BookEntityId == bookId)
                .Select(x => x.GenreEntityId)
                .ToList();

            foreach (Guid id in genreIds)
            {
                var genreName = context.Genres
                    .FirstOrDefault(g => g.Id == id)!
                    .Name;

                genreNames.Add(genreName);
            }

            return genreNames;
        }

        public async Task<bool> ContainsGenreName(Guid genreId, string genreName)
        {
            var result = await context.Genres
                .FirstOrDefaultAsync(x => x.Name == genreName);

            if (result == null || result.Id == genreId)
            {
                return false;
            }

            return true;
        }

        public async Task<List<GenreOutput>> GetAllGenresAsync()
        {
            var genres = await context.Genres
                //.Select(x => Mapper.ToGenreOutput(x)) //was
                .ProjectTo<GenreOutput>(mapper.ConfigurationProvider)
                .ToListAsync();

            return genres;
        }

        public async Task<int> GetCountOfAllGenresAsync()
        {
            var allGenres = await context.GenresBooks
                .Select(x => x.GenreEntityId)
                .Distinct()
                .CountAsync();

            return allGenres;
        }

        public async Task<List<Guid>> FindMultipleGenresByNameAsync(string name)
        {
            var result = await context.Genres
                .Where(x => x.Name.Contains(name))
                .Select(x => x.Id)
                .ToListAsync();

            return result;
        }

        public async Task<(List<GenreOutput>, int)> SearchForGenresAsync(SearchGenreDto input, PaginatorInputDto pagination)
        {
            var query = this.context.Genres
                .Where(x => input.Name == null || x.Name.Contains(input.Name));

            var result = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsSplitQuery()
                //.Select(x => Mapper.ToGenreOutput(x)) //was
                .ProjectTo<GenreOutput>(mapper.ConfigurationProvider)
                .ToListAsync();

            var totalCount = query.Count();

            return (result, totalCount);
        }
    }
}
