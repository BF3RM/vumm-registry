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

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IModUserPermissionRepository>();
            _service = new ModAuthorizationService(_repositoryMock.Object, new Mock<IUserRepository>().Object);
        }

        [Test]
        public async Task HasAnyPermissions_ShouldReturnFalseIfNullPermissions()
        {
            Assert.IsFalse(await _service.HasAnyPermissions(ModId, UserId, ModPermission.Readonly));
        }

        [Test]
        public async Task HasAnyPermissions_ShouldReturnFalseIfNoneMatch()
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Readonly};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId))
                .ReturnsAsync(modUserPermission);
            
            Assert.IsFalse(await _service.HasAnyPermissions(ModId, UserId, ModPermission.Publish));
        }

        [Test]
        public async Task HasAnyPermissions_ShouldReturnTrueIfOneMatches()
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Readonly};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId))
                .ReturnsAsync(modUserPermission);
            
            Assert.IsTrue(await _service.HasAnyPermissions(ModId, UserId, ModPermission.Publish, ModPermission.Readonly));
        }
        
        [Test]
        public async Task SetPermission_ShouldAddIfNotExisting()
        {
            Assert.IsTrue(await _service.SetPermission(ModId, UserId, ModPermission.Readonly));
            
            _repositoryMock.Verify(r => r.AddAsync(
                It.Is<ModUserPermission>(p => p.Permission == ModPermission.Readonly)
                ), Times.Once());
        }

        [TestCase(ModPermission.Publish, true)]
        [TestCase(ModPermission.Readonly, false)]
        public async Task SetPermission_ShouldUpdateExistingPermissionIfDifferent(ModPermission setToPermission, bool changed)
        {
            var modUserPermission = new ModUserPermission {Permission = ModPermission.Readonly};
            _repositoryMock
                .Setup(r => r.FindAsync(ModId, UserId))
                .ReturnsAsync(modUserPermission);
            
            Assert.AreEqual(changed, await _service.SetPermission(ModId, UserId, setToPermission));

            var timesUpdateCalled = changed ? Times.Once() : Times.Never();
            _repositoryMock.Verify(r => r.UpdateAsync(
                    It.Is<ModUserPermission>(p => p.Permission == setToPermission)
                    ), timesUpdateCalled);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RevokePermissions_ShouldReturnTrueIfRemoved(bool removed)
        {
            _repositoryMock
                .Setup(r => r.DeleteByModAndUserIdAsync(ModId, UserId))
                .ReturnsAsync(removed);
            
            Assert.AreEqual(removed, await _service.RevokePermissions(ModId, UserId));
        }
    }
}