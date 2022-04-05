using Common.Models.InputDTOs;
using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public interface IBookReservationRepository : IGenericRepository<BookReservationEntity>
    {
        Task<(List<BookReservationEntity>, int)> GetBookReservationPageAsync(PaginatorInputDto input);
    }
}
