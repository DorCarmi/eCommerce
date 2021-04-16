﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class RegisteredUsersRepository : IRepository<IUser>
    {

        private ConcurrentDictionary<string, IUser> _users;

        public RegisteredUsersRepository()
        {
            _users = new ConcurrentDictionary<string, IUser>();
        }
        
        public bool Add(IUser user)
        {
            return _users.TryAdd(user.Username, user);
        }

        public IUser GetOrNull(string id)
        {
            if (!_users.TryGetValue(id, out var data))
            {
                return null;
            }

            return data;
        }

        
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Remove(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}