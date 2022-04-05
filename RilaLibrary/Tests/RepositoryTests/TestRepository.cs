using DataAccess;
using DataAccess.Entities;
using Repositories;

namespace Tests
{
    public class TestRepository : GenericRepository<UserEntity>
    {
        public TestRepository(LibraryDbContext context) 
            : base(context)
        {
        }
    }
}
