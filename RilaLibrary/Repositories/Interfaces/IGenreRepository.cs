using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public interface IGenreRepository : IGenericRepository<GenreEntity>
    {
        bool ContainsGenreName(string genreName);

        Task<bool> ContainsGenreName(Guid id, string genreName);

        Task<List<GenreOutput>> GetAllGenresAsync();

        GenreEntity FindGenreByName(string genre);

        List<string> FindGenresByBookId(Guid bookId);

        Task<int> GetCountOfAllGenresAsync();

        Task<List<Guid>> FindMultipleGenresByNameAsync(string name);

        Task<(List<GenreOutput>, int)> SearchForGenresAsync(SearchGenreDto input, PaginatorInputDto pagination);
    }
}
