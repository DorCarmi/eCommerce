using System;
using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Business;


namespace eCommerce.Service
{
    public class DtoUtils
    {
        public static UserToSystemState ServiceUserRoleToSystemState(ServiceUserRole role)
        {
            switch (role)
            {
                case ServiceUserRole.Member:
                {
                    return Member.State;
                }
                case ServiceUserRole.Admin:
                {
                    return Admin.State;
                }
            }

            // TODO log if it gets here
            throw new NotImplementedException();
        }
        
        public static ItemInfo ItemDtoToProductInfo(IItem itemDto)
        {
            List<string> keywords = new List<string>();
            IEnumerator<string> enumerator = itemDto.KeyWords.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keywords.Add(enumerator.Current);
            }

            return new ItemInfo(
                itemDto.Amount,
                itemDto.ItemName,
                itemDto.StoreName,
                itemDto.Category,
                keywords,
                (int)itemDto.PricePerUnit);
            return null;
        }
    }
}