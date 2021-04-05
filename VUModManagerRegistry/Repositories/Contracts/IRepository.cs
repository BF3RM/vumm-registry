using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> FindAllAsync();
        Task<T> FindByIdAsync(long id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteByIdAsync(long id);
    }
}