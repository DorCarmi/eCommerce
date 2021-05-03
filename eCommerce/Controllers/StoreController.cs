using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    public class StoreAndItemId
    {
        public string StoreId { get; set; }
        public string ItemId { get; set; }
    }
    
    [Route("api/[controller]")]
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

        [HttpPost]
        [Route("[action]")]
        public Result AddItem([FromBody] SItem item)
        {
            return _storeService.AddNewItemToStore((string) HttpContext.Items["authToken"],
                item);
        }
        
        [HttpPost]
        [Route("[action]")]
        public Result RemoveItem([FromForm] StoreAndItemId data)
        {
            return _storeService.RemoveItemFromStore((string) HttpContext.Items["authToken"],
                data.StoreId, data.ItemId);
        }

        [HttpPost]
        [Route("[action]")]
        public Result EditItem([FromBody] SItem item)
        {
            return _storeService.EditItemInStore((string) HttpContext.Items["authToken"],
                item);
        }
        
        [HttpGet]
        [Route("[action]")]
        public Result<IItem> GetItem([FromBody] StoreAndItemId data)
        {
            return _storeService.GetItem((string) HttpContext.Items["authToken"],
                data.StoreId, data.ItemId);
        }
        
        [HttpGet]
        [Route("[action]/{storeId}")]
        public Result<IEnumerable<IItem>> GetAllItems(string storeId)
        {
            return _storeService.GetAllStoreItems((string) HttpContext.Items["authToken"],
                    storeId);
        }
    }
}