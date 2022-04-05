using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;

namespace Repositories
{
    public class AuthorsBooksRepository : GenericRepository<AuthorsBooks>, IAuthorsBooksRepository
    {
        public AuthorsBooksRepository(LibraryDbContext context)
           : base(context)
        {
        }

        public void DeleteAuthorEntriesForBook(Guid bookId)
        {
            var entries = context.AuthorsBooks
                .Where(x => x.BookEntityId == bookId)
                .ToList();

            if (entries.Count > 0)
            { 
                context.AuthorsBooks.RemoveRange(entries);
            }
        }

        public async Task<int> GetBooksNumberForAuthorAsync(Guid authorId)
        {
            var result = await context.AuthorsBooks
                .Where(x => x.AuthorEntityId == authorId)
                .CountAsync();

            return result;
        }
    }
}
