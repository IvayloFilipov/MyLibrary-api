namespace Services.Interfaces
{
    public interface IAuthorsBooksService
    {
        void DeleteAuthorEntriesForBook(Guid bookId);

        Task<int> GetBooksNumberForAuthorAsync(Guid authorId);
    }
}
