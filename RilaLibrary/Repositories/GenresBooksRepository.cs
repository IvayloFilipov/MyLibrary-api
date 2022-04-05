using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;

namespace Repositories
{
    public class GenresBooksRepository : GenericRepository<GenresBooks>, IGenresBooksRepository
    {
        public GenresBooksRepository(LibraryDbContext context)
           : base(context)
        {
        }

        public void DeleteGenreEntriesForBook(Guid bookId)
        {
            var entries = context.GenresBooks
                .Where(x => x.BookEntityId == bookId)
                .ToList();

            if (entries.Count > 0)
            {
                context.GenresBooks.RemoveRange(entries);
            }
        }

        public async Task<int> GetBooksNumberForGenreAsync(Guid genreId)
        {
            var result = await context.GenresBooks
                .Where(x => x.GenreEntityId == genreId)
                .CountAsync();

            return result;
        }
    }
}
