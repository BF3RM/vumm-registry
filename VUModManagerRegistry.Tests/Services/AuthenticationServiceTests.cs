using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private IAuthenticationService _service;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IAccessTokenRepository> _accessTokenRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _accessTokenRepositoryMock = new Mock<IAccessTokenRepository>();
            _service = new AuthenticationService(_userRepositoryMock.Object, _accessTokenRepositoryMock.Object);
        }

        [Test]
        public void Register_FailsIfUserExists()
        {
            _userRepositoryMock
                .Setup(r => r.ExistsByUsernameAsync("testuser")).ReturnsAsync(true);
            
            var credentials = new CredentialsDto()
            {
                Username = "testuser",
                Password = "pass"
            };
            Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _service.Register(credentials));
        }

        [Test]
        public async Task Register_CreatesUserWithCorrectPassword()
        {
            User user = null;
            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => user = u)
                .ReturnsAsync(user);
            
            var credentials = new CredentialsDto()
            {
                Username = "newuser",
                Password = "pass"
            };

            await _service.Register(credentials);
            
            Assert.AreEqual(credentials.Username, user.Username);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password));
        }

        [TestCase("testuser", "testuser", "pass", "pass", true)]
        [TestCase("testuser", "wronguser", "pass", "pass", false)]
        [TestCase("testuser", "testuser", "pass", "pass", true)]
        [TestCase("testuser", "testuser", "pass", "wrongpass", false)]
        public async Task Login_OnlyValidCorrectCredentials(string storedUsername, string actualUsername, string storedPassword, string actualPassword, bool valid)
        {
            _userRepositoryMock
                .Setup(r => r.FindByUsernameAsync(storedUsername))
                .ReturnsAsync(new User()
                {
                    Username = storedUsername,
                    Password = BCrypt.Net.BCrypt.HashPassword(storedPassword)
                });
            
            var credentials = new CredentialsDto()
            {
                Username = actualUsername,
                Password = actualPassword
            };

            var res = await _service.Login(credentials);
            Assert.AreEqual(valid, res.IsValid);
        }

        [Test]
        public async Task VerifyToken_InvalidOnUnknownToken()
        {
            var res = await _service.VerifyToken(Guid.NewGuid());
            Assert.IsFalse(res.IsValid);
        }
        
        [Test]
        public async Task VerifyToken_ValidOnKnownToken()
        {
            var user = new User()
            {
                Id = 1
            };
            var accessToken = new UserAccessToken()
            {
                Id = 1,
                UserId = user.Id,
                Token = Guid.NewGuid(),
                Type = AccessTokenType.ReadOnly
            };

            _accessTokenRepositoryMock.Setup(r => r.FindByTokenAsync(accessToken.Token))
                .ReturnsAsync(accessToken);

            _userRepositoryMock.Setup(r => r.FindByIdAsync(user.Id))
                .ReturnsAsync(user);


            var (isValid, foundUser, tokenType) = await _service.VerifyToken(accessToken.Token);
            Assert.IsTrue(isValid);
            Assert.AreEqual(user, foundUser);
            Assert.AreEqual(tokenType, AccessTokenType.ReadOnly);
        }
    }
}