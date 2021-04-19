using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class UserTransactionHistory
    {
        private ConcurrentBag<PurchaseRecord> _purchases;
        private Object transLock;

        public UserTransactionHistory()
        {
            _purchases = new ConcurrentBag<PurchaseRecord>();
        }
        public Result EnterRecordToHistory(PurchaseRecord record)
        {    
            _purchases.Add(record);
            return Result.Ok();
        }
        public Result<IList<PurchaseRecord>> GetUserHistory()
        {
            return Result.Ok<IList<PurchaseRecord>>(new List<PurchaseRecord>(_purchases.ToArray()));
        }

    }
}