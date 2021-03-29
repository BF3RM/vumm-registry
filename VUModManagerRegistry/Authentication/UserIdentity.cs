using System.Security.Principal;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Authentication
{
    public class UserIdentity : IIdentity
    {
        public long Id => User.Id;
        public string Name => User.Username;
        public string AuthenticationType => AccessTokenDefaults.AuthenticationScheme;
        public bool IsAuthenticated => true;
        public User User { get; }

        public UserIdentity(User user)
        {
            User = user;
        }
    }
}