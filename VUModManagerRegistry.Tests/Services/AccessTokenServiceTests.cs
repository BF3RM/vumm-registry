using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class AccessTokenServiceTests
    {
        private IAccessTokenService _service;
        private Mock<IAccessTokenRepository> _repositoryMock;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAccessTokenRepository>();
            _service = new AccessTokenService(_repositoryMock.Object);
        }

        [Test]
        public async Task Revoke_ShouldRemoveExistingToken()
        {
            var accessToken = new UserAccessToken()
            {
                Id = 1,
                UserId = 1,
                Token = Guid.NewGuid()
            };
            _repositoryMock.Setup(r => r.FindByUserIdAndTokenAsync(accessToken.UserId, accessToken.Token))
                .ReturnsAsync(accessToken);

            _repositoryMock.Setup(r => r.DeleteByIdAsync(accessToken.Id)).ReturnsAsync(true);
            
            Assert.IsTrue(await _service.Revoke(accessToken.UserId, accessToken.Token));
        }

        [Test]
        public async Task Revoke_ShouldReturnFalseIfUnknown()
        {
            Assert.IsFalse(await _service.Revoke(1, Guid.NewGuid()));
        }

        [Test]
        public async Task GetAll_ShouldReturnUsersTokens()
        {
            var tokens = new List<UserAccessToken>()
            {
                new UserAccessToken(),
                new UserAccessToken()
            };
            _repositoryMock
                .Setup(r => r.FindAllByUserIdAsync(1))
                .ReturnsAsync(tokens);
            
            Assert.AreEqual(2, (await _service.GetAll(1)).Count);
        }
    }
}