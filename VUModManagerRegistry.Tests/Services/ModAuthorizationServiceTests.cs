using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class ModAuthorizationServiceTests
    {
        private const long ModId = 1;
        private const long UserId = 1;

        private IModAuthorizationService _service;
        private Mock<IModUserPermissionRepository> _repositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IModUserPermissionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _service = new ModAuthorizationService(_repositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task HasAnyPermissions_ShouldReturnFalseIfNullPermissions()
        {
            Assert.IsFalse(await _service.HasAnyPermissions(ModId, UserId, ModPermission.Read));
        }
        
        [Test]
        public async Task HasAnyPermissionsWithTag_ShouldReturnFalseIfNullPermissions()
        {
            Assert.IsFalse(await _service.HasAnyPermissions(ModId, UserId, "latest", ModPermission.Read));
        }

        [Test]
        public async Task HasAnyPermissions_ShouldReturnFalseIfNoneMatch()
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Read};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId))
                .ReturnsAsync(modUserPermission);
            
            Assert.IsFalse(await _service.HasAnyPermissions(ModId, UserId, ModPermission.Write));
        }
        
        [Test]
        public async Task HasAnyPermissionsWithTag_ShouldReturnFalseIfNoneMatch()
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Read};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId, "latest"))
                .ReturnsAsync(modUserPermission);
            
            Assert.IsFalse(await _service.HasAnyPermissions(ModId, UserId, "latest", ModPermission.Write));
        }

        [Test]
        public async Task HasAnyPermissions_ShouldReturnTrueIfOneMatches()
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Read};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId))
                .ReturnsAsync(modUserPermission);
            
            Assert.IsTrue(await _service.HasAnyPermissions(ModId, UserId, ModPermission.Write, ModPermission.Read));
        }
        
        [Test]
        public async Task HasAnyPermissionsWithTags_ShouldReturnTrueIfOneMatches()
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Read};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId, "latest", ""))
                .ReturnsAsync(modUserPermission);
            
            Assert.IsTrue(await _service.HasAnyPermissions(ModId, UserId, "latest", ModPermission.Write, ModPermission.Read));
        }
        
        [Test]
        public async Task SetPermission_ShouldAddIfNotExisting()
        {
            Assert.IsTrue(await _service.SetPermission(ModId, UserId, ModPermission.Read));
            
            _repositoryMock.Verify(r => r.AddAsync(
                It.Is<ModUserPermission>(p => p.Permission == ModPermission.Read)
                ), Times.Once());
        }

        [TestCase(ModPermission.Write, true)]
        [TestCase(ModPermission.Read, false)]
        public async Task SetPermission_ShouldUpdateExistingPermissionIfDifferent(ModPermission setToPermission, bool changed)
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Read, Tag = ""};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId, ""))
                .ReturnsAsync(modUserPermission);
            
            Assert.AreEqual(changed, await _service.SetPermission(ModId, UserId, setToPermission));

            var timesUpdateCalled = changed ? Times.Once() : Times.Never();
            _repositoryMock.Verify(r => r.UpdateAsync(
                    It.Is<ModUserPermission>(p => p.Permission == setToPermission)
                    ), timesUpdateCalled);
        }

        [Test]
        public async Task SetPermission_ShouldResolveUserId()
        {
            var user = new User {Id = UserId, Username = "user"};
            _userRepositoryMock
                .Setup(r => r.FindByUsernameAsync(user.Username))
                .ReturnsAsync(user);

            await _service.SetPermission(ModId, user.Username, "latest", ModPermission.Read);
            
            _repositoryMock.Verify(r => r.FindAsync(ModId, UserId, "latest"), Times.Once);
        }

        [Test]
        public async Task SetPermission_ShouldNotAddIfUserNotFound()
        {
            Assert.IsFalse(await _service.SetPermission(ModId, "user", "latest", ModPermission.Read));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RevokePermissions_ShouldReturnTrueIfRemoved(bool removed)
        {
            _repositoryMock
                .Setup(r => r.DeleteAsync(ModId, UserId, ""))
                .ReturnsAsync(removed);
            
            Assert.AreEqual(removed, await _service.RevokePermissions(ModId, UserId, ""));
        }
        
        [Test]
        public async Task RevokePermissions_ShouldResolveUserId()
        {
            var user = new User {Id = UserId, Username = "user"};
            _userRepositoryMock
                .Setup(r => r.FindByUsernameAsync(user.Username))
                .ReturnsAsync(user);

            await _service.RevokePermissions(ModId, user.Username, "latest");
            
            _repositoryMock.Verify(r => r.DeleteAsync(ModId, UserId, "latest"), Times.Once);
        }
        
        [Test]
        public async Task RevokePermissions_ShouldReturnFalseIfUserNotFound()
        {
            Assert.IsFalse(await _service.RevokePermissions(ModId, "user", "latest"));
        }
    }
}