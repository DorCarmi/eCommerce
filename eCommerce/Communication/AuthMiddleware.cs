using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace eCommerce.Communication
{
    public class AuthMiddleware
    {
        private const string AUTH_COOKIE = "_auth";
        
        private readonly RequestDelegate _next;
        private readonly IDictionary<string, bool> _controllersNames;
        
        
        public AuthMiddleware(RequestDelegate next,
            IActionDescriptorCollectionProvider provider)
        {
            _next = next;
            _controllersNames = new Dictionary<string, bool>();
            
             var controllers = provider.ActionDescriptors.Items.Where(
                 ad => ad.AttributeRouteInfo != null).Select(
                 ad => ad.AttributeRouteInfo.Template
             ).ToList();
             
            Console.WriteLine("Controllers:");
            foreach (var controller in controllers)
            {
                if (controller != null)
                {
                    Console.WriteLine(controller);
                    _controllersNames.Add(controller, true);
                }
            }
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var authCookie = context.Request.Cookies[AUTH_COOKIE];
            var path = context.Request.Path.Value;
            
            if (authCookie == null && path != null )//_controllersNames.ContainsKey(path))
            {
                if (!(path.Equals("/login") || path.StartsWith("/auth", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/static/")))
                {
                    Console.WriteLine($"Redirect {path}");
                    context.Response.StatusCode = 302;
                    context.Response.Redirect("/login");
                    return;
                }
            }

            context.Items["authToken"] = authCookie;
            Console.WriteLine($"Don't redirect {path}");
            await _next(context);

        }
    }
}