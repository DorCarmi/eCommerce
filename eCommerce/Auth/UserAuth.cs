using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using eCommerce.Business;
using eCommerce.Common;

namespace eCommerce.Auth
{
    //TODO make thread safe
    public class UserAuth  : IUserAuth
    {
        private static readonly UserAuth Instance = new UserAuth();

        private readonly JWTAuth _jwtAuth;

        private readonly string GUEST_ROLE_STRING = "Guest";
        // TODO make this filed in DB
        private long _guestId;
        private ConcurrentRegisteredUserRepo _userRepo;
        private readonly SHA256 _sha256;

        
        private UserAuth()
        {
            _jwtAuth = new JWTAuth("keykeykeykeykeyekeykey");
            _guestId = 0;
            _userRepo = new ConcurrentRegisteredUserRepo();
            _sha256 = SHA256.Create();
        }

        public static UserAuth GetInstance()
        {
            return Instance;
        }
        
        public Result<string> Connect()
        {
            Result<string> tokenRes = Result.Ok<string>(GenerateToken(new AuthData(GenerateGuestUsername(), GUEST_ROLE_STRING)));
            _guestId++;
            return tokenRes;
        }

        private string GenerateGuestUsername()
        {
            return string.Format("_Guest%ln", _guestId);
        }
        
        public Result Register(string username, string password)
        {
            Result policyCheck = RegistrationsPolicy(username, password);
            if (policyCheck.IsFailure)
            {
                return policyCheck;
            }

            User newUser = new User(username, HashPassword(password));
            newUser.AddRole(UserRole.Member);
            
            if (!_userRepo.Add(newUser))
            {
                return Result.Fail("Username already taken");
            }
            
            return Result.Ok();
        }

        private Result RegistrationsPolicy(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Result.Fail("Username cant be empty");
            }
            
            if (string.IsNullOrEmpty(password))
            {
                return Result.Fail("Password cant be empty");
            }
            return Result.Ok();

        }

        public Result<string> Login(string username, string password, UserRole role)
        {
            
            Result policyCheck = RegistrationsPolicy(username, password);
            if (policyCheck.IsFailure)
            {
                return Result.Fail<string>("Username or password cant be empty");
            }

            User user = _userRepo.GetUserOrNull(username);
            if (user == null || !user.HashedPassword.SequenceEqual(HashPassword(password)) || !user.HasRole(role))
            {
                return Result.Fail<string>("Invalid username or password or role");
            }

            return Result.Ok<>(GenerateToken(new AuthData(user.Username,  role.ToString())));
        }

        /// <summary>
        /// Hash the password.
        /// <para>Precondition: password is not empty.</para>
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>The password hash</returns>
        private byte[] HashPassword(string password)
        {
            return _sha256.ComputeHash(Encoding.Default.GetBytes(password));
        }
        
        // ========== Token ========== //
        
        private string GenerateToken(AuthData authData)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(AuthClaimTypes.Username, authData.Username),
                new Claim(AuthClaimTypes.Role, authData.Role)
            };

            if (authData.Role.Equals(GUEST_ROLE_STRING))
            {
                claims.Add(new Claim(AuthClaimTypes.GuestId, _guestId.ToString()));
                _guestId++;
            }

            return _jwtAuth.GenerateToken(claims.ToArray());
        }

        private AuthData GetDataIfValid(string token)
        {
            var claims = _jwtAuth.GetClaimsFromToken(token);
            if (claims == null)
            {
                return null;
            }
            
            AuthData data = new AuthData(null, null);
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case AuthClaimTypes.Username:
                    {
                        data.Username = claim.Value;
                        break;
                    }
                    case AuthClaimTypes.Role:
                    {
                        data.Role = claim.Value;
                        break;
                    }
                    default:
                        // TODO maybe log it
                        return null;
                }
            }

            if (data.AllDataIsNotNull())
            {
                return null;
            }

            return data;
        }
    }
}