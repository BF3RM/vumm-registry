using System;
using System.Collections.Generic;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;
using VUModManagerRegistry.Models.Extensions;

namespace VUModManagerRegistry.Tests.Extensions
{
    [TestFixture]
    public class AccessTokenExtensionsTests
    {
        [Test]
        public void ConvertToDto()
        {
            var accessToken = new UserAccessToken()
            {
                Token = Guid.NewGuid(),
                Type = AccessTokenType.Publish
            };

            var dto = accessToken.ToDto();
            AssertDtoEqual(accessToken, dto);
        }

        [Test]
        public void ConvertToDtoList()
        {
            var accessTokens = new List<UserAccessToken>()
            {
                new()
                {
                    Token = Guid.NewGuid(),
                    Type = AccessTokenType.Publish
                },
                new()
                {
                    Token = Guid.NewGuid(),
                    Type = AccessTokenType.Readonly
                }
            };

            var dtos = accessTokens.ToDtoList();
            Assert.AreEqual(accessTokens.Count, dtos.Count);
            AssertDtoEqual(accessTokens[0], dtos[0]);
            AssertDtoEqual(accessTokens[1], dtos[1]);
        }

        private static void AssertDtoEqual(UserAccessToken accessToken, UserAccessTokenDto dto)
        {
            Assert.AreEqual(accessToken.Type, dto.Type);
            Assert.AreEqual(accessToken.Token, dto.Token);
            Assert.AreEqual(accessToken.CreatedAt, dto.CreatedAt);
        }
    }
}