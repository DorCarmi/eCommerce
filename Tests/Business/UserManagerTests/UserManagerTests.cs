using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.Business.UserManagerTests
{
    [TestFixture]
    public class UserManagerTests
    {
        private UserManager _userManager;

        [SetUp]
        public void Setup()
        {
            _userManager = new UserManager(new UserAuthMock(), new RegisteredUserRepositoryMock());
        }
        
        [Test, Order(1)]
        public void SyncConnectTest()
        {
            const int numberOfConnections = 5;

            ISet<string> guestsUsername = new HashSet<string>();
            
            for (int i = 0; i < 5; i++)
            {
                Result<IUser> connectRes = _userManager.GetUserIfConnectedOrLoggedIn(_userManager.Connect());
                Assert.True(connectRes.IsSuccess,
                    $"The connect method didnt connect the user");

                Console.WriteLine($"Created new guest {connectRes.Value.Username}");
                guestsUsername.Add(connectRes.Value.Username);
            }
            
            Assert.AreEqual(numberOfConnections,
                guestsUsername.Count,
                "The connect methods created guest with the same name");
        }
        
        [Test, Repeat(2)]
        public async Task ConcurrentConnectTest()
        {
            Console.WriteLine("\nConcurrentConnectTest\n");
            const int numberOfTasks = 10;
            string[] tokens = await TaskTestUtils.CreateAndRunTasks(
                () => _userManager.Connect(),
                numberOfTasks);

            ISet<string> usernames = new HashSet<string>();

            for (var i = 0; i < numberOfTasks; i++)
            {
                Result<IUser> connectRes = _userManager.GetUserIfConnectedOrLoggedIn(tokens[i]);
                Assert.True(connectRes.IsSuccess,
                    $"The guest is connect therefore the token should be valid\nError: {connectRes.Error}");
                usernames.Add(connectRes.Value.Username);
                Console.WriteLine($"Created name: {connectRes.Value.Username}");
            }

            Assert.AreEqual(numberOfTasks,
                usernames.Count,
                $"There are duplicate usernames");
            }
        }
}