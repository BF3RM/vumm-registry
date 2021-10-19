using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Helpers;

namespace VUModManagerRegistry.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Mod> Mods { get; set; }
        public DbSet<ModVersion> ModVersions { get; set; }
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccessToken> AccessTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
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
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, null),
                        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, null)
                    )
                    .Metadata.SetValueComparer(DictionaryHelpers.StringValueComparer);

                entity.HasOne(mp => mp.Mod)
                    .WithMany(m => m.Versions)
                    .HasForeignKey(mp => mp.ModId);
                
                entity
                    .HasIndex(v => new {v.Name, v.Version})
                    .IsUnique();
            });
        }
    }
}