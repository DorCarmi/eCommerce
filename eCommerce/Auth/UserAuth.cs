﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using eCommerce.Common;

namespace eCommerce.Auth
{
    public class UserAuth  : IUserAuth
    {
        private static readonly UserAuth Instance = new UserAuth(new ConcurrentRegisteredUserRepo());

        private readonly JWTAuth _jwtAuth;

        private Mutex _hashMutex;
        private readonly SHA256 _sha256;
        
        private IRegisteredUserRepo _userRepo;

        private UserAuth(IRegisteredUserRepo repo)
        {
            _jwtAuth = new JWTAuth("keykeykeykeykeyekeykey");

            _hashMutex = new Mutex();
            _sha256 = SHA256.Create();
            
            _userRepo = repo;
        }

        public static UserAuth GetInstance()
        {
            return Instance;
        }
        
        public static UserAuth CreateInstanceForTests(IRegisteredUserRepo userRepo)
        {
            return new UserAuth(userRepo);
        }

        public Result Register(string username, string password)
        {
            Result policyCheck = RegistrationsPolicy(username, password);
            if (policyCheck.IsFailure)
            {
                return policyCheck;
            }

            if (IsRegistered(username))
            {
                return Result.Fail("Username already taken");
            }
            
            User newUser = new User(username, HashPassword(password));

            if (!_userRepo.Add(newUser))
            {
                return Result.Fail("Username already taken");
            }
            return Result.Ok();
        }

        public Result Authenticate(string username, string password)
        {
            User user = _userRepo.GetUserOrNull(username);
            Result canLogInRes = CanLogIn(user, password);
            if (canLogInRes.IsFailure)
            {
                return Result.Fail(canLogInRes.Error);
            }

            return Result.Ok();
        }

        public string GenerateToken(string username)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(AuthClaimTypes.Username, username)
            };
            
            return _jwtAuth.GenerateToken(claims);
        }

        public bool IsRegistered(string username)
        {
            return _userRepo.GetUserOrNull(username) != null;
        }

        public bool IsValidToken(string token)
        {
            return GetData(token).IsSuccess;
        }
        
        public Result<AuthData> GetData(string token)
        {
            var claims = _jwtAuth.GetClaimsFromToken(token);
            if (claims == null)
            {
                return Result.Fail<AuthData>("Token is not valid");
            }
            
            AuthData data = new AuthData(null);
            if (!FillAuthData(claims, data) || !data.AllDataIsNotNull())
            {
                return Result.Fail<AuthData>("Token have missing or redundant data");
            }

            return Result.Ok(data);
        }

        public string RoleToString(AuthUserRole role)
        {
            return role.ToString();
        }

        // ========== Private methods ========== //
        
        
        /// <summary>
        /// Fill the AuthData from claims
        /// </summary>
        /// <param name="claims">The claims</param>
        /// <param name="data">The auth data to fill</param>
        /// <returns>True if all the claims are valid</returns>
        private bool FillAuthData(IEnumerable<Claim> claims, AuthData data)
        {
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case AuthClaimTypes.Username:
                    {
                        data.Username = claim.Value;
                        break;
                    }
                    default:
                        // TODO check the other expected types
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Hash the password.
        /// <para>Precondition: password is not empty.</para>
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>The password hash</returns>
        private byte[] HashPassword(string password)
        {
            byte[] hashedPassword = null;
            _hashMutex.WaitOne();
            hashedPassword = _sha256.ComputeHash(Encoding.Default.GetBytes(password));
            _hashMutex.ReleaseMutex();
            return hashedPassword;
        }

        private Result RegistrationsPolicy(string username, string password)
        {
            String errMessage = null;
            if (string.IsNullOrEmpty(username))
            {
                errMessage = $"{errMessage}\nUsername cant be empty";
            }
            else if(username.StartsWith("_Guest"))
            {
                errMessage = $"{errMessage}\nUsername not valid";
            }

            if (string.IsNullOrEmpty(password))
            {
                errMessage = $"{errMessage}\nPassword not valid";
            }

            return errMessage == null ? Result.Ok() : Result.Fail(errMessage);
        }

        private Result CanLogIn(User user, string password)
        {
            if (user == null || !user.HashedPassword.SequenceEqual(HashPassword(password)))
            {
                return Result.Fail("Invalid username or password or role");
            }

            return Result.Ok();
        }
    }
}