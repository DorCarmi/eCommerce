﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;
using NLog;

namespace eCommerce.Business
{
    public class UserManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private IUserAuth _auth;
        
        // token to user
        private ConcurrentDictionary<string, IUser> _connectedUsers;
        private IRepository<IUser> _registeredUsersRepo;
        private ConcurrentDictionary<string, IUser> _admins;

        private ConcurrentIdGenerator _concurrentIdGenerator;


        public UserManager(IUserAuth auth, IRepository<IUser> registeredUsersRepo)
        {
            _auth = auth;
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
            // TODO get the initialze id value from DB
            _concurrentIdGenerator = new ConcurrentIdGenerator(0);
            _registeredUsersRepo = registeredUsersRepo;
            _admins = new ConcurrentDictionary<string, IUser>();
        }

        public string Connect()
        {
            string guestUsername = GenerateGuestUsername();
            string token = _auth.GenerateToken(guestUsername);

            IUser newUser = CreateGuestUser(guestUsername);
            _connectedUsers.TryAdd(token, newUser);
            
            _logger.Info($"New guest: {guestUsername}");
            return token;
        }
        
        public void Disconnect(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                _connectedUsers.TryRemove(token, out var tuser);
                return;
            }

            if (_connectedUsers.TryGetValue(token, out var user)
                && user.GetState() == Guest.State)
            {
                _connectedUsers.TryRemove(token, out user);
            }
        }

        public Result Register(string token, MemberInfo memberInfo, string password)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                _connectedUsers.TryRemove(token, out var tuser);
                return Result.Fail("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                return Result.Fail("User need to be connected or logged in");
            }
            
            Result validMemberInfoRes = IsValidMemberInfo(memberInfo);
            if (validMemberInfoRes.IsFailure)
            {
                return validMemberInfoRes;
            }

            Result authRegistrationRes = RegisterAtAuthorization(memberInfo.Username, password);
            if (authRegistrationRes.IsFailure)
            {
                return authRegistrationRes;
            }
            
            IUser newUser = new User(Member.State, memberInfo.Clone());
            if (!_registeredUsersRepo.Add(newUser))
            {
                // TODO maybe remove the user form userAuth
                _logger.Error($"User {memberInfo.Username} was able to register at Auth but already exists in " +
                    "the registered user repository");
                return Result.Fail("User already exists");
            }

            _logger.Info($"User {memberInfo.Username} was registered");
            return Result.Ok();
        }

        public void AddAdmin(MemberInfo adminInfo, string password)
        {
            IUser user = new User(Admin.State, adminInfo);
            RegisterAtAuthorization(adminInfo.Username, password);
            _registeredUsersRepo.Add(user);
            _admins.TryAdd(adminInfo.Username, user);
        }
        
        private Result RegisterAtAuthorization(string username, string password)
        {
            return _auth.Register(username, password);
        }
        
        // TODO: use the role here, how user that is admin can log in as member
        public Result<string> Login(string guestToken, string username, string password, UserToSystemState role)
        {
            if (!_auth.IsValidToken(guestToken))
            {
                _logger.Warn($"Invalid token {guestToken}");
                _connectedUsers.TryRemove(guestToken, out var tUser);
                return Result.Fail<string>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(guestToken, out var guestUser) || guestUser.GetState() != Guest.State)
            {
                return Result.Fail<string>("Not connected or not guest");
            }
            
            Result authLoginRes = _auth.Authenticate(username, password);
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }
            string loginToken = _auth.GenerateToken(username);
            
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                return Result.Fail<string>("Invalid username or password");
            }

            if (!_connectedUsers.TryRemove(guestToken, out var tUser1))
            {
                return Result.Fail<string>("Guest not connected");
            }
            
            if (!_connectedUsers.TryAdd(loginToken, user))
            {
                _logger.Error($"UserAuth created duplicate toekn(already in connected userses dictionry)");
                return Result.Fail<string>("Error");
            }
            
            return Result.Ok(loginToken);
        }
        
        public Result<string> Logout(string token)
        {

            if (!_auth.IsValidToken(token))
            {
                _logger.Warn($"Invalid token {token}");
                _connectedUsers.TryRemove(token, out var tUser);
                return Result.Fail<string>("Invalid token");
            }
            
            if (!(_connectedUsers.TryGetValue(token, out var user) && user.GetState() != Guest.State))
            {
                return Result.Fail<string>("Guest cant logout");
            }

            if (!_connectedUsers.TryRemove(token, out var tUser1))
            {
                _logger.Error($"User logout error");
            }
            else
            {
                _logger.Info($"User {tUser1.Username} logout");
            }
            
            return Result.Ok(Connect());
        }

        public Result<IUser> GetUserIfConnectedOrLoggedIn(string token)
        {
            if (!_auth.IsValidToken(token))
            {
                _logger.Info($"Invalid use of token {token}");
                _connectedUsers.TryRemove(token, out var tUser);
                return Result.Fail<IUser>("Invalid token");
            }

            if (!_connectedUsers.TryGetValue(token, out var user))
            {
                _logger.Info($"Usage of old token {token}");
                return Result.Fail<IUser>("User not connected or logged in");
            }

            return Result.Ok(user);
        }
        
        /// <summary>
        /// Get the user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The user</returns>
        public Result<IUser> GetUser(string username)
        {
            IUser user = _registeredUsersRepo.GetOrNull(username);
            if (user == null)
            {
                return Result.Fail<IUser>("User doesn't exists");
            }

            return Result.Ok(user);
        }

        private IUser CreateGuestUser(string guestName)
        {
            return new User(guestName);
        }
        
        /// <summary>
        /// Check if the member information is valid
        /// </summary>
        /// <returns>Result of the check</returns>
        private Result IsValidMemberInfo(MemberInfo memberInfo)
        {
            Result fullDataRes = memberInfo.IsBasicDataFull();
            if (fullDataRes.IsFailure)
            {
                return fullDataRes;
            }

            if (!IsValidUsername(memberInfo.Username))
            {
                return Result.Fail("Invalid username address");
            }

            if (!RegexUtils.IsValidEmail(memberInfo.Email))
            {
                return Result.Fail("Invalid email address");
            }

            return Result.Ok();
        }

        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username,
            "^[a-zA-z][a-zA-z0-9]*$");
        }

        private long GetAndIncrementGuestId()
        {
            return _concurrentIdGenerator.MoveNext();
        }
        
        private string GenerateGuestUsername()
        {
            return $"_Guest{GetAndIncrementGuestId():D}";
        }
    }
}