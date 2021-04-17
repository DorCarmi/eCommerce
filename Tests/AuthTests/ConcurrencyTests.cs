using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Common;
using NUnit.Framework;

namespace Tests.AuthTests
{
    [TestFixture]
    public class ConcurrencyTests
    {

        private IUserAuth _auth; 
        
        [SetUp]
        public void Setup()
        {
            _auth = UserAuth.CreateInstanceForTests(new TRegisteredUserRepo());
        }

        [Test]
        public async Task ConcurrentConnectTest()
        {
            const int numberOfTasks = 10;
            string[] tokens = await TaskTestUtils.CreateAndRunTasks(
                () => _auth.Connect(),
                numberOfTasks);
            
            ISet<string> usernames = new HashSet<string>();
            
            for (var i = 0; i < numberOfTasks; i++)
            {
                Result<AuthData> authData = _auth.GetData(tokens[i]);
                Assert.True(authData.IsSuccess, 
                    $"The guest is connect therefore the token should be valid\nError: {authData.Error}");
                usernames.Add(authData.Value.Username);
                Console.WriteLine($"Created name: {authData.Value.Username}");
            }
            
            Assert.AreEqual(numberOfTasks,
                usernames.Count,
                $"There are duplicate usernames");
        }
        
        [Test, Repeat(2)]
        public async Task ConcurrentRegisterSameUserTest()
        {
            const int numberOfTasks = 5;
            Result[] registerRes = await TaskTestUtils.CreateAndRunTasks(
                () => _auth.Register("user1", "password1"), 
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
        
        [Test]
        public async Task ConcurrentLoginFewUsersTest()
        {
            const int numberOfTasks = 5;
            string username = "user";
            string password = "password";

            for (int i = 0; i < numberOfTasks; i++)
            {
                Result registerRes = _auth.Register($"{username}{i}", $"{password}{i}");
                if (registerRes.IsFailure)
                {
                    Assert.Fail("The registration of the user didnt work");
                }
            }

            Task<Result<string>>[] loginTasks = new Task<Result<string>>[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                string uname = $"{username}{i}";
                string upassword = $"{password}{i}";
                loginTasks[i] = new Task<Result<string>>(
                    () => _auth.Login(uname, upassword, AuthUserRole.Member));
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
    }
}