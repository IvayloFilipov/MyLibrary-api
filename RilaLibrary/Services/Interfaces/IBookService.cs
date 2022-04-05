using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;

namespace Services.Interfaces
{
    public interface IBookService
    {
        Task<BookOutput> GetBookByIdAsync(Guid bookId);

        Task<BookOutput> AddBookAsync(AddBookDto book);

        Task<BookOutput> UpdateBookAsync(Guid bookId, AddBookDto book);

        Task DeleteBookAsync(Guid bookId);

        Task<(List<BookOutput>, int)> GetBooksAsync(PaginatorInputDto pagination);

        Task<List<BookOutput>> GetAllBooksAsync();

        Task<int> GetCountOfAllBooksAsync();

        Task<(List<LastBooksOutput>, int)> GetBooksForLastTwoWeeksAsync(PaginatorInputDto pagination);

        Task<(List<BookOutput>, int)> SearchForBooksAsync(SearchBookDto input, PaginatorInputDto pagination);

        void CheckNullBookByEntity(BookEntity? entity);

        void CheckAuthorsAndGenres(AddBookDto book);

        void CheckForTitleOnAddBook(AddBookDto book);

        List<AuthorEntity> GenerateAuthorEntities(AddBookDto book);

        List<GenreEntity> GenerateGenreEntities(AddBookDto book);

        void CheckBookQuantityOnAddBook(AddBookDto book);

        Task<string?> GetBookCoverUrlOnAddBookAsync(AddBookDto book);

        Task CheckIfBookExistsOnUpdateAsync(Guid bookId, AddBookDto book);

        Task<string?> GetBookCoverUrlOnUpdateBookAsync(AddBookDto book, BookEntity existingEntity);

        BookEntity UpdateExistingBookQuantity(BookEntity existingEntity, BookEntity inputBookEntity);

        BookEntity UpdateExistingBookProperties(BookEntity existingEntity, BookEntity inputBookEntity);

        BookOutput AddAuthorsAndGenresToOutput(BookOutput outputEntity);

        List<BookOutput> AddAuthorsAndGenresToListOutput(List<BookOutput> outputEntities);

        AddBookDto TrimBookTitleAndDescription(AddBookDto book);

        void CheckBooksByCount(int bookCount);

        Task<List<Guid>> GetAuthorGuidsForSearchAsync(SearchBookDto input);

        Task<List<Guid>> GetGenresGuidsForSearchAsync(SearchBookDto input);

        BookOutput GetAddOrUpdateBookResult(BookEntity bookEntity);

        Task<BookEntity?> GetByBookIdRepoAsync(Guid bookId);

        Task<BookEntity> UpdateRepoAsync(BookEntity bookEntity);

        Task<bool> CompareBookQuantity(Guid bookId);

        Task CompareBookQuantityOnDeleting(Guid bookId);
    }
}