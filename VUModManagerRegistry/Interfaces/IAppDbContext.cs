using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Interfaces
{
    public interface IAppDbContext : IDisposable
    {
        public DbSet<User> Users { get; }
        public DbSet<UserAccessToken> AccessTokens { get; }
        
        public DbSet<Mod> Mods { get; }
        public DbSet<ModVersion> ModVersions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}