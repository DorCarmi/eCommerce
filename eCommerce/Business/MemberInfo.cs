using System;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class MemberInfo : ICloneable<MemberInfo>
    {
        public string Id { get; set; }
        public string Username { get; }
        public string Email { get; }
        public string Name { get; }
        public DateTime Birthday { get; }
        public string Address { get; }

        public MemberInfo(string username, string email, string name, DateTime birthday, string address)
        {
            // TODO: may change id to real id
            Id = username;
            Username = username;
            Email = email;
            Name = name;
            Birthday = birthday;
            Address = address;
        }

        public Result IsBasicDataFull()
        {
            IList<string> emptyOrNullFields = new List<string>();
            if (string.IsNullOrEmpty(Username))
            {
                emptyOrNullFields.Add("Username");
            }

            if (string.IsNullOrEmpty(Email))
            {
                emptyOrNullFields.Add("Email");
            }

            if (string.IsNullOrEmpty(Name))
            {
                emptyOrNullFields.Add("Name");
            }

            if (string.IsNullOrEmpty(Address))
            {
                emptyOrNullFields.Add("Address");
            }

            if (emptyOrNullFields.Count > 0)
            {
                return Result.Fail($"This fields are empty or null: {emptyOrNullFields.ToString()}");
            }

            return Result.Ok();
        }

        public MemberInfo Clone()
        {
            return (MemberInfo) this.MemberwiseClone();
        }
    }
}