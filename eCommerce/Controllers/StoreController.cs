using System;
using System.Collections;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    public class StoreName
    {
        public string StoreId { get; set; }
    }
    
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
        private readonly IUserService _userService;

        public StoreController(ILogger<StoreController> logger)
        {
            _logger = logger;
            _storeService = new StoreService();
            _userService = new UserService();
        }


        // the route is /Store/OpenStore  
        //removes "Controller" from the class name and add the name of the function as an endpoint 

        [HttpPost]
        [Route("[action]")]
        public Result OpenStore([FromBody] StoreName storeName)
        {
            return _storeService.OpenStore((string) HttpContext.Items["authToken"],
                storeName.StoreId);
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
        public Result RemoveItem([FromBody] StoreAndItemId data)
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
        [Route("{storeId}/{itemId}")]
        public Result<IItem> GetItem(string storeId, string itemId)
        {
            return _storeService.GetItem((string) HttpContext.Items["authToken"],
                storeId, itemId);
        }
        
        [HttpGet]
        [Route("[action]/{storeId}")]
        public Result<IEnumerable<IItem>> GetAllItems(string storeId)
        {
            return _storeService.GetAllStoreItems((string) HttpContext.Items["authToken"],
                    storeId);
        }
        
        [HttpGet]
        [Route("[action]")]
        public Result<IEnumerable<IItem>> Search(string query)
        {
            return _storeService.SearchForItem((string) HttpContext.Items["authToken"],
                query);
        }
        
        [HttpGet]
        [Route("[action]")]
        public Result<IEnumerable<string>> SearchStore(string query)
        {
            return _storeService.SearchForStore((string) HttpContext.Items["authToken"],
                query);
        }
        
        // ========== Store staff ========== //

        [HttpGet]
        [Route("{storeId}/staff")]
        public Result<IList<StaffPermission>> GetStoreStaffPermissions(string storeId)
        {
            return _storeService.GetStoreStaffAndTheirPermissions((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpPost]
        [Route("{storeId}/staff")]
        public Result AppointStaff(string storeId, string role, string userId)
        {
            string token = (string) HttpContext.Items["authToken"];
            Result appointRes;

            switch (role)
            {
                case "owner":
                    appointRes = _userService.AppointCoOwner(token, storeId, userId); 
                    break;
                case "manager":
                    appointRes = _userService.AppointManager(token, storeId, userId);
                    break;
                default:
                    appointRes = Result.Fail("Invalid staff role");
                    break;
            }

            return appointRes;
        }
    }
}