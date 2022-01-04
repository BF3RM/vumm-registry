using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Tests
{
    public abstract class ContextAwareTest
    {
        protected AppDbContext Context { get; set; }

        [SetUp]
        public void CreateContext()
        {
            Context = new AppDbContext(
                new SystemTimeProvider(),
                new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options);
        }
    }
}