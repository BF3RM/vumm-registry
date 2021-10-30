using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Common.Exceptions;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class ModServiceTests
    {
        private const long ModId = 1;
        private const long UserId = 1;
        private const string ModName = "testmod";
        private const string ModVersion = "1.0.0";
        
        private IModService _service;
        private Mock<IModRepository> _modRepositoryMock;
        private Mock<IModVersionRepository> _versionRepositoryMock;
        private Mock<IModAuthorizationService> _authServiceMock;
        private Mock<IModStorage> _modStorage;

        [SetUp]
        public void Setup()
        {
            _modRepositoryMock = new Mock<IModRepository>();
            _versionRepositoryMock = new Mock<IModVersionRepository>();
            _authServiceMock = new Mock<IModAuthorizationService>();
            _modStorage = new Mock<IModStorage>();
            _service = new ModService(_modRepositoryMock.Object, _versionRepositoryMock.Object, _modStorage.Object, _authServiceMock.Object);
        }

        [Test]
        public async Task GetMod_PassesThroughRepository()
        {
            var mod = new Mod {Name = ModName};
            _modRepositoryMock
                .Setup(r => r.FindByNameAsync(ModName))
                .ReturnsAsync(mod);
            
            Assert.AreEqual(mod, await _service.GetMod(ModName));
        }
        
        [Test]
        public async Task GetAllowedModVersions_PassesThroughRepository()
        {
            var versions = new List<ModVersion>() {new() {Name = ModName}};
            _versionRepositoryMock
                .Setup(r => r.FindAllowedVersions(ModName, UserId))
                .ReturnsAsync(versions);
            
            Assert.AreEqual(versions, await _service.GetAllowedModVersions(ModName, UserId));
        }
        
        [Test]
        public async Task DeleteMod_PassesThroughRepository()
        {
            _modRepositoryMock
                .Setup(r => r.DeleteByNameAsync(ModName))
                .ReturnsAsync(true);
            
            Assert.IsTrue(await _service.DeleteMod(ModName));
        }

        [Test]
        public void CreateModVersion_ThrowsExceptionIfAlreadyExists()
        {
            var request = new CreateModVersionRequest {Name = ModName, Version = ModVersion, Tag = "latest"};
            
            _versionRepositoryMock
                .Setup(r => r.ExistsByNameAndVersionAsync(ModName, ModVersion))
                .ReturnsAsync(true);

            Assert.ThrowsAsync<ModVersionAlreadyExistsException>(async () =>
                await _service.CreateModVersion(request, UserId));
        }

        [Test]
        public async Task CreateModVersion_CreatesModAndAddsPermissionsIfNotExists()
        {
            var request = new CreateModVersionRequest {Name = ModName, Version = ModVersion, Tag = "latest"};

            Assert.IsNotNull(await _service.CreateModVersion(request, UserId));
            
            _modRepositoryMock.Verify(r => r.AddAsync(It.Is<Mod>(m => m.Name == request.Name)));
            _authServiceMock.Verify(s => s.SetPermission(0, UserId, ModPermission.Write));
            _versionRepositoryMock.Verify(r =>
                r.AddAsync(It.Is<ModVersion>(
                    v => v.Name == request.Name && v.Version == request.Version && v.Tag == "latest")));
            _modStorage.Verify(s => s.StoreArchive(ModName, ModVersion, null));
        }

        [Test]
        public async Task CreateModVersion_UsesExistingMod()
        {
            var mod = new Mod {Id = ModId, Name = ModName};
            var request = new CreateModVersionRequest {Name = ModName, Version = ModVersion, Tag = "latest"};
            
            _modRepositoryMock.Setup(r => r.FindByNameAsync(ModName)).ReturnsAsync(mod);

            var version = await _service.CreateModVersion(request, UserId);
            
            Assert.AreEqual(ModId, version.ModId);
        }

        [Test]
        public async Task DeleteModVersion_ReturnsFalseIfNotFound()
        {
            Assert.IsFalse(await _service.DeleteModVersion(ModName, ModVersion));
        }

        [Test]
        public async Task DeleteModVersion_DeletesModIfNoVersionsLeft()
        {
            var mod = new Mod {Id = ModId, Versions = new List<ModVersion>()};
            
            _versionRepositoryMock
                .Setup(r => r.DeleteByNameAndVersionAsync(ModName, ModVersion))
                .ReturnsAsync(true);

            _modRepositoryMock.Setup(r => r.FindByNameWithVersionsAsync(ModName)).ReturnsAsync(mod);
            
            Assert.IsTrue(await _service.DeleteModVersion(ModName, ModVersion));
            
            _modRepositoryMock.Verify(r => r.DeleteByIdAsync(ModId));
        }
        
        [Test]
        public async Task ModVersionExists_PassesThroughRepository()
        {
            _versionRepositoryMock
                .Setup(r => r.ExistsByNameAndVersionAsync(ModName, ModVersion))
                .ReturnsAsync(true);
            
            Assert.IsTrue(await _service.ModVersionExists(ModName, ModVersion));
        }
        
        [Test]
        public async Task GetModVersion_PassesThroughRepository()
        {
            var version = new ModVersion {Name = ModName, Version = ModVersion};
            _versionRepositoryMock
                .Setup(r => r.FindByNameAndVersion(ModName, ModVersion))
                .ReturnsAsync(version);
            
            Assert.AreEqual(version, await _service.GetModVersion(ModName, ModVersion));
        }
    }
}