using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class ModAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Mod>
    {
        private readonly IModAuthorizationService _modAuthorizationService;

        public ModAuthorizationHandler(IModAuthorizationService modAuthorizationService)
        {
            _modAuthorizationService = modAuthorizationService;
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
                else if (await _modAuthorizationService.HasAnyPermissions(resource.Id, context.User.Id(),
                    ModPermission.Read, ModPermission.Write))
                {
                    context.Succeed(requirement);
                }

                return;
            }

            // Publish permission, check if user has that permission
            if (requirement.Name == ModOperations.Publish.Name)
            {
                if (await _modAuthorizationService.HasAnyPermissions(resource.Id, context.User.Id(),
                    ModPermission.Write))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    public static class ModOperations
    {
        public static OperationAuthorizationRequirement Read =
            new() {Name = nameof(Read)};

        public static OperationAuthorizationRequirement Publish =
            new() {Name = nameof(Publish)};
    }
}