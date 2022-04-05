using Microsoft.EntityFrameworkCore;
using Common.Models.InputDTOs;
using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;

namespace Repositories
{
    public class BookReservationRepository : GenericRepository<BookReservationEntity>, IBookReservationRepository
    {
        public BookReservationRepository(LibraryDbContext context)
           : base(context)
        {
            this.context = context;
        }

        public async Task<(List<BookReservationEntity>, int)> GetBookReservationPageAsync(PaginatorInputDto input)
        {
            var bookReservations = await context.BookReservations
                .Where(r => r.IsReviewed == false)
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();

            var totalCount = await context.BookReservations
                .Where(r => r.IsReviewed == false)
                .CountAsync();

            return (bookReservations, totalCount);
        }
    }
}
