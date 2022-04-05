using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Services
{
    public class GenresBooksService : IGenresBooksService
    {
        private readonly IGenresBooksRepository genresBooksRepository;

        public GenresBooksService(IGenresBooksRepository genresBooksRepository)
        {
            this.genresBooksRepository = genresBooksRepository;
        }

        public void DeleteGenreEntriesForBook(Guid bookId)
        {
            genresBooksRepository.DeleteGenreEntriesForBook(bookId);
        }

        public async Task<int> GetBooksNumberForGenreAsync(Guid genreId)
        {
            var result = await genresBooksRepository.GetBooksNumberForGenreAsync(genreId);
            return result;
        }
    }
}
