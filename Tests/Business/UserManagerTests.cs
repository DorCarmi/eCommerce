using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace Tests.Business
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

        [Test, Repeat(2)]
        public async Task ConcurrentRegisterSameUserTest()
        {
            const int numberOfTasks = 5;

            MemberInfo memberInfo = new MemberInfo("user1", "email@email.com", "user", DateTime.Now, "Sea street 1");
            Result[] registerRes = await TaskTestUtils.CreateAndRunTasks(
                () =>
                {
                    string token = _userManager.Connect();
                    return _userManager.Register(token, memberInfo, "password1");
                },
                numberOfTasks);

            int registeredSuccessfully = 0;
            foreach (var res in registerRes)
            {
                if (res.IsSuccess)
                {
                    registeredSuccessfully++;
                }
            }

            Assert.AreEqual(1,
                registeredSuccessfully,
                $"Only one task should has been able to register but {registeredSuccessfully} succeeded");
        }

        [Test, Repeat(2)]
        public async Task ConcurrentLoginSameUserTest()
        {
            const int numberOfTasks = 5;
            string password = "passwrod1"; 
            string token = _userManager.Connect();
            MemberInfo memberInfo = new MemberInfo("user1", "email@email.com", "user", DateTime.Now, "Sea street 1");
            Result registerRes = _userManager.Register(token, memberInfo, password);
            if (registerRes.IsFailure)
            {
                Assert.Fail("The registration of the user didnt work");
            }

            Result[] loginRes = await TaskTestUtils.CreateAndRunTasks(
                () => _userManager.Login(token, memberInfo.Username, password, ServiceUserRole.Member),
                numberOfTasks);

            int registeredSuccessfully = 0;
            foreach (var res in loginRes)
            {
                if (res.IsSuccess)
                {
                    registeredSuccessfully++;
                }
            }

            Assert.AreEqual(1,
                registeredSuccessfully,
                $"Only one task should has been able to login but {registeredSuccessfully} succeeded");
        }

        [Test]
        public async Task ConcurrentLoginFewUsersTest()
        {
            const int numberOfTasks = 5;
            string username = "user";

            string password = "password";

            string[] guestTokens = new string[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                guestTokens[i] = _userManager.Connect();
                MemberInfo memberInfo = new MemberInfo($"{username}{i}", "email@email.com", "user", DateTime.Now, "Sea street 1");
                Result registerRes = _userManager.Register(guestTokens[i], memberInfo, $"{password}{i}");
                if (registerRes.IsFailure)
                {
                    Assert.Fail($"The registration of the user didnt work\nError:{registerRes.Error}");
                }
            }

            Task<Result<string>>[] loginTasks = new Task<Result<string>>[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                string uname = $"{username}{i}";
                string upassword = $"{password}{i}";
                string token = guestTokens[i];
                loginTasks[i] = new Task<Result<string>>(
                    () => _userManager.Login(token, uname, upassword, ServiceUserRole.Member));
            }

            TaskTestUtils.RunTasks(loginTasks);
            Result<string>[] loginRes = await Task.WhenAll(loginTasks);

            int numberOfUsersLoggedIn = 0;
            foreach (var loginTokenRes in loginRes)
            {
                if (loginTokenRes.IsSuccess)
                {
                    numberOfUsersLoggedIn++;
                }
                else
                {
                    Console.WriteLine(loginTokenRes.Error);
                }
            }

            Assert.AreEqual(numberOfTasks,
                numberOfUsersLoggedIn,
                $"All the users should have been able to login but {numberOfUsersLoggedIn} succeeded");
        }

        [Test]
        public void LoginLogoutInARow()
        {
            string username = "user";
            string password = "password";
            MemberInfo memberInfo = new MemberInfo(username, "email@email.com", "user", DateTime.Now, "Sea street 1");

            
            string token = _userManager.Connect();
            Result registrationRes = _userManager.Register(token, memberInfo, password);
            Assert.True(registrationRes.IsSuccess, registrationRes.Error);
            
            for (int i = 0; i < 3; i++)
            {
                Result<string> loginTokenRes = _userManager.Login(token, username, password, ServiceUserRole.Member);
                Assert.True(loginTokenRes.IsSuccess, loginTokenRes.Error);
                Result<string> logoutTokenRes = _userManager.Logout(loginTokenRes.Value);
                Assert.True(logoutTokenRes.IsSuccess, logoutTokenRes.Error);
                token = logoutTokenRes.Value;
            }
        }
    }
}