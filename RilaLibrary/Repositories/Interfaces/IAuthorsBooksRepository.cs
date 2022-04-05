using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public interface IAuthorsBooksRepository : IGenericRepository<AuthorsBooks>
    {
        void DeleteAuthorEntriesForBook(Guid bookId);

        Task<int> GetBooksNumberForAuthorAsync(Guid authorId);
    }
}
