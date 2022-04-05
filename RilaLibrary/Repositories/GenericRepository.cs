using Microsoft.EntityFrameworkCore;
using Common.Models.InputDTOs;
using DataAccess;
using Repositories.Interfaces;

namespace Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected LibraryDbContext context;
        private DbSet<T> entities;
        private bool disposed;

        public GenericRepository(LibraryDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            T? entity = await entities.FindAsync(id);

            return entity;
        }

        public async Task<T> InsertAsync(T obj)
        {
            await entities.AddAsync(obj);
            return obj;
        }

        public async Task<T> UpdateAsync(T obj)
        {
            entities.Attach(obj);
            context.Entry(obj).State=EntityState.Modified;
            return obj;
        }

        public async Task DeleteAsync(object id)
        {
            T? entity = await this.GetByIdAsync(id);

            if (entity != null)
            {
                entities.Remove(entity);
            }
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<(List<T>, int)> GetEntityPageAsync(PaginatorInputDto input)
        {
            var entitiesResult = await entities
                .Skip((input.Page - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();

            var totalCount = await entities.CountAsync();

            return (entitiesResult, totalCount);
        }

        public async ValueTask DisposeAsync()
        {
          await context.DisposeAsync();
          GC.SuppressFinalize(this);
        } 
    }
}
