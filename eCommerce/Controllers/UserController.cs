using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{
    
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
            _userService = new UserService();
        }

        [HttpGet]
        [Route("[action]")]
        public Result<List<string>> GetALlStoreIds()
        {
            return _userService.GetAllStoreIds((string) HttpContext.Items["authToken"]);
        }
        
        [HttpGet]
        [Route("[action]")]
        public Result<UserBasicInfo> GetUserBasicInfo()
        {
            return _userService.GetUserBasicInfo((string) HttpContext.Items["authToken"]);
        }
    }
}