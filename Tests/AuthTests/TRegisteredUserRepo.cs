﻿using System.Collections.Concurrent;
using eCommerce.Auth;

namespace Tests.Auth
{
    public class TRegisteredUserRepo : IRegisteredUserRepo
    {
        
        private ConcurrentDictionary<string, User> _dictionary;

        public TRegisteredUserRepo()
        {
            _dictionary = new ConcurrentDictionary<string, User>();
        }
        
        public bool Add(User user)
        {
            return _dictionary.TryAdd(user.Username, user);
        }
        
        public User GetUserOrNull(string username)
        {
            User user;
            if (!_dictionary.TryGetValue(username, out user))
            {
                user = null;
            }
            return user;
        }
    }
}