using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class ModVersionAuthorizationHandlerTests
    {
        private ModVersionAuthorizationHandler _handler;
        private Mock<IModAuthorizationService> _serviceMock;
        private Mock<ClaimsPrincipal> _principalMock;

        [SetUp]
        public void Setup()
        {
            _principalMock = new Mock<ClaimsPrincipal>();
            
            _serviceMock = new Mock<IModAuthorizationService>();
            _handler = new ModVersionAuthorizationHandler(_serviceMock.Object);
        }

        [Test]
        public async Task Authorize_ReadShouldSucceedIfNotPrivate()
        {
            var version = new ModVersion
            {
                Mod = new Mod {IsPrivate = false}
            };
            
            await AssertRequirementSucceed(version, ModOperations.Read);
        }

        [Test]
        public async Task Authorize_ReadShouldFailIfPrivateAndNoPermissions()
        {
            var version = new ModVersion
            {
                Mod = new Mod {IsPrivate = true}
            };
            await AssertRequirementSucceed(version, ModOperations.Read, false);
        }

        [Test]
        public async Task Authorize_ReadShouldSucceedIfPrivateAndPermissions()
        {
            _serviceMock.Setup(s =>
                    s.HasAnyPermissions(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), ModPermission.Read,
                        ModPermission.Write))
                .ReturnsAsync(true);
            var version = new ModVersion
            {
                Mod = new Mod {IsPrivate = true}
            };
            await AssertRequirementSucceed(version, ModOperations.Read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Authorize_PublishShouldSucceedIfPublishPermissions(bool hasPermission)
        {
            _serviceMock.Setup(s => s.HasAnyPermissions(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), ModPermission.Write))
                .ReturnsAsync(hasPermission);

            await AssertRequirementSucceed(new ModVersion(), ModOperations.Publish, hasPermission);
        }

        private async Task AssertRequirementSucceed(ModVersion mod, OperationAuthorizationRequirement requirement, bool succeed = true)
        {
            var contextMock = new Mock<AuthorizationHandlerContext>(
                new[] {requirement}, _principalMock.Object, mod) {CallBase = true};

            await _handler.HandleAsync(contextMock.Object);

            var timesCalled = succeed ? Times.Once() : Times.Never();
            contextMock.Verify(c => c.Succeed(requirement), timesCalled);
        }
    }
}