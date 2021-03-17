using System;
using System.Threading.Tasks;

namespace VUModManagerRegistry.Interfaces
{
    public interface IAccessTokenService
    {
        public Task<Guid> Create();
        public Task<bool> Verify(Guid accessToken);
    }
}