using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests : ContextAwareTest
    {
        private AuthenticationService _service;

        [SetUp]
        public async Task Setup()
        {
            var registeredUser = new User()
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("pass")
            };
            await Context.Users.AddAsync(registeredUser);
            await Context.SaveChangesAsync();

            var registeredAccessToken = new UserAccessToken()
            {
                UserId = registeredUser.Id,
                Token = Guid.NewGuid(),
                Type = AccessTokenType.ReadOnly
            };
            await Context.AccessTokens.AddAsync(registeredAccessToken);
            await Context.SaveChangesAsync();
            
            var tokenServiceMock = new Mock<IAccessTokenService>();
            tokenServiceMock
                .Setup(s => s.Create(It.IsAny<User>(), AccessTokenType.ReadOnly))
                .ReturnsAsync(registeredAccessToken);
            
            _service = new AuthenticationService(Context, tokenServiceMock.Object);
        }

        [Test]
        public void Register_FailsIfUserExists()
        {
            var credentials = new CredentialsDto()
            {
                Username = "testuser",
                Password = "pass"
            };
            Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _service.Register(credentials));
        }

        [Test]
        public async Task Register_CreatesUser()
        {
            var credentials = new CredentialsDto()
            {
                Username = "newuser",
                Password = "pass"
            };

            await _service.Register(credentials);

            Assert.IsTrue(await Context.Users
                .AnyAsync(u => u.Username == credentials.Username));
        }

        [TestCase("newuser", "pass", false)]
        [TestCase("testuser", "wrongpass", false)]
        [TestCase("testuser", "pass", true)]
        public async Task Login_OnlyValidCorrectCredentials(string username, string password, bool valid)
        {
            var credentials = new CredentialsDto()
            {
                Username = username,
                Password = password
            };

            var res = await _service.Login(credentials);
            Assert.AreEqual(valid, res.IsValid);
        }
    }
}