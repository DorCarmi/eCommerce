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
    
    public record Token(string JwtToken);
    
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

        public Token GenerateToken(AuthData authData)
        {

            return new Token(
                _jwtAuth.GenerateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Name, authData.Username)
                })
            );
        }

        public AuthData GetDataIfValid(Token token)
        {
            var claims = _jwtAuth.GetClaimsFromToken(token.JwtToken);
            
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