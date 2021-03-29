using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Authentication;

namespace VUModManagerRegistry.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected UserIdentity AuthenticatedUser => User.Identity as UserIdentity;
    }
}