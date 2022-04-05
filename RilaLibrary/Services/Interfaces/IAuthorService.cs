using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Services.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorOutput> AddAuthorAsync(AuthorDto input);

        Task<List<AuthorOutput>> GetAllAuthorsAsync();

        Task<(List<AuthorOutput>, int)> GetAuthorsAsync(PaginatorInputDto input);

        Task<AuthorOutput> GetAuthorByIdAsync(Guid authorId);

        Task<AuthorOutput> UpdateAuthorAsync(AuthorDto input, Guid authorId);

        Task<(List<AuthorOutput>, int)> SearchForAuthorsAsync(SearchAuthorDto input, PaginatorInputDto pagination);

        Task DeleteAuthorAsync(Guid authorId);

        AuthorEntity FindAuthorByName(string author);

        List<string> FindAuthorsByBookId(Guid bookId);

        Task<List<Guid>> FindMultipleAuthorsByNameAsync(string name);

        Task<int> GetBooksNumberForAuthorAsync(Guid authorId);

        void CheckAuthorsErrorByTotalCount(int count);

        void CheckNullAuthorByEntity(AuthorEntity? entity);

        AuthorDto CheckAuthorNameOnAdd(AuthorDto input);

        Task CheckAuthorNameOnUpdateAsync(AuthorDto input, Guid authorId);

        Task CheckAuthorBooksBeforeDeletingAsync(Guid authorId);
    }
}
