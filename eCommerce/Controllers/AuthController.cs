using System;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace eCommerce.Controllers
{

    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
    
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
            _authService = new AuthService();
        }

        [HttpGet]
        [Route("[action]")]
        public string Connect()
        {
            string token = _authService.Connect();
            Response.Cookies.Append("_auth", token, new CookieOptions()
            {
                Path = "/",
                Secure = true,
                MaxAge = TimeSpan.FromDays(5),
                Domain = Request.PathBase.Value,
                Expires = DateTimeOffset.Now.AddDays(5),
                HttpOnly = true
            });
            Response.Headers.Add("RedirectTo", "/");
            return token;
        }
        
        [HttpPost]
        [Route("[action]")]
        public Result<string> Login([FromBody] LoginInfo loginInfo)
        {
            if (Enum.TryParse<ServiceUserRole>(loginInfo.Role, true, out var serviceRole))
            {
                Result<string> loginRes = _authService.Login((string) HttpContext.Items["authToken"],
                    loginInfo.Username, loginInfo.Password, serviceRole);
                if (loginRes.IsSuccess)
                {
                    Response.Headers.Add("RedirectTo", "/");
                }
                
                return loginRes;
            }

            return Result.Fail<string>("Invalid role");
        }
        
    }
}