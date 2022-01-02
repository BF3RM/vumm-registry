using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories
{
    public abstract class RepositoryBase<TEntity, TContext> : IRepository<TEntity>
        where TEntity: class, IEntity
        where TContext: DbContext
    {
        protected readonly TContext Context;
        protected readonly DbSet<TEntity> Set;

        protected RepositoryBase(TContext context)
        {
            Context = context;
            Set = Context.Set<TEntity>();
        }
        
        public async Task<List<TEntity>> FindAllAsync()
        {
            return await Set.ToListAsync();
        }

        public async Task<TEntity> FindByIdAsync(long id)
        {
            return await Set.FindAsync(id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await Set.AddAsync(entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            var entity = await Set.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            Set.Remove(entity);
            await Context.SaveChangesAsync();

            return true;
        }
    }
}