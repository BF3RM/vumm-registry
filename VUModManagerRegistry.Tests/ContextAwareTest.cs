using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Tests
{
    public abstract class ContextAwareTest
    {
        protected AppDbContext Context { get; set; }

        [SetUp]
        public void CreateContext()
        {
            Context = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("TestDb").Options);
        }
    }
}