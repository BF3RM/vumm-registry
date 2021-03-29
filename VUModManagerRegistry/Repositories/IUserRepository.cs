using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindByUsernameAsync(string username);

        Task<bool> ExistsByUsernameAsync(string username);
    }
}