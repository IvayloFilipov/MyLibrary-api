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
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository genreRepository;
        private readonly IGenresBooksService genresBooksService;
        private readonly IMapper mapper;
        private readonly ILog log = LogManager.GetLogger(typeof(GenreService));

        public GenreService(IGenreRepository genreRepository, IGenresBooksService genresBooksService, IMapper mapper)
        {
            this.genreRepository = genreRepository;
            this.genresBooksService = genresBooksService;
            this.mapper = mapper;
        }

        public async Task<GenreOutput> AddGenreAsync(Genre input)
        {
            input.Name = input.Name?.Trim()!;
            CheckIfGenreExistsOnAdd(input);

            //var createdEntity = Mapper.ToGenreEntity(input); //was
            var createdEntity = mapper.Map<GenreEntity>(input);

            await genreRepository.InsertAsync(createdEntity);
            await genreRepository.SaveAsync();

            //var result = Mapper.ToGenreOutput(createdEntity); //was
            var result = mapper.Map<GenreOutput>(createdEntity);

            return result;
        }

        public async Task<GenreOutput> UpdateGenreAsync(Guid genreId, Genre input)
        {
            input.Name = input.Name?.Trim()!;

            var existingEntity = await genreRepository.GetByIdAsync(genreId);

            CheckNullGenreByEntity(existingEntity);
            await CheckIfGenreExistsOnUpdate(genreId, input);

            existingEntity!.Name = input.Name;

            var updatedEntity = await genreRepository.UpdateAsync(existingEntity);
            await genreRepository.SaveAsync();

            //var result = Mapper.ToGenreOutput(updatedEntity); //was
            var result = mapper.Map<GenreOutput>(updatedEntity);

            return result;
        }

        public async Task DeleteGenreAsync(Guid genreId)
        {
            var existingEntity = await genreRepository.GetByIdAsync(genreId);

            CheckNullGenreByEntity(existingEntity);
            await CheckGenreBooksBeforeDelete(genreId);

            await genreRepository.DeleteAsync(genreId);
            await genreRepository.SaveAsync();
        }

        public async Task<(List<GenreOutput>, int)> GetGenresAsync(PaginatorInputDto input)
        {
            var (entities, totalCount) = await genreRepository.GetEntityPageAsync(input);

            CheckGenreErrorByTotalCount(totalCount);

            var result = new List<GenreOutput>();

            //entities.ForEach(entity => result.Add(Mapper.ToGenreOutput(entity))); //was
            entities.ForEach(entity => result.Add(mapper.Map<GenreOutput>(entity)));

            return (result, totalCount);
        }

        public async Task<List<GenreOutput>> GetAllGenresAsync()
        {
            var result = await genreRepository.GetAllGenresAsync();

            CheckGenreErrorByTotalCount(result.Count);

            return result;
        }

        public async Task<GenreOutput> GetGenreByIdAsync(Guid genreId)
        {
            var existingEntity = await genreRepository.GetByIdAsync(genreId);

            CheckNullGenreByEntity(existingEntity);

            //var result = Mapper.ToGenreOutput(existingEntity!); //was
            var result = mapper.Map<GenreOutput>(existingEntity!);

            return result;
        }

        public async Task<int> GetCountOfAllGenresAsync()
        {
            var allGenresCount = await genreRepository.GetCountOfAllGenresAsync();

            return allGenresCount;
        }

        public async Task<(List<GenreOutput>, int)> SearchForGenresAsync(SearchGenreDto input, PaginatorInputDto pagination)
        {
            var (result, totalCount) = await genreRepository.SearchForGenresAsync(input, pagination);

            CheckGenreErrorByTotalCount(totalCount);

            return (result, totalCount);
        }

        public GenreEntity FindGenreByName(string genre)
        {
            var result = genreRepository.FindGenreByName(genre);

            return result;
        }

        public List<string> FindGenresByBookId(Guid bookId)
        {
            var result = genreRepository.FindGenresByBookId(bookId);

            return result;
        }

        public async Task<List<Guid>> FindMultipleGenresByNameAsync(string name)
        {
            var result = await genreRepository.FindMultipleGenresByNameAsync(name);

            return result;
        }

        public async Task<int> GetBooksNumberForGenreAsync(Guid genreId)
        {
            var result = await genresBooksService.GetBooksNumberForGenreAsync(genreId);

            return result;
        }

        public void CheckGenreErrorByTotalCount(int count)
        {
            if (count == 0)
            {
                log.Error($"Exception is thrown {NO_GENRES_FOUND}");
                throw new NullReferenceException(NO_GENRES_FOUND);
            }
        }

        public void CheckNullGenreByEntity(GenreEntity? entity)
        {
            if (entity == null)
            {
                log.Error($"Genre is null. Exception is thrown {GENRE_NOT_FOUND}");
                throw new NullReferenceException(GENRE_NOT_FOUND);
            }
        }

        public void CheckIfGenreExistsOnAdd(Genre input)
        {
            if (genreRepository.ContainsGenreName(input.Name))
            {
                log.Error($"Exception is thrown {GENRE_EXISTS}");
                throw new ArgumentException(GENRE_EXISTS);
            }
        }

        public async Task CheckIfGenreExistsOnUpdate(Guid genreId, Genre input)
        {
            var exists = await genreRepository.ContainsGenreName(genreId, input.Name);

            if (exists)
            {
                log.Error($"Exception is thrown {GENRE_EXISTS}");
                throw new ArgumentException(GENRE_EXISTS);
            }
        }

        public async Task CheckGenreBooksBeforeDelete(Guid genreId)
        {
            var genreBooks = await GetBooksNumberForGenreAsync(genreId);

            if (genreBooks > 0)
            {
                log.Error($"Exception is thrown {GENRE_HAS_BOOKS}");
                throw new ArgumentException(GENRE_HAS_BOOKS);
            }
        }
    }
}
