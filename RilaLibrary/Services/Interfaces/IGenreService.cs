using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Services.Interfaces
{
    public interface IGenreService
    {
        Task<GenreOutput> AddGenreAsync(Genre input);

        Task<GenreOutput> UpdateGenreAsync(Guid genreId, Genre input);

        Task DeleteGenreAsync(Guid genreId);

        Task<(List<GenreOutput>, int)> GetGenresAsync(PaginatorInputDto input);

        Task<List<GenreOutput>> GetAllGenresAsync();

        Task<GenreOutput> GetGenreByIdAsync(Guid genreId);

        Task<int> GetCountOfAllGenresAsync();

        Task<(List<GenreOutput>, int)> SearchForGenresAsync(SearchGenreDto input, PaginatorInputDto pagination);

        GenreEntity FindGenreByName(string genre);

        List<string> FindGenresByBookId(Guid bookId);

        Task<List<Guid>> FindMultipleGenresByNameAsync(string name);

        Task<int> GetBooksNumberForGenreAsync(Guid genreId);

        void CheckGenreErrorByTotalCount(int count);

        void CheckNullGenreByEntity(GenreEntity? entity);

        void CheckIfGenreExistsOnAdd(Genre input);

        Task CheckIfGenreExistsOnUpdate(Guid genreId, Genre input);

        Task CheckGenreBooksBeforeDelete(Guid genreId);
    }
}
