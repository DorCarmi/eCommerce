namespace Tests.Auth
{
    public class TestsUserData
    {
        public string Username { get; }
        public string Password { get; }

        public TestsUserData(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}