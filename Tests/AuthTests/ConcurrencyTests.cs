using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Auth;
using eCommerce.Business;
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
            string[] tokens = await CreateAndRunTasks(
                () => _auth.Connect(),
                numberOfTasks);
            
            ISet<string> usernames = new HashSet<string>();
            
            for (var i = 0; i < numberOfTasks; i++)
            {
                Result<AuthData> authData = _auth.GetDataIfConnected(tokens[i]);
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
            Result[] registerRes = await CreateAndRunTasks(
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

        private Task<T[]> CreateAndRunTasks<T>(Func<T> func, int numberOfTasks)
        {
            Task<T>[] tasks = CreateArrayOfTasks(func, numberOfTasks);
            RunTasks(tasks);

            return Task.WhenAll<T>(tasks);
        }

        private Task<T>[] CreateArrayOfTasks<T>(Func<T> func, int numberOfTasks)
        {
            Task<T>[] tasks = new Task<T>[numberOfTasks];
            for (var i = 0; i < numberOfTasks; i++)
            {
                tasks[i] = new Task<T>(func);
            }

            return tasks;
        }
        
        private void RunTasks<T>(Task<T>[] tasks)
        {
            foreach (var task in tasks)
            {
                task.Start();
            }
        }
    }
}