using System;
using eCommerce.Adapters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace eCommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppConfig config = AppConfig.GetInstance();
            if (config.GetData("InitWithData").Equals("True"))
            {
                InitSystem initSystem = new InitSystem();
                initSystem.Init("Init.json");
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}