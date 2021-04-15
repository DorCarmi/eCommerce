using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NUnit.Framework;

namespace Tests.Business
{
    /// <summary>
    /// Integrate between the User and Store 
    /// </summary>
    [TestFixture]
    public class MarketFacadeTests
    {
        private IMarketFacade _marketFacade;
        private const string PASSWORD = "easyPassword";

        private IList<string> _connectedGuestTokens;
        private IDictionary<string, string> _registeredAndLoggedInUsers;

        private readonly MemberInfo[] _membersInfo;

        private IList<string> _loggedInTokens;
        
        public MarketFacadeTests()
        {
            _membersInfo = CreateMembersInfo();
            _loggedInTokens = new List<string>();
        }
        
        [SetUp]
        public void SetUp()
        {
            // TODO fill it with mocks
            _marketFacade = MarketFacade.CreateInstanceForTests(
                new UserAuthMock(),
                new MemberDataRepositoryMock(),
                null);

            _loggedInTokens = new List<string>();
        }
        
        [TearDown]
        public void TearDown()
        {
            _loggedInTokens.Clear();
        }


        private MemberInfo[] CreateMembersInfo()
        {
            return new[]
            {
                new MemberInfo("user1", "email@mail.com", "TheUser", DateTime.Now, "Seller Street 1"),
                new MemberInfo("Samsung", "ceo@samsung.com", "hello", DateTime.Now, "Summer Street 121"),
            };
        }

        [Test, Order(1)]
        public void RegisterTests()
        {
            foreach (var memberInfo in _membersInfo)
            {
                Result registrationRes = _marketFacade.Register(memberInfo, PASSWORD);
                Assert.True(registrationRes.IsSuccess,
                    $"The user needed to be successfully registered\n{memberInfo}\nError: {registrationRes.Error}");
            }
        }
        
        [Test, Order(2)]
        public void LoginAndLogoutTests()
        {
            RegisterMembers();

            foreach (var member in _membersInfo)
            {
                Result<string> loginRes = _marketFacade.Login(_marketFacade.Connect(), member.Username, 
                    PASSWORD, ServiceUserRole.Member);
                Assert.True(loginRes.IsSuccess,
                    $"User {member.Username} should has been logged in");
                _loggedInTokens.Add(loginRes.Value);
            }

            foreach (var token in _loggedInTokens)
            {
                Assert.True(_marketFacade.Logout(token).IsSuccess,
                    $"The token {token} is valid and it should have logout");
                _loggedInTokens.Remove(token);
            }
        }

        private void RegisterMembers()
        {
            foreach (var memberInfo in _membersInfo)
            {
                if (_marketFacade.Register(memberInfo, "easyPassword").IsFailure)
                {
                    throw new TestCanceledException("Member didnt registered");
                }
            }
        }
    }
}