using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using VUModManagerRegistry.Authentication;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;

namespace VUModManagerRegistry.Services
{
    public class ModAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Mod>
    {
        private readonly IModRepository _modRepository;

        public ModAuthorizationHandler(IModRepository modRepository)
        {
            _modRepository = modRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            Mod resource)
        {
            if (requirement.Name == ModOperations.Read.Name)
            {
                if (!resource.IsPrivate)
                {
                    context.Succeed(requirement);
                }
                else if (await HasPrincipalPermissions(context.User, resource.Id, 
                    ModPermission.Readonly, ModPermission.Publish))
                {
                    context.Succeed(requirement);
                }

                return;
            }

            // Publish permission, check if user has that permission
            if (requirement.Name == ModOperations.Publish.Name)
            {
                if (await HasPrincipalPermissions(context.User, resource.Id, 
                    ModPermission.Readonly))
                {
                    context.Succeed(requirement);
                }
            }
        }

        private async Task<bool> HasPrincipalPermissions(ClaimsPrincipal principal, long modId,
            params ModPermission[] permissions)
        {
            if (principal?.Identity?.IsAuthenticated == false)
            {
                return false;
            }

            var userId = ((UserIdentity) principal.Identity).Id;

            return await HasPermissions(modId, userId, permissions);
        }

        private async Task<bool> HasPermissions(long modId, long userId, params ModPermission[] permissions)
        {
            var userPermissions = await _modRepository.FindPermissionsByIdAndUserId(modId, userId);

            if (userPermissions == null)
            {
                return false;
            }

            return userPermissions
                .Select(p => p.Permission)
                .Intersect(permissions)
                .Any();
        }
    }

    public static class ModOperations
    {
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement() {Name = nameof(Read)};

        public static OperationAuthorizationRequirement Publish =
            new OperationAuthorizationRequirement() {Name = nameof(Publish)};
    }
}