using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public interface IGenresBooksRepository : IGenericRepository<GenresBooks>
    {
        void DeleteGenreEntriesForBook(Guid bookId);

        Task<int> GetBooksNumberForGenreAsync(Guid genreId);
    }
}
