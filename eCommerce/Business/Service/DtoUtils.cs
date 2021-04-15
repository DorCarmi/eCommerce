using System;
using System.Linq;
using eCommerce.Auth;

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
            // TODO implement
            switch (role)
            {
                case ServiceUserRole.Member:
                {
                    throw new NotImplementedException();
                }
                case ServiceUserRole.Admin:
                {
                    throw new NotImplementedException();
                }
            }

            // TODO log if it gets here
            throw new NotImplementedException();
        }
        
        public static ItemInfo ProductDtoToProductInfo(ProductDto productDto)
        {
            return new ItemInfo(
                productDto.Amount,
                productDto.ProductName,
                productDto.StoreName,
                productDto.Categories.FirstOrDefault(),
                productDto.KeyWords);
        }
    }
}