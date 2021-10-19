using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Services
{
    public class ModVersionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, ModVersion>
    {
        private readonly IModAuthorizationService _modAuthorizationService;

        public ModVersionAuthorizationHandler(IModAuthorizationService modAuthorizationService)
        {
            _modAuthorizationService = modAuthorizationService;
        }
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            ModVersion resource)
        {
            if (requirement.Name == ModOperations.Read.Name)
            {
                if (resource.Mod != null && !resource.Mod.IsPrivate)
                {
                    context.Succeed(requirement);
                }
                else if (await _modAuthorizationService.HasAnyPermissions(resource.ModId, context.User.Id(), resource.Tag,
                    ModPermission.Readonly, ModPermission.Publish))
                {
                    context.Succeed(requirement);
                }

                return;
            }

            // Publish permission, check if user has that permission
            if (requirement.Name == ModOperations.Publish.Name)
            {
                if (await _modAuthorizationService.HasAnyPermissions(resource.Id, context.User.Id(), resource.Tag,
                    ModPermission.Publish))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}