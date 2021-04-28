using System;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace eCommerce.Controllers
{
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
            return _authService.Connect();
            /*Response.Cookies.Append("_auth", token, new CookieOptions()
            {
                Path = "/",
                Secure = true,
                MaxAge = TimeSpan.FromDays(5),
                Domain = Request.Path.Value,
                Expires = DateTimeOffset.Now.AddDays(5)
            });
            Response.Redirect("/");*/
        }
        
    }
}