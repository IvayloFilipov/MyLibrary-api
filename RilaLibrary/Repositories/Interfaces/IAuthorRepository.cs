using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public interface IAuthorRepository : IGenericRepository<AuthorEntity>
    {
        AuthorEntity FindAuthorByName(string author);

        List<string> FindAuthorsByBookId(Guid bookId);

        Task<List<AuthorOutput>> GetAllAuthorsAsync();

        Task<bool> ContainsAuthor(Guid id, string name);

        Task<List<Guid>> FindMultipleAuthorsByNameAsync(string name);

        Task<(List<AuthorOutput>, int)> SearchForAuthorsAsync(SearchAuthorDto input, PaginatorInputDto pagination);
    }
}
