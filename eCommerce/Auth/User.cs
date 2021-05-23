using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Common;

namespace eCommerce.Auth
{
    public class User
    {
        
        private string _username;
        private byte[] _hashedPassword;

        public User(string username, byte[] hashedPassword)
        {
            _username = username;
            _hashedPassword = hashedPassword;
        }

        // ========== Properties ========== //

        [Key]
        public string Username
        {
            get => _username;
        }

        public byte[] HashedPassword
        {
            get => _hashedPassword; 
        }
    }
}