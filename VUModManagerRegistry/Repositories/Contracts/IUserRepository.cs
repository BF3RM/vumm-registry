using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindByUsernameAsync(string username);

        Task<bool> ExistsByUsernameAsync(string username);
    }
}