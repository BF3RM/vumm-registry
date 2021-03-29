using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Interfaces
{
    public interface IUserService
    {
        public Task<User> FindByName(long id);
    }
}