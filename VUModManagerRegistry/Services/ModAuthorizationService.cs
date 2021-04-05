using System.Linq;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Services
{
    public class ModAuthorizationService : IModAuthorizationService
    {
        private readonly IModUserPermissionRepository _repository;

        public ModAuthorizationService(IModUserPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> HasAnyPermissions(long modId, long userId, params ModPermission[] permissions)
        {
            var userPermission = await _repository.FindByModAndUserIdAsync(modId, userId);
            return userPermission != null && permissions.Contains(userPermission.Permission);
        }

        public async Task<bool> SetPermission(long modId, long userId, ModPermission permission)
        {
            var userPermission = await _repository.FindByModAndUserIdAsync(modId, userId);
            if (userPermission == null)
            {
                userPermission = new ModUserPermission()
                {
                    ModId = modId,
                    UserId = userId,
                    Permission = permission
                };
                await _repository.AddAsync(userPermission);
                return true;
            }

            if (userPermission.Permission != permission)
            {
                userPermission.Permission = permission;
                await _repository.UpdateAsync(userPermission);
                return true;
            }

            return false;
        }

        public Task<bool> RevokePermissions(long modId, long userId)
        {
            return _repository.DeleteByModAndUserIdAsync(modId, userId);
        }
    }
}