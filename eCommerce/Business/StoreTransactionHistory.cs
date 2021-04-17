﻿using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class StoreTransactionHistory
    {

        private IList<PurchaseRecord> _history;
        private IStore _store;

        public StoreTransactionHistory(IStore store)
        {
            this._store = store;
            this._history = new List<PurchaseRecord>();
        }

        public Result AddRecordToHistory(IBasket basket)
        {
            PurchaseRecord newRecord = new PurchaseRecord(_store, basket, DateTime.Now);
            this._history.Add(newRecord);
            return Result.Ok();
        }

        public Result<IList<PurchaseRecord>> GetHistory(IUser user)
        {
            if (user.HasPermission(_store, StorePermission.GetStoreHistory).IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>("User doesn't have the permission to get store's history");
                
            }
            else
            {
                return Result.Ok(this._history);
                
            }
        }
        
        public Result<IList<PurchaseRecord>> GetHistory(User user, DateTime dateStart, DateTime dateEnd)
        {
            if (user.HasPermission(_store, StorePermission.GetStoreHistory).IsSuccess)
            {
                var lst = this._history.Where(x => x.GetDate() >= dateStart && x.GetDate() <= dateEnd).ToList();
                return Result.Ok<IList<PurchaseRecord>>(lst);
            }
            else
            {
                return Result.Fail<IList<PurchaseRecord>>("User doesn't have the permission to get store's history");
                
            }
        }
        

    }

    
}