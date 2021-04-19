using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Auth;
using Microsoft.Extensions.Logging.EventSource;

namespace eCommerce.Business.Service
{
    public class DtoUtils
    {
        public static AuthUserRole ServiceUserRoleToAuthUserRole(ServiceUserRole role)
        {
            switch (role)
            {
                case ServiceUserRole.Member:
                {
                    return AuthUserRole.Member;
                }
                case ServiceUserRole.Admin:
                {
                    return AuthUserRole.Admin;
                }
            }

            // TODO log if it gets here
            return AuthUserRole.Member;
        }
        
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

        public static BasketDto IBasketToBasketDto(IBasket basket)
        {
            return new BasketDto(
                basket.GetStoreName(),
                basket.GetAllItems().Value,
                basket.GetTotalPrice().Value
            );
        }
    }
}