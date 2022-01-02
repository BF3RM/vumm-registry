using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class ModAuthorizationHandlerTests
    {
        private ModAuthorizationHandler _handler;
        private Mock<IModAuthorizationService> _serviceMock;
        private Mock<ClaimsPrincipal> _principalMock;

        [SetUp]
        public void Setup()
        {
            _principalMock = new Mock<ClaimsPrincipal>();
            
            _serviceMock = new Mock<IModAuthorizationService>();
            _handler = new ModAuthorizationHandler(_serviceMock.Object);
        }

        [Test]
        public async Task Authorize_ReadShouldSucceedIfNotPrivate()
        {
            var mod = new Mod {IsPrivate = false};
            await AssertRequirementSucceed(mod, ModOperations.Read);
        }

        [Test]
        public async Task Authorize_ReadShouldFailIfPrivateAndNoPermissions()
        {
            var mod = new Mod {IsPrivate = true};
            await AssertRequirementSucceed(mod, ModOperations.Read, false);
        }

        [Test]
        public async Task Authorize_ReadShouldSucceedIfPrivateAndPermissions()
        {
            _serviceMock.Setup(s =>
                    s.HasAnyPermissions(It.IsAny<long>(), It.IsAny<long>(), ModPermission.Read,
                        ModPermission.Write))
                .ReturnsAsync(true);
            var mod = new Mod {IsPrivate = true};
            await AssertRequirementSucceed(mod, ModOperations.Read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Authorize_PublishShouldSucceedIfPublishPermissions(bool hasPermission)
        {
            _serviceMock.Setup(s => s.HasAnyPermissions(It.IsAny<long>(), It.IsAny<long>(), ModPermission.Write))
                .ReturnsAsync(hasPermission);

            await AssertRequirementSucceed(new Mod(), ModOperations.Publish, hasPermission);
        }

        private async Task AssertRequirementSucceed(Mod mod, OperationAuthorizationRequirement requirement, bool succeed = true)
        {
            var contextMock = new Mock<AuthorizationHandlerContext>(
                new[] {requirement}, _principalMock.Object, mod) {CallBase = true};

            await _handler.HandleAsync(contextMock.Object);

            var timesCalled = succeed ? Times.Once() : Times.Never();
            contextMock.Verify(c => c.Succeed(requirement), timesCalled);
        }
    }
}