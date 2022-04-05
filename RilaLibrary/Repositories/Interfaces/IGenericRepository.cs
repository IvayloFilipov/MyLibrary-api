using Common.Models.InputDTOs;

namespace Repositories.Interfaces
{
    public interface IGenericRepository<T> : IAsyncDisposable where T : class
    {
        Task<T?> GetByIdAsync(object id);

        Task<T> InsertAsync(T obj);

        Task<T> UpdateAsync(T obj);

        Task DeleteAsync(object id);

        Task SaveAsync();

        Task<(List<T>, int)> GetEntityPageAsync(PaginatorInputDto input);
    }
}
