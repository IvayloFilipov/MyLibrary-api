using Microsoft.EntityFrameworkCore;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;
using Repositories.Mappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Repositories
{
    public class BookRepository : GenericRepository<BookEntity>, IBookRepository
    {
        private readonly IMapper mapper;

        public BookRepository(LibraryDbContext context, IMapper mapper) 
            : base(context)
        {
            this.mapper = mapper;
        }

        public BookEntity FindBookByTitle(AddBookDto book)
        {
            var currBook = context.Books
                .Where(x => x.Title == book.BookTitle)
                .FirstOrDefault()!;

            return currBook;
        }

        public async Task<List<BookOutput>> GetAllBooksAsync()
        {
            var books = await context.Books
                //.Select(x => Mapper.ToBookOutput(x)) //was
                .ProjectTo<BookOutput>(mapper.ConfigurationProvider)
                .ToListAsync();

            return books;
        }

        public async Task<bool> ContainsBookName(Guid bookId, string bookTitle)
        {
            var result = await context.Books
                .FirstOrDefaultAsync(x => x.Title == bookTitle);

            if (result == null || result.Id == bookId)
            {
                return false;
            }

            return true;
        }

        public async Task<int> GetCountOfAllBooksAsync()
        {
            var totalCount = await context.Books
                .CountAsync();

            return totalCount;
        }

        public async Task<(List<LastBooksOutput>, int)> GetBooksForLastTwoWeeksAsync(PaginatorInputDto pagination)
        {
            DateTime today = DateTime.Today;
            DateTime fourteenDaysEarlier = today.AddDays(-14);

            var lastBooks = context.Books
                .Where(x => x.CreatedOn >= fourteenDaysEarlier)
                .OrderByDescending(x => x.CreatedOn)
                //.Select(x => Mapper.ToLastBooksOutput(x)); //was
                .ProjectTo<LastBooksOutput>(mapper.ConfigurationProvider);

            var result = await lastBooks
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var lastBooksCount = lastBooks.Count();

            return (result, lastBooksCount);
        }

        public async Task<(List<BookOutput>, int)> SearchForBooksAsync(SearchBookDto input, List<Guid> authors, List<Guid> genres, PaginatorInputDto pagination)
        {
            var query = context.Books
                .Where(x => input.Title == null || x.Title.Contains(input.Title))
                .Where(x => input.Description != null ? x.Description!.Contains(input.Description) : true)
                .Include(x => x.AuthorsBooks)
                .Where(x => input.Author != null ? x.AuthorsBooks.Any(y => authors.Contains(y.AuthorEntityId)) : true)
                .Include(x => x.GenresBooks)
                .Where(x => input.Genre != null ? x.GenresBooks.Any(y => genres.Contains(y.GenreEntityId)) : true);

            var result = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsSplitQuery()
                //.Select(x => Mapper.ToBookOutput(x)) //was
                .ProjectTo<BookOutput>(mapper.ConfigurationProvider)
                .ToListAsync();

            var totalCount = query.Count();

            return (result, totalCount);
        }
    }
}

