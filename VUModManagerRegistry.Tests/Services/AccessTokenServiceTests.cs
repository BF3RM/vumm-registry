using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class AccessTokenServiceTests : ContextAwareTest
    {
        private IAccessTokenService _service;
        private User _registeredUser;

        [SetUp]
        public async Task Setup()
        {
            _service = new AccessTokenService(Context);

            _registeredUser = new User()
            {
                Username = "test",
                Password = "hashedpass"
            };
            await Context.Users.AddAsync(_registeredUser);
            await Context.SaveChangesAsync();
        }

        [Test]
        public async Task Create_ShouldDefaultCreateReadonlyToken()
        {
            var token = await _service.Create(_registeredUser);
            Assert.NotNull(token.Id);
            Assert.AreEqual(_registeredUser.Id, token.UserId);
            Assert.AreEqual(AccessTokenType.ReadOnly, token.Type);

            Assert.IsTrue(await Context.AccessTokens.AnyAsync(t => t.Id == token.Id));
        }

        [Test]
        public async Task Create_ShouldCreateGivenTokenType()
        {
            var token = await _service.Create(_registeredUser, AccessTokenType.Publish);
            Assert.AreEqual(AccessTokenType.Publish, token.Type);
        }

        [Test]
        public async Task Verify_ShouldFailWhenInvalid()
        {
            var result = await _service.Verify(Guid.NewGuid());
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.User);
        }

        [Test]
        public async Task Verify_ShouldSucceedWhenValid()
        {
            var token = new UserAccessToken()
            {
                UserId = _registeredUser.Id,
                Token = Guid.NewGuid(),
                Type = AccessTokenType.ReadOnly
            };
            await Context.AccessTokens.AddAsync(token);
            await Context.SaveChangesAsync();

            var result = await _service.Verify(token.Token);
            Assert.IsTrue(result.IsValid);
            Assert.IsNotNull(result.User);
            Assert.AreEqual(token.UserId, result.User.Id);
        }
    }
}