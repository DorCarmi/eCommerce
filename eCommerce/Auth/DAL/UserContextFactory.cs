using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Auth.DAL
{
    public class UserContextFactory
    {
        //TODO change db string to config file

        public UserContext Create()
        {
            return new UserContext(@"Data Source=./AuthDB.db");
        }
    }
}