using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;

namespace VUModManagerRegistry.Tests.Repositories
{
    [TestFixture]
    public class AccessTokenRepositoryTests : ContextAwareTest
    {
        private IAccessTokenRepository _repository;
        private User _createdUserOne, _createdUserTwo;
        private List<UserAccessToken> _createdTokensOne, _createdTokensTwo;
        
        [SetUp]
        public async Task Setup()
        {
            _repository = new AccessTokenRepository(Context);

            _createdUserOne = new User()
            {
                Username = "testuser",
                Password = "pass"
            };
            await Context.Users.AddAsync(_createdUserOne);

            _createdUserTwo = new User()
            {
                Username = "otheruser",
                Password = "pass"
            };
            await Context.Users.AddAsync(_createdUserTwo);
            await Context.SaveChangesAsync();

            _createdTokensOne = new List<UserAccessToken>()
            {
                new()
                {
                    UserId = _createdUserOne.Id,
                    Token = Guid.NewGuid(),
                    Type = AccessTokenType.ReadOnly
                },
                new()
                {
                    UserId = _createdUserOne.Id,
                    Token = Guid.NewGuid(),
                    Type = AccessTokenType.ReadOnly
                }
            };
            await Context.AccessTokens.AddRangeAsync(_createdTokensOne);
            
            _createdTokensTwo = new List<UserAccessToken>()
            {
                new()
                {
                    UserId = _createdUserTwo.Id,
                    Token = Guid.NewGuid(),
                    Type = AccessTokenType.ReadOnly
                },
                new()
                {
                    UserId = _createdUserTwo.Id,
                    Token = Guid.NewGuid(),
                    Type = AccessTokenType.ReadOnly
                }
            };
            await Context.AccessTokens.AddRangeAsync(_createdTokensTwo);
            await Context.SaveChangesAsync();
        }

        [Test]
        public async Task FindAll_FindsUserTokens()
        {
            Assert.AreEqual(_createdTokensOne.Count, (await _repository.FindAllByUserIdAsync(_createdUserOne.Id)).Count);
        }

        [Test]
        public async Task FindByToken_FindsUserToken()
        {
            var foundToken = await _repository.FindByTokenAsync(_createdTokensOne[0].Token);
            Assert.NotNull(foundToken);
            Assert.AreEqual(_createdUserOne.Id, foundToken.UserId);
        }

        [Test]
        public async Task FindByUserIdAndToken_FindsCorrectToken()
        {
            var foundToken = await _repository.FindByUserIdAndTokenAsync(_createdUserOne.Id, _createdTokensOne[0].Token);
            Assert.NotNull(foundToken);
        }
        
    }
}