using DataAccess;

namespace Common.Models.Seeding
{
    public interface ISeeder
    {
        Task SeedAsync(LibraryDbContext dbContext, IServiceProvider serviceProvider);
    }
}
