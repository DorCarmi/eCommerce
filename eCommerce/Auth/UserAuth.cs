using System;
using System.Collections.Generic;
using System.Security.Claims;
using eCommerce.Business;
using eCommerce.Common;

namespace eCommerce.Auth
{

    public class AuthData
    {
        public string Username { get; set; }
        public string Role { get; set; }
        
        public AuthData(string username, string role)
        {
            Username = username;
            Role = role;
        }

        public bool IsFull()
        {
            return Username != null & Role != null;
        }
    }

    public record Token(string JwtToken);
    
    //TODO make thread safe
    public class UserAuth  : IUserAuth
    {
        private static readonly UserAuth Instance = new UserAuth();

        private readonly JWTAuth _jwtAuth;

        // TODO make this filed in DB
        private long _guestId;
        private ConcurrentRegisteredUserRepo _userRepo;

        
        private UserAuth()
        {
            _jwtAuth = new JWTAuth("keykeykeykeykeyekeykey");
            _guestId = 0;
            _userRepo = new ConcurrentRegisteredUserRepo();
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

            if (!_userRepo.Add(username, password))
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

        public Result<Token> Login(string username, string password)
        {
            throw new NotImplementedException();
        }
        
        // ========== Token ========== //
        
        private Token GenerateToken(AuthData authData)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, authData.Username),
                new Claim(ClaimTypes.Role, authData.Role)
            };

            if (authData.Role == "Guest")
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

            if (data.IsFull())
            {
                return null;
            }

            return data;
        }
    }
}