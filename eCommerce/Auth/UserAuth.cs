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
    public record Token(string JwtToken);
    
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
        
        public Result<Token> Connect()
        {
            throw new NotImplementedException();
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

        public Result<Token> Login(string username, string password, UserRole role)
        {
            
            Result policyCheck = RegistrationsPolicy(username, password);
            if (policyCheck.IsFailure)
            {
                return Result.Fail<Token>("Username or password cant be empty");
            }

            User user = _userRepo.GetUserOrNull(username);
            if (user == null || !user.HashedPassword.SequenceEqual(HashPassword(password)) || !user.HasRole(role))
            {
                return Result.Fail<Token>("Invalid username or password or role");
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
        
        private Token GenerateToken(AuthData authData)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, authData.Username),
                new Claim(ClaimTypes.Role, authData.Role)
            };

            if (authData.Role.Equals(GUEST_ROLE_STRING))
            {
                claims.Add(new Claim(ClaimTypes.UserData, _guestId.ToString()));
                _guestId++;
            }
            
            return new Token(_jwtAuth.GenerateToken(claims.ToArray()));
        }

        private AuthData GetDataIfValid(Token token)
        {
            var claims = _jwtAuth.GetClaimsFromToken(token.JwtToken);
            if (claims == null)
            {
                return null;
            }
            
            AuthData data = new AuthData(null, null);
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.Name:
                    {
                        data.Username = claim.Value;
                        break;
                    }
                    case ClaimTypes.Role:
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