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
           CreateHostBuilder(args).Build().Run();
           /*var payment = new WSEPPaymentAdapter();
           var res = payment.Charge(10, "a", "a", "123",
               DateTime.Now.AddDays(1).ToLongDateString(), "123").Result;
           Console.WriteLine(res.Value);*/
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}