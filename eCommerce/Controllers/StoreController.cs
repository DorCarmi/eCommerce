using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Common;
using eCommerce.Service;
using eCommerce.Service.StorePolicies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eCommerce.Controllers
{

    public class JSONStorePermissions
    {
        public List<StorePermission> StorePermissions { get; set; }
    }
    
    public class StoreAndItemId
    {
        public string StoreId { get; set; }
        public string ItemId { get; set; }
    }
    
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
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

        [HttpPost("[action]")]
        public Result OpenStore(string storeId)
        {
            return _storeService.OpenStore((string) HttpContext.Items["authToken"],
                storeId);
        }

        [HttpPost("[action]")]
        public Result AddItem([FromBody] SItem item)
        {
            return _storeService.AddNewItemToStore((string) HttpContext.Items["authToken"],
                item);
        }
        
        [HttpPost("[action]")]
        public Result RemoveItem([FromBody] StoreAndItemId data)
        {
            return _storeService.RemoveItemFromStore((string) HttpContext.Items["authToken"],
                data.StoreId, data.ItemId);
        }

        [HttpPost("[action]")]
        public Result EditItem([FromBody] SItem item)
        {
            return _storeService.EditItemInStore((string) HttpContext.Items["authToken"],
                item);
        }
        
        [HttpGet("{storeId}/{itemId}")]
        public Result<IItem> GetItem(string storeId, string itemId)
        {
            return _storeService.GetItem((string) HttpContext.Items["authToken"],
                storeId, itemId);
        }
        
        [HttpGet("[action]/{storeId}")]
        public Result<IEnumerable<IItem>> GetAllItems(string storeId)
        {
            return _storeService.GetAllStoreItems((string) HttpContext.Items["authToken"],
                    storeId);
        }
        
        [HttpGet("[action]")]
        public Result<IEnumerable<IItem>> Search(string query)
        {
            return _storeService.SearchForItem((string) HttpContext.Items["authToken"],
                query);
        }
        
        [HttpGet("[action]")]
        public Result<IEnumerable<string>> SearchStore(string query)
        {
            return _storeService.SearchForStore((string) HttpContext.Items["authToken"],
                query);
        }
        
        [HttpGet("{storeId}/history")]
        public Result<SPurchaseHistory> GetStoreHistory(string storeId)
        {
            return _storeService.GetPurchaseHistoryOfStore((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpGet("{storeId}/storeHistory")]
        public Result<SPurchaseHistory> AdminGetStoreHistory(string storeId)
        {
            return _userService.AdminGetPurchaseHistoryStore((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        // ========== Store staff ========== //

        [HttpGet("[action]/{storeId}")]
        public Result<IList<StorePermission>> StorePermissionForUser(string storeId)
        {
            return _storeService.GetStorePermission((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpGet("{storeId}/staff")]
        public Result<IList<StaffPermission>> GetStoreStaffPermissions(string storeId)
        {
            return _storeService.GetStoreStaffAndTheirPermissions((string) HttpContext.Items["authToken"],
                storeId);
        }
        
        [HttpPost("{storeId}/staff")]
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
        
        [HttpPut("{storeId}/staff")]
        public Result UpdateManagerPermissions(string storeId, string role, 
            string userId, [FromBody] JSONStorePermissions storePermissions)
        {
            string token = (string) HttpContext.Items["authToken"];
            Result updateRes;

            switch (role)
            {
                case "manager":
                    updateRes = _userService.UpdateManagerPermission(token, storeId, userId, storePermissions.StorePermissions);
                    break;
                default:
                    updateRes = Result.Fail("Invalid staff role");
                    break;
            }

            return updateRes;
        }
        
        // ========== Store policy ========== //

        [HttpPost("{storeId}/policy")]
        public Result AddPolicy(string storeId, [FromBody] SRuleNode ruleNode)
        {
            return _storeService.AddRuleToStorePolicy((string) HttpContext.Items["authToken"],
                storeId, ruleNode);
        }
    }
}