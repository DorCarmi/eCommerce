﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Update management permission for sub-manager
    /// Remove management permission for sub-manager
    /// </UC>
    /// <Req>
    /// 4.6
    /// </Req>
    /// </summary>
    //[TestFixture]
    //[Order(7)]
    public class TestChangeManagerPermissions
    {
        private IAuthService _auth;
        private IStoreService _store;
        private IUserService _user;
        private string store = "Yossi's Store7";
        
        
        [SetUpAttribute]
        public void SetUp()
        {
            StoreRepository SR = new StoreRepository();
            TRegisteredUserRepo RP = new TRegisteredUserRepo();
            UserAuth UA = UserAuth.CreateInstanceForTests(RP);
            IRepository<IUser> UR = new RegisteredUsersRepository();

            _auth = AuthService.CreateUserServiceForTests(UA, UR, SR);
            _store = StoreService.CreateUserServiceForTests(UA, UR, SR);
            _user = UserService.CreateUserServiceForTests(UA, UR, SR);
            MemberInfo yossi = new MemberInfo("Yossi117", "yossi@gmail.com", "Yossi Park",
                DateTime.ParseExact("19/04/2005", "dd/MM/yyyy", CultureInfo.InvariantCulture), "hazait 14");
            MemberInfo shiran = new MemberInfo("singerMermaid7", "shiran@gmail.com", "Shiran Moris",
                DateTime.ParseExact("25/06/2008", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Rabin 14");
            MemberInfo lior = new MemberInfo("Liorwork347","lior@gmail.com", "Lior Lee", 
                DateTime.ParseExact("05/07/1996", "dd/MM/yyyy", CultureInfo.InvariantCulture), "Carl Neter 14");
            string token = _auth.Connect();
            _auth.Register(token, yossi, "qwerty123");
            _auth.Register(token, shiran, "130452abc");
            _auth.Register(token, lior, "987654321");
            Result<string> yossiLogInResult = _auth.Login(token, "Yossi117", "qwerty123", ServiceUserRole.Member);
            _store.OpenStore(yossiLogInResult.Value, store);
            _user.AppointManager(yossiLogInResult.Value, store, "singerMermaid7");
            token = _auth.Logout(yossiLogInResult.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TearDownAttribute]
        public void Teardown()
        {
            _auth = null;
            _store = null;
            _user = null;
        }

        [TestCase("singerMermaid7")]
        [Order(0)]
        [Test]
        public void TestSuccessAdd(string manager)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi117", "qwerty123", ServiceUserRole.Member);
            StorePermission[] perms = new[]
                {StorePermission.ChangeItemStrategy, StorePermission.AddItemToStore, StorePermission.ChangeItemPrice};
            List<StorePermission> permissions = new List<StorePermission>(perms);
            Result result = _user.UpdateManagerPermission(yossiLogin.Value, store, manager, permissions);
            Assert.True(result.IsSuccess, "failed to update " + manager + "'s permissions: " + result.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("singerMermaid7")]
        [Test]
        public void TestSuccessRemove(string manager)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi117", "qwerty123", ServiceUserRole.Member);
            StorePermission[] oldPerms = new[]
                {StorePermission.ChangeItemStrategy, StorePermission.AddItemToStore, StorePermission.ChangeItemPrice};
            List<StorePermission> permissions = new List<StorePermission>(oldPerms);
            List<StorePermission> newPermissions =
                new List<StorePermission>(new StorePermission[] {StorePermission.ChangeItemPrice});
            _user.UpdateManagerPermission(yossiLogin.Value, store, manager, permissions);
            Result result = _user.UpdateManagerPermission(yossiLogin.Value, store, manager, newPermissions);
            Assert.True(result.IsSuccess, "failed to update " + manager + "'s permissions: " + result.Error);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("Liorwork347")]
        [Test]
        public void TestFailureInvalid(string manager)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi117", "qwerty123", ServiceUserRole.Member);
            StorePermission[] perms = new[]
                {StorePermission.ChangeItemStrategy, StorePermission.AddItemToStore, StorePermission.ChangeItemPrice};
            List<StorePermission> permissions = new List<StorePermission>(perms);
            Result result = _user.UpdateManagerPermission(yossiLogin.Value, store, manager, permissions);
            Assert.True(result.IsFailure, "was suppose to fail, user is not a manager");
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
        [TestCase("singerMermaid7", "prancing dragon")]
        [TestCase("Tamir123", "prancing dragon")]
        [Test]
        public void TestFailurelogic(string manager, string storeName)
        {
            string token = _auth.Connect();
            Result<string> yossiLogin = _auth.Login(token, "Yossi117", "qwerty123", ServiceUserRole.Member);
            StorePermission[] perms = new[]
                {StorePermission.ChangeItemStrategy, StorePermission.AddItemToStore, StorePermission.ChangeItemPrice};
            List<StorePermission> permissions = new List<StorePermission>(perms);
            Result result = _user.UpdateManagerPermission(yossiLogin.Value, storeName, manager, permissions);
            Assert.True(result.IsFailure);
            token = _auth.Logout(yossiLogin.Value).Value;
            _auth.Disconnect(token);
        }
        
    }
}