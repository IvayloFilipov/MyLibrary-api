using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;

namespace Repositories
{
    public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        public UserRepository(LibraryDbContext context) 
            : base(context)
        {
        }

        public async Task<int> GetCountOfAllReadersAsync()
        {
            var allReaders = await context.Users
                .CountAsync();

            return allReaders;
        }
    }
}
