using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Helpers;

namespace VUModManagerRegistry.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Mod> Mods { get; set; }
        public DbSet<ModVersion> ModVersions { get; set; }
        
        public DbSet<ModUserPermission> ModUserPermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccessToken> AccessTokens { get; set; }

        private readonly ISystemTimeProvider _systemTimeProvider;
        public AppDbContext(ISystemTimeProvider systemTimeProvider, DbContextOptions<AppDbContext> options)
            : base(options)
        {
            _systemTimeProvider = systemTimeProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AccessTokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<UserAccessToken>()
                .HasIndex(a => a.Token);

            modelBuilder.Entity<Mod>(entity =>
            {
                // Has many versions
                entity
                    .HasMany(m => m.Versions)
                    .WithOne()
                    .HasForeignKey(v => v.ModId);

                // Many mods have many users, joined by ModUserPermission
                entity
                    .HasMany(m => m.Users)
                    .WithMany(u => u.Mods)
                    .UsingEntity<ModUserPermission>(
                        j => j
                            .HasOne(mp => mp.User)
                            .WithMany(u => u.ModPermissions)
                            .HasForeignKey(mp => mp.UserId),
                        j => j
                            .HasOne(mp => mp.Mod)
                            .WithMany(m => m.UserPermissions)
                            .HasForeignKey(mp => mp.ModId),
                        j =>
                            j.HasKey(mp => new {mp.ModId, mp.UserId, mp.Tag})
                    );
            });

            modelBuilder.Entity<ModVersion>(entity =>
            {
                entity
                    .Property(v => v.Dependencies)
                    .HasColumnType("jsonb");

                entity.HasOne(mp => mp.Mod)
                    .WithMany(m => m.Versions)
                    .HasForeignKey(mp => mp.ModId);
                
                entity
                    .HasIndex(v => new {v.Name, v.Version})
                    .IsUnique();
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var now = _systemTimeProvider.Now;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IEntity entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.Created = now;
                            entity.LastUpdated = now;
                            break;
                        case EntityState.Modified:
                            Entry(entity).Property(x => x.Created).IsModified = false;
                            entity.LastUpdated = now;
                            break;
                    }
                }
            }
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}