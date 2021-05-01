using System;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{





    [Route("[controller]")]
    public class StoreController : Controller
    {
        private readonly ILogger<StoreController> _logger;
        private readonly IStoreService _storeService;

        public StoreController(ILogger<StoreController> logger)
        {
            _logger = logger;
            _storeService = new StoreService();
        }


        // the route is /Store/OpenStore  
        //removes "Controller" from the class name and add the name of the function as an endpoint 

        [HttpPost]
        [Route("[action]")]
        public Result OpenStore([FromBody] SItem itemInfo)
        {
            return _storeService.OpenStore((string) HttpContext.Items["authToken"],
                itemInfo.StoreName, itemInfo);
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