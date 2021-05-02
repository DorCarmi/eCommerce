using System;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{





    [Route("[controller]")]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
            _cartService = new CartService();
        }


        // the route is /Store/OpenStore  
        //removes "Controller" from the class name and add the name of the function as an endpoint 

        [HttpGet]
        [Route("[action]")]
        public Result GetCart()
        {
            return _cartService.GetCart((string) HttpContext.Items["authToken"]);
        }
        
        // [HttpPost]
        // [Route("[action]")]
        // public Result Register([FromBody] MemberInfo memberInfo)
        // {
        //     Result registerRes = _authService.Register((string) HttpContext.Items["authToken"],
        //         memberInfo, memberInfo.Password);
        //     if (registerRes.IsSuccess)
        //     {
        //         Response.Headers.Add("RedirectTo", "/");
        //     }
        //     /* if (Enum.TryParse<ServiceUserRole>(memberInfo.D, true, out var serviceRole))
        //      {
        //          Result<string> loginRes = _authService.Login((string) HttpContext.Items["authToken"],
        //              loginInfo.Username, loginInfo.Password, serviceRole);
        //          if (loginRes.IsSuccess)
        //          {
        //              Response.Headers.Add("RedirectTo", "/");
        //          }
        //          
        //          return loginRes;
        //      }*/
        //
        //     return registerRes;
        // }
    }
}