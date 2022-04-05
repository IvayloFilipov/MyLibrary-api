using log4net;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
using Repositories.Interfaces;
using Repositories.Mappers;
using Services.Interfaces;
using AutoMapper;

using static Common.ExceptionMessages;

namespace Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository bookRepository;
        private readonly IAuthorService authorService;
        private readonly IGenreService genreService;
        private readonly IAuthorsBooksService authorsBooksService;
        private readonly IGenresBooksService genresBooksService;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;
        private readonly ILog log = LogManager.GetLogger(typeof(BookService));

        public BookService(IBookRepository bookRepository,
            IAuthorService authorService,
            IGenreService genreService,
            IAuthorsBooksService authorsBooksService,
            IGenresBooksService genresBooksService,
            IBlobService blobService,
            IMapper mapper
            )
        {
            this.bookRepository = bookRepository;
            this.authorService = authorService;
            this.genreService = genreService;
            this.authorsBooksService = authorsBooksService;
            this.genresBooksService = genresBooksService;
            this.blobService = blobService;
            this.mapper = mapper;
        }

        public async Task<BookOutput> AddBookAsync(AddBookDto book)
        {
            book = TrimBookTitleAndDescription(book);
            book.Availability = true;

            CheckForTitleOnAddBook(book);
            CheckAuthorsAndGenres(book);
            CheckBookQuantityOnAddBook(book);

            var authorsEntities = GenerateAuthorEntities(book);
            var genresEntities = GenerateGenreEntities(book);

            var url = await GetBookCoverUrlOnAddBookAsync(book);

            //var bookEntity = Mapper.ToBookEntity(book, authorsEntities, genresEntities, url); //was
            var bookEntity = mapper.Map<BookEntity>(book); //new

            var mapAutors = mapper.Map<List<AuthorEntity>, ICollection<AuthorsBooks>>(authorsEntities); //new
            var mapAGenres = mapper.Map<List<GenreEntity>, ICollection<GenresBooks>>(genresEntities); //new

            bookEntity.ImageAddress = url; //new
            bookEntity.Sku = Guid.NewGuid(); //new

            bookEntity.AuthorsBooks = mapAutors; //new
            bookEntity.GenresBooks = mapAGenres; //new

            await bookRepository.InsertAsync(bookEntity);
            await bookRepository.SaveAsync();

            var result = GetAddOrUpdateBookResult(bookEntity);

            return result;
        }

        public async Task<BookOutput> UpdateBookAsync(Guid bookId, AddBookDto book)
        {
            var existingEntity = await GetByBookIdRepoAsync(bookId);
            CheckNullBookByEntity(existingEntity);

            book = TrimBookTitleAndDescription(book);

            await CheckIfBookExistsOnUpdateAsync(bookId, book);
            CheckAuthorsAndGenres(book);

            authorsBooksService.DeleteAuthorEntriesForBook(bookId);
            genresBooksService.DeleteGenreEntriesForBook(bookId);

            var authorsEntities = GenerateAuthorEntities(book);
            var genresEntities = GenerateGenreEntities(book);

            var url = await GetBookCoverUrlOnUpdateBookAsync(book, existingEntity!);

            //var inputBookEntity = Mapper.ToBookEntity(book, authorsEntities, genresEntities, url); //was
            var inputBookEntity = mapper.Map<BookEntity>(book); //new

            var mapAutors = mapper.Map<List<AuthorEntity>, ICollection<AuthorsBooks>>(authorsEntities); //new
            var mapAGenres = mapper.Map<List<GenreEntity>, ICollection<GenresBooks>>(genresEntities); //new

            inputBookEntity.ImageAddress = url; //new
            inputBookEntity.Sku = Guid.NewGuid(); //new

            inputBookEntity.AuthorsBooks = mapAutors; //new
            inputBookEntity.GenresBooks = mapAGenres; //new

            existingEntity = UpdateExistingBookProperties(existingEntity!, inputBookEntity);

            await UpdateRepoAsync(existingEntity);
            await bookRepository.SaveAsync();

            var result = GetAddOrUpdateBookResult(existingEntity);

            return result;
        }

        public async Task DeleteBookAsync(Guid bookId)
        {
            var existingEntity = await GetByBookIdRepoAsync(bookId);

            CheckNullBookByEntity(existingEntity);
            await CompareBookQuantityOnDeleting(bookId);

            if (existingEntity!.ImageAddress != null)
            {
                await blobService.RemoveBlobFileAsync(existingEntity.ImageAddress!);
            }

            await bookRepository.DeleteAsync(bookId);
            await bookRepository.SaveAsync();
        }

        public async Task<(List<BookOutput>, int)> GetBooksAsync(PaginatorInputDto pagination)
        {
            var (entities, totalCount) = await bookRepository.GetEntityPageAsync(pagination);

            CheckBooksByCount(totalCount);

            var result = new List<BookOutput>();

            //entities.ForEach(entity => result.Add(Mapper.ToBookOutput(entity))); //was
            entities.ForEach(entity => result.Add(mapper.Map<BookOutput>(entity)));

            return (result, totalCount);
        }

        public async Task<List<BookOutput>> GetAllBooksAsync()
        {
            var outputEntities = await bookRepository.GetAllBooksAsync();

            CheckBooksByCount(outputEntities.Count);

            var result = AddAuthorsAndGenresToListOutput(outputEntities);

            return result;
        }

        public async Task<BookOutput> GetBookByIdAsync(Guid bookId)
        {
            var existingEntity = await GetByBookIdRepoAsync(bookId);

            CheckNullBookByEntity(existingEntity);

            //var outputEntity = Mapper.ToBookOutput(existingEntity!); //was
            var outputEntity = mapper.Map<BookOutput>(existingEntity!);
            var result = AddAuthorsAndGenresToOutput(outputEntity);

            return result;
        }

        public async Task<int> GetCountOfAllBooksAsync()
        {
            var allBooks = await bookRepository.GetCountOfAllBooksAsync();

            return allBooks;
        }

        public async Task<(List<LastBooksOutput>, int)> GetBooksForLastTwoWeeksAsync(PaginatorInputDto pagination)
        {
            var (lastBooksOutput, lastBooksCount) = await bookRepository.GetBooksForLastTwoWeeksAsync(pagination);

            foreach (var book in lastBooksOutput)
            {
                var authors = authorService.FindAuthorsByBookId(book.Id);
                book.AllAuthors = string.Join(", ", authors);
            }

            return (lastBooksOutput, lastBooksCount);
        }

        public async Task<(List<BookOutput>, int)> SearchForBooksAsync(SearchBookDto input, PaginatorInputDto pagination)
        {
            var authors = await GetAuthorGuidsForSearchAsync(input);
            var genres = await GetGenresGuidsForSearchAsync(input);

            var (outputEntities, totalCount) = await bookRepository.SearchForBooksAsync(input, authors, genres, pagination);

            CheckBooksByCount(totalCount);

            var result = AddAuthorsAndGenresToListOutput(outputEntities);

            return (result, totalCount);
        }

        public void CheckNullBookByEntity(BookEntity? entity)
        {
            if (entity == null)
            {
                log.Error($"Book is null. Exception is thrown {BOOK_NOT_FOUND}");
                throw new NullReferenceException(BOOK_NOT_FOUND);
            }
        }

        public void CheckAuthorsAndGenres(AddBookDto book)
        {
            if (book.BookAuthors == null || book.BookAuthors.Count == 0)
            {
                log.Error($"Exception is thrown {NO_AUTHORS_FOUND}");
                throw new ArgumentException(NO_AUTHORS_FOUND);
            }

            if (book.Genres == null || book.Genres.Count == 0)
            {
                log.Error($"Exception is thrown {NO_GENRES_FOUND}");
                throw new ArgumentException(NO_GENRES_FOUND);
            }
        }

        public void CheckForTitleOnAddBook(AddBookDto book)
        {
            var existBook = bookRepository.FindBookByTitle(book);

            if (existBook != null)
            {
                log.Error($"Exception is thrown {BOOKTITLE_FOUND}");
                throw new ArgumentException(BOOKTITLE_FOUND);
            }
        }

        public List<AuthorEntity> GenerateAuthorEntities(AddBookDto book)
        {
            var authorsEntities = new List<AuthorEntity>();

            foreach (var author in book.BookAuthors!)
            {
                var existAuthor = authorService.FindAuthorByName(author);
                authorsEntities.Add(existAuthor);
            }

            return authorsEntities;
        }

        public List<GenreEntity> GenerateGenreEntities(AddBookDto book)
        {
            var genresEntities = new List<GenreEntity>();

            foreach (var genre in book.Genres!)
            {
                var existGenre = genreService.FindGenreByName(genre);
                genresEntities.Add(existGenre);
            }

            return genresEntities;
        }

        public void CheckBookQuantityOnAddBook(AddBookDto book)
        {
            if (book.TotalQuantity <= 0)
            {
                log.Error($"Book quantity is 0. Exception is thrown {BOOK_QUANTITY_IS_INVALID}");
                throw new ArgumentException(BOOK_QUANTITY_IS_INVALID);
            }
        }

        public async Task<string?> GetBookCoverUrlOnAddBookAsync(AddBookDto book)
        {
            string? url = default(string);

            if (book.BookCover?.Length > 0)
            {
                url = await blobService.UploadBlobFileAsync(book.BookCover!, book.BookTitle);
            }

            return url;
        }

        public async Task CheckIfBookExistsOnUpdateAsync(Guid bookId, AddBookDto book)
        {
            var containsBookName = await bookRepository.ContainsBookName(bookId, book.BookTitle);

            if (containsBookName)
            {
                log.Error($"Exception is thrown {BOOK_EXISTS}");
                throw new ArgumentException(BOOK_EXISTS);
            }
        }

        public async Task<string?> GetBookCoverUrlOnUpdateBookAsync(AddBookDto book, BookEntity existingEntity)
        {
            string? url = default(string);

            if (book.DeleteCover == true && existingEntity!.ImageAddress != null)
            {
                await blobService.RemoveBlobFileAsync(existingEntity.ImageAddress!);
            }
            else if (book.BookCover?.Length > 0 && existingEntity!.ImageAddress != null)
            {
                url = await blobService.UpdateBlobFileAsync(book.BookCover!, existingEntity.ImageAddress!, book.BookTitle);
            }
            else if (book.BookCover?.Length > 0 && existingEntity!.ImageAddress == null)
            {
                url = await blobService.UploadBlobFileAsync(book.BookCover!, book.BookTitle);
            }
            else if ((book.BookTitle != existingEntity!.Title) && existingEntity.ImageAddress?.Length > 0)
            {
                url = await blobService.RenameBlobFileAsync(existingEntity.ImageAddress!, book.BookTitle);
            }
            else
            {
                url = existingEntity.ImageAddress;
            }

            return url;
        }

        public BookEntity UpdateExistingBookQuantity(BookEntity existingEntity, BookEntity inputBookEntity)
        {
            var diffTotalQuantityAfterUpdate = inputBookEntity.TotalQuantity - existingEntity.TotalQuantity;
            var numberBorrowedBooks = existingEntity.TotalQuantity - existingEntity.CurrentQuantity;

            if (inputBookEntity.TotalQuantity < 0)
            {
                log.Error($"Exception is thrown {BOOK_QUANTITY_IS_LESS_THAN_ZERO}");
                throw new ArgumentException(BOOK_QUANTITY_IS_LESS_THAN_ZERO);
            }
            else if (inputBookEntity.TotalQuantity < numberBorrowedBooks)
            {
                log.Error($"Exception is thrown {string.Format(BOOK_QUANTITY_IS_LESS_THAN_BORROWEDBOOKS, numberBorrowedBooks)}");
                throw new ArgumentException(string.Format(BOOK_QUANTITY_IS_LESS_THAN_BORROWEDBOOKS, numberBorrowedBooks));
            }
            else
            {
                existingEntity.TotalQuantity += diffTotalQuantityAfterUpdate;
                existingEntity.CurrentQuantity += diffTotalQuantityAfterUpdate;
            }

            return existingEntity;
        }

        public BookEntity UpdateExistingBookProperties(BookEntity existingEntity, BookEntity inputBookEntity)
        {
            UpdateExistingBookQuantity(existingEntity, inputBookEntity);

            existingEntity.Title = inputBookEntity.Title;
            existingEntity.Description = inputBookEntity.Description;
            existingEntity.AuthorsBooks = inputBookEntity.AuthorsBooks;
            existingEntity.GenresBooks = inputBookEntity.GenresBooks;
            existingEntity.IsAvailable = inputBookEntity.IsAvailable;
            existingEntity.ImageAddress = inputBookEntity.ImageAddress;

            return existingEntity;
        }

        public BookOutput AddAuthorsAndGenresToOutput(BookOutput outputEntity)
        {
            var authors = authorService.FindAuthorsByBookId(outputEntity.Id);
            outputEntity.AllAuthors = string.Join(", ", authors);

            var genres = genreService.FindGenresByBookId(outputEntity.Id);
            outputEntity.AllGenres = string.Join(", ", genres);

            return outputEntity;
        }

        public List<BookOutput> AddAuthorsAndGenresToListOutput(List<BookOutput> outputEntities)
        {
            for (var i = 0; i < outputEntities.Count; i++)
            {
                outputEntities[i] = AddAuthorsAndGenresToOutput(outputEntities[i]);
            }

            return outputEntities;
        }

        public AddBookDto TrimBookTitleAndDescription(AddBookDto book)
        {
            book.BookTitle = book.BookTitle.Trim();
            book.Description = book.Description?.Trim();

            return book;
        }

        public void CheckBooksByCount(int bookCount)
        {
            if (bookCount == 0)
            {
                log.Error($"Exception is thrown {NO_BOOKS_FOUND}");
                throw new NullReferenceException(NO_BOOKS_FOUND);
            }
        }

        public async Task<List<Guid>> GetAuthorGuidsForSearchAsync(SearchBookDto input)
        {
            var authors = new List<Guid>();

            if (input.Author != null)
            {
                var authorsResult = await authorService.FindMultipleAuthorsByNameAsync(input.Author);
                authors.AddRange(authorsResult);
            }

            return authors;
        }

        public async Task<List<Guid>> GetGenresGuidsForSearchAsync(SearchBookDto input)
        {
            var genres = new List<Guid>();

            if (input.Genre != null)
            {
                var genresResult = await genreService.FindMultipleGenresByNameAsync(input.Genre);
                genres.AddRange(genresResult);
            }

            return genres;
        }

        public BookOutput GetAddOrUpdateBookResult(BookEntity bookEntity)
        {
            //var outputEntity = Mapper.ToBookOutput(bookEntity); //was
            var outputEntity = mapper.Map<BookOutput>(bookEntity);
            var result = AddAuthorsAndGenresToOutput(outputEntity);

            return result;
        }

        public async Task<BookEntity?> GetByBookIdRepoAsync(Guid bookId)
        {
            var result = await bookRepository.GetByIdAsync(bookId);
            return result;
        }

        public async Task<BookEntity> UpdateRepoAsync(BookEntity bookEntity)
        {
            var result = await bookRepository.UpdateAsync(bookEntity);
            return result;
        }

        public async Task<bool> CompareBookQuantity(Guid bookId)
        {
            var bookEntity = await GetByBookIdRepoAsync(bookId);

            var result = bookEntity!.CurrentQuantity == bookEntity.TotalQuantity;

            return result;
        }

        public async Task CompareBookQuantityOnDeleting(Guid bookId)
        {
            var quantityMatch = await CompareBookQuantity(bookId);

            if (!quantityMatch)
            {
                log.Error($"Book quantity does not match. Exception is thrown {BOOK_HAS_APPROVED_RESERVATIONS}");
                throw new ArgumentException(BOOK_HAS_APPROVED_RESERVATIONS);
            }
        }
    }
}
