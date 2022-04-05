using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public static class InMemoryDbContext
    {
        public static LibraryDbContext GetInMemoryDbContext(string databaseName)
        {
            return new LibraryDbContext(
            new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options);
        }
    }
}
