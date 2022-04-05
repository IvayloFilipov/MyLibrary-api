using DataAccess.Entities;

namespace Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserEntity>
    {
        Task<int> GetCountOfAllReadersAsync();
    }
}
