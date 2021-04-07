using System;
using System.Security.Claims;

namespace eCommerce.Auth
{

    public class AuthData
    {
        public string Username { get; }

        public AuthData(string username)
        {
            Username = username;
        }
    }
    
    //TODO make thread safe
    public class ServerAuth
    {
        private static readonly ServerAuth Instance = new ServerAuth();

        private readonly JWTAuth _jwtAuth;

        private ServerAuth()
        {
            _jwtAuth = new JWTAuth("keykeykeykeykeyekeykey");
        }

        public static ServerAuth GetInstance()
        {
            return Instance;
        }

        public string GenerateToken(AuthData authData)
        {

            return _jwtAuth.GenerateToken(new Claim[]
            {
                new Claim(ClaimTypes.Name, authData.Username)
            });
        }

        public AuthData GetDataIfValid(string token)
        {
            var claims = _jwtAuth.GetClaimsFromToken(token);
            
            if (claims == null)
            {
                return null;
            }
            
            string username = null;
            foreach (var claim in claims)
            {
                Console.WriteLine(claim.Type);
                if (claim.Type == ClaimTypes.Name)
                {
                    username = claim.Value;
                    break;
                }
            }

            if (username == null)
            {
                return null;
            }

            return new AuthData(username);
        }
    }
}