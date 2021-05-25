namespace eCommerce.Auth
{
    public interface IRegisteredUserRepo
    {
        /// <summary>
        /// Add a user to the repository if not already exists
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>True if the user have been added</returns>
        public bool Add(User user);

        /// <summary>
        /// GetDiscount the user
        /// </summary>
        /// <param name="username">The user name</param>
        /// <returns>Return a User of exists</returns>
        public User GetUserOrNull(string username);
    }
}