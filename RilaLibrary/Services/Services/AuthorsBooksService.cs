using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services
{
    public class AuthorsBooksService : IAuthorsBooksService
    {
        private readonly IAuthorsBooksRepository authorsBooksRepository;

        public AuthorsBooksService(IAuthorsBooksRepository authorsBooksRepository)
        {
            this.authorsBooksRepository = authorsBooksRepository;
        }

        public void DeleteAuthorEntriesForBook(Guid bookId)
        {
            authorsBooksRepository.DeleteAuthorEntriesForBook(bookId);
        }

        public async Task<int> GetBooksNumberForAuthorAsync(Guid authorId)
        {
            var result = await authorsBooksRepository.GetBooksNumberForAuthorAsync(authorId);

            return result;
        }
    }
}
