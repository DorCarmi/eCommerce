﻿using System.Collections.Concurrent;
using eCommerce.Common;
using eCommerce.DataLayer;

namespace eCommerce.Business.Repositories
{
    public class PersistenceRegisteredUsersRepo : IRepository<User>
    {
        private DataFacade _dataFacade;

        public PersistenceRegisteredUsersRepo()
        {
            _dataFacade = new DataFacade();
        }
        
        public bool Add(User user)
        {
            return _dataFacade.SaveUser(user).IsSuccess;
        }

        public User GetOrNull(string username)
        {
            Result<User> userRes = _dataFacade.ReadUser(username);
            if (userRes.IsFailure)
            {
                return null;
            }

            return userRes.Value;
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