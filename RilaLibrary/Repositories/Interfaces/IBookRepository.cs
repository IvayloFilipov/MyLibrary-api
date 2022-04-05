using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public  interface IBookRepository : IGenericRepository<BookEntity>
    {
        BookEntity FindBookByTitle(AddBookDto book);

        Task<List<BookOutput>> GetAllBooksAsync();

        Task<bool> ContainsBookName(Guid id, string bookName);

        Task<int> GetCountOfAllBooksAsync();

        Task<(List<LastBooksOutput>, int)> GetBooksForLastTwoWeeksAsync(PaginatorInputDto pagination);

        Task<(List<BookOutput>, int)> SearchForBooksAsync(SearchBookDto input, List<Guid> authors, List<Guid> genres, PaginatorInputDto pagination);
    }
}
