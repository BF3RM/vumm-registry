using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;
using VUModManagerRegistry.Models.Extensions;

namespace VUModManagerRegistry.Tests.Extensions
{
    [TestFixture]
    public class ModExtensionsTests
    {
        [Test]
        public void ConvertModVersionToDto()
        {
            var version = new ModVersion
            {
                Name = "testmod",
                Description = "testdesc",
                Author = "testauthor",
                Version = "1.0.0",
                Dependencies = new Dictionary<string, string>()
            };

            var dto = version.ToDto();
            AssertVersionEqual(version, dto);
        }

        [Test]
        public void ConvertModToDto()
        {
            var mod = new Mod
            {
                Name = "testmod",
                Description = "testdesc",
                Author = "testauthor",
                Versions = new List<ModVersion>()
                {
                    new ModVersion
                    {
                        Name = "testmod",
                        Description = "testdesc",
                        Author = "testauthor",
                        Version = "1.0.0",
                        Tag = "latest",
                        Dependencies = new Dictionary<string, string>()
                    }
                }
            };
            var dto = mod.ToDto();
            Assert.AreEqual(mod.Name, dto.Name);
            Assert.AreEqual(mod.Description, dto.Description);
            Assert.AreEqual(mod.Author, dto.Author);
            Assert.AreEqual(1, dto.Tags.Count);
            
            AssertVersionEqual(mod.Versions.First(), dto.Versions.First().Value);
        }

        private static void AssertVersionEqual(ModVersion version, ModVersionDto dto)
        {
            Assert.AreEqual(version.Name, dto.Name);
            Assert.AreEqual(version.Description, dto.Description);
            Assert.AreEqual(version.Author, dto.Author);
            Assert.AreEqual(version.Version, dto.Version);
            Assert.AreEqual(version.Dependencies, dto.Dependencies);
        }
    }
}