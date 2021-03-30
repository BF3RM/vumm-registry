using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories
{
    public class UserRepository : RepositoryBase<User, AppDbContext>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User> FindByIdWithPermissionsAsync(long id)
        {
            return await Set
                .Include(u => u.ModPermissions)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            return await Set.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await Set.AnyAsync(u => u.Username == username);
        }
    }
}