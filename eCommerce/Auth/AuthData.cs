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

        public bool AllDataIsNotNull()
        {
            return Username != null & Role != null;
        }
    }
}