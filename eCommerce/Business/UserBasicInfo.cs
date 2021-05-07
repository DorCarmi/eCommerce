namespace eCommerce.Business
{
    public class UserBasicInfo
    {
        public string Username { get; set; }
        public bool IsLoggedIn { get; set; }

        public UserBasicInfo(string username, bool isLoggedIn)
        {
            Username = username;
            IsLoggedIn = isLoggedIn;
        }
    }
}