using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using VUModManagerRegistry.Authentication;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Services
{
    public class ModAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Mod>
    {
        private readonly IModService _modService;

        public ModAuthorizationHandler(IModService modService)
        {
            _modService = modService;
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
                else if (await HasPrincipalPermissions(context.User.AuthenticatedUser(), resource.Id, 
                    ModPermission.Readonly, ModPermission.Publish))
                {
                    context.Succeed(requirement);
                }

                return;
            }

            // Publish permission, check if user has that permission
            if (requirement.Name == ModOperations.Publish.Name)
            {
                if (await HasPrincipalPermissions(context.User.AuthenticatedUser(), resource.Id, 
                    ModPermission.Readonly))
                {
                    context.Succeed(requirement);
                }
            }
        }

        private async Task<bool> HasPrincipalPermissions(UserIdentity identity, long modId,
            params ModPermission[] permissions)
        {
            if (identity == null)
            {
                return false;
            }

            return await _modService.HasAnyPermissions(modId, identity.Id, permissions);
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