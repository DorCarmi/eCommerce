using System;
using System.Globalization;
using System.Threading.Tasks;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using NUnit.Framework;

namespace Tests.AcceptanceTests
{
    [TestFixture]
    public class TestLogout
    {
        private IAuthService _auth;

        [SetUp]
        public void SetUp()
        {
            _auth = new AuthService();
            MemberInfo yossi = new MemberInfo("Yossi11","yossi@gmail.com", "Yossi Park", DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid","shiran@gmail.com", "Shiran Moris", DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork","lior@gmail.com", "Lior Lee", DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Register(token, lior, "987654321");
            _auth.Disconnect(token);
        }
        
        /// <summary>
        /// <UC>
        /// Logout
        /// </UC>
        /// <Req>
        /// 3.1
        /// </Req>
        /// </summary>
        [Test]
        public async Task TestSuccess()
        {
            string token = _auth.Connect();
            Result<string> result = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            result = _auth.Logout(result.Value);
            Assert.True(result.IsSuccess, result.Error);
            token = result.Value;
            result = await _auth.Login(token, "singerMermaid", "130452abc", ServiceUserRole.Member);
            result = _auth.Logout(result.Value);
            Assert.True(result.IsSuccess, result.Error);
            _auth.Disconnect(result.Value);
        }
        
        [Test]
        public async Task TestFailure()
        {
            string token = _auth.Connect();
            Result<string> result = await _auth.Login(token, "Yossi11", "qwerty123", ServiceUserRole.Member);
            Result<string> falseResult = _auth.Logout(token);
            Assert.True(falseResult.IsFailure, "Logout was suppose to fail");
            _auth.Disconnect(result.Value);
        }
    }
}