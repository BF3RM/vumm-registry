using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Tests;

public class AppTestDbContext : AppDbContext
{
    public AppTestDbContext() : base(
        new SystemTimeProvider(),
        new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // We need to tweak the "jsonb" as the in memory database does not support this type
        modelBuilder.Entity<ModVersion>()
            .Property(v => v.Dependencies)
            .HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)
        );
    }
}