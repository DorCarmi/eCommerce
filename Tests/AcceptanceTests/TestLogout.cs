using System;
using System.Globalization;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;
using Tests.AuthTests;

namespace Tests.AcceptanceTests
{
    /// <summary>
    /// <UC>
    /// Logout
    /// </UC>
    /// <Req>
    /// 3.1
    /// </Req>
    /// </summary>
    
    [TestFixture]
    public class TestLogout
    {
        private IAuthService _auth;

        [SetUpAttribute]
        public void SetUp()
        {
            StoreRepository SR = new StoreRepository();
            TRegisteredUserRepo RP = new TRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Yossi250","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("happyFrog","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Disconnect(token);
        }

        [TearDownAttribute]
        public void TearDown()
        {
            _auth = null;
        }
        
        [Test]
        [Order(12)]
        public void TestLogoutSuccess()
        {
            string token = _auth.Connect();
            Result<string> result = _auth.Login(token, "Yossi250", "qwerty123", ServiceUserRole.Member);
            result = _auth.Logout(result.Value);
            Assert.True(result.IsSuccess, result.Error);
            token = result.Value;
            result = _auth.Login(token, "happyFrog", "130452abc", ServiceUserRole.Member);
            result = _auth.Logout(result.Value);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Disconnect(result.Value);
        }

        [Test]
        public void TestLogoutFailure()
        {
            string token = _auth.Connect();
            Result<string> result = _auth.Login(token, "Yossi250", "qwerty123", ServiceUserRole.Member);
            Result<string> falseResult = _auth.Logout(token);
            Assert.True(falseResult.IsFailure, "Logout was suppose to fail");
            _auth.Disconnect(result.Value);
        }
    }
}