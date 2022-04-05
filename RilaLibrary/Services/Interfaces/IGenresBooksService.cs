namespace Services.Interfaces
{
    public interface IGenresBooksService
    {
        void DeleteGenreEntriesForBook(Guid bookId);

        Task<int> GetBooksNumberForGenreAsync(Guid genreId);
    }
}
