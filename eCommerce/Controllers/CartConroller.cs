using System;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    public class AddOrEditItemData
    {
        public string StoreId { get; set; }
        public string ItemId { get; set; }
        public int Amount { get; set; }
    }



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

        [HttpPost]
        [Route("[action]")]
        public Result AddItem([FromBody] AddOrEditItemData addItemData)
        {
            return _cartService.AddItemToCart((string) HttpContext.Items["authToken"],
                addItemData.ItemId, addItemData.StoreId, addItemData.Amount);
        }
        
        [HttpPost]
        [Route("[action]")]
        public Result EditItemAmount([FromBody] AddOrEditItemData editItemData)
        {
            return _cartService.EditItemAmountOfCart((string) HttpContext.Items["authToken"],
                editItemData.ItemId, editItemData.StoreId, editItemData.Amount);
        }
        
        [HttpGet]
        [Route("[action]")]
        public Result<SCart> GetCart()
        {
            return _cartService.GetCart((string) HttpContext.Items["authToken"]);
        }
        
        [HttpGet]
        [Route("[action]")]
        public Result<double> GetPurchasePrice()
        {
            return _cartService.GetPurchaseCartPrice((string) HttpContext.Items["authToken"]);
        }
        
        [HttpPost]
        [Route("[action]")]
        public Result PurchaseCart([FromBody] PaymentInfo paymentInfo)
        {
            return _cartService.PurchaseCart((string) HttpContext.Items["authToken"],
                paymentInfo);
        }
    }
}