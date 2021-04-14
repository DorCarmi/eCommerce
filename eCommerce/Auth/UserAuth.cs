using System;
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
        private readonly SHA256 _sha256;

        private const string GUEST_ROLE_STRING = "Guest";
        private ConcurrentIdGenerator _concurrentIdGenerator;
        
        private IRegisteredUserRepo _userRepo;
        private IDictionary<string, string> _connectedGuests;
        private IDictionary<string, string> _connectedUsers;
        
        private UserAuth(IRegisteredUserRepo repo)
        {
            _jwtAuth = new JWTAuth("keykeykeykeykeyekeykey");
            // TODO get the initialze id value from DB
            _concurrentIdGenerator = new ConcurrentIdGenerator(0);
            _sha256 = SHA256.Create();
            _userRepo = repo;
            _connectedGuests = new ConcurrentDictionary<string, string>();
            _connectedUsers = new ConcurrentDictionary<string, string>();
            InitOneAdmin();
        }

        private void InitOneAdmin()
        {
            Register("Admin", "Admin");
            User adminUser = _userRepo.GetUserOrNull("Admin");
            if (adminUser == null)
            {
                throw new Exception("There isnt at least one admin in the system");
            }

            adminUser.AddRole(AuthUserRole.Admin);
        }

        public static UserAuth GetInstance()
        {
            return Instance;
        }
        
        public static UserAuth CreateInstanceForTests(IRegisteredUserRepo userRepo)
        {
            return new UserAuth(userRepo);
        }
        
        public string Connect()
        {
            string guestUsername = GenerateGuestUsername();
            string token = GenerateToken(new AuthData(guestUsername, GUEST_ROLE_STRING));
            _connectedGuests.Add(token, guestUsername);
            return token;
        }

        public void Disconnect(string token)
        {
            _connectedGuests.Remove(token);
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
            newUser.AddRole(AuthUserRole.Member);

            _userRepo.Add(newUser);
            return Result.Ok();
        }

        public Result<string> Login(string guestToken, string username, string password, AuthUserRole role)
        {

            Result discountGuestRes = DiscountGuestIfConnected(guestToken);
            if (discountGuestRes.IsFailure)
            {
                return Result.Fail<string>(discountGuestRes.Error);
            }

            User user = _userRepo.GetUserOrNull(username);
            Result canLogInRes = CanLogIn(user, password, role);
            if (canLogInRes.IsFailure)
            {
                return Result.Fail<string>(canLogInRes.Error);
            }

            string token = GenerateToken(new AuthData(user.Username, role.ToString()));
            _connectedUsers.Add(username, token);

            return Result.Ok(token);
        }

        public Result TryLogin(string guestToken, string username, string password, AuthUserRole role)
        {
            if (!IsGuestConnected(guestToken))
            {
                Result.Fail("Guest is not connected");
            }
            
            User user = _userRepo.GetUserOrNull(username);
            Result canLogInRes = CanLogIn(user, password, role);
            if (canLogInRes.IsFailure)
            {
                return canLogInRes;
            }

            return Result.Ok();
        }

        public Result<string> Logout(string token)
        {
            Result<AuthData> authData = GetData(token);
            if (authData.IsFailure)
            {
                return Result.Fail<string>("The user need to be logged in");
            }
            _connectedUsers.Remove(authData.Value.Username);
            return Result.Ok(Connect());
        }

        public bool IsRegistered(string username)
        {
            return _userRepo.GetUserOrNull(username) != null;
        }
        
        public bool IsLoggedIn(string username)
        {
            return _connectedUsers.ContainsKey(username);
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
            
            AuthData data = new AuthData(null, null);
            if (!FillAuthData(claims, data) || !data.AllDataIsNotNull())
            {
                return Result.Fail<AuthData>("Token have missing or redundant data");
            }

            return Result.Ok(data);
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
                    case AuthClaimTypes.Role:
                    {
                        data.Role = claim.Value;
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
            return _sha256.ComputeHash(Encoding.Default.GetBytes(password));
        }
        
        private string GenerateToken(AuthData authData)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(AuthClaimTypes.Username, authData.Username),
                new Claim(AuthClaimTypes.Role, authData.Role)
            };

            /*if (authData.Role.Equals(GUEST_ROLE_STRING))
            {
                claims.Add(new Claim(AuthClaimTypes.GuestId, _guestId.ToString()));
                IncrementGuestId();
            }*/

            return _jwtAuth.GenerateToken(claims.ToArray());
        }

        private long GetAndIncrementGuestId()
        {
            return _concurrentIdGenerator.MoveNext();
        }
        
        private string GenerateGuestUsername()
        {
            return $"_{GUEST_ROLE_STRING}{GetAndIncrementGuestId():D}";
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

        private bool IsGuestConnected(string token)
        {
            return _connectedGuests.ContainsKey(token);
        }
        
        private string GetConnectedGuestUsername(string token)
        {
            string username = null;
            _connectedGuests.TryGetValue(token, out username);
            return username;
        }

        private Result DiscountGuestIfConnected(string guestToken)
        {
            if (!IsGuestConnected(guestToken))
            {
                return Result.Fail<string>("Not connected as guest");
            }

            Disconnect(guestToken);
            return Result.Ok();
        }

        private Result CanLogIn(User user, string password, AuthUserRole role)
        {
            if (user == null || !user.HashedPassword.SequenceEqual(HashPassword(password)) || !user.HasRole(role))
            {
                return Result.Fail("Invalid username or password or role");
            }

            if (IsLoggedIn(user.Username))
            {
                return Result.Fail("User already connected");
            }

            return Result.Ok();
        }
    }
}