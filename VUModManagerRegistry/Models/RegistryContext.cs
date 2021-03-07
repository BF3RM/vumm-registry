using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace VUModManagerRegistry.Models
{
    public class RegistryContext : DbContext
    {
        public DbSet<Mod> Mods { get; set; }
        public DbSet<ModVersion> ModVersions { get; set; }
        
        public RegistryContext(DbContextOptions<RegistryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ModVersion>(entity =>
            {
                entity.Property(v => v.Dependencies)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v, Formatting.None),
                        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v)
                    );

                entity.HasIndex(v => new {v.Name, v.Version})
                    .IsUnique();
            });
        }
    }
}