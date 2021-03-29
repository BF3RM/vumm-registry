using System.Threading.Tasks;
using NUnit.Framework;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;

namespace VUModManagerRegistry.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests : ContextAwareTest
    {
        private IUserRepository _repository;

        [SetUp]
        public async Task Setup()
        {
            _repository = new UserRepository(Context);

            var user = new User()
            {
                Username = "testuser",
                Password = "pass"
            };
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
        }

        [TestCase("testuser", true)]
        [TestCase("wronguser", false)]
        public async Task Exists_ChecksIfUserExistsByUsername(string username, bool exists)
        {
            Assert.AreEqual(exists, await _repository.ExistsByUsernameAsync(username));
        }

        [Test]
        public async Task Find_FindsUserByUsername()
        {
            Assert.IsNotNull(await _repository.FindByUsernameAsync("testuser"));
        }
    }
}