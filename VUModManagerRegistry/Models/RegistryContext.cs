using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Helpers;

namespace VUModManagerRegistry.Models
{
    public class RegistryContext : DbContext
    {
        public DbSet<Mod> Mods { get; set; }
        public DbSet<ModVersion> ModVersions { get; set; }
        
        public DbSet<ModTag> ModTags { get; set; }
        
        public RegistryContext(DbContextOptions<RegistryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Mod>(entity =>
            {
                // Has many versions
                entity
                    .HasMany(m => m.Versions)
                    .WithOne()
                    .HasForeignKey(v => v.ModId);

                // Has many tags
                entity
                    .HasMany(m => m.Tags)
                    .WithOne()
                    .HasForeignKey(t => t.ModId);
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

                entity
                    .HasIndex(v => new {v.Name, v.Version})
                    .IsUnique();
            });

            modelBuilder.Entity<ModTag>()
                .HasIndex(t => new {t.ModId, t.Name, t.Version})
                .IsUnique();
        }
    }
}