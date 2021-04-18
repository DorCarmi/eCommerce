using System;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class UserTransactionHistory
    {
        private IList<PurchaseRecord> _purchases;
        private Object transLock;

        public UserTransactionHistory()
        {
            _purchases = new List<PurchaseRecord>();
            transLock = new Object();
        }
        public Result EnterRecordToHistory(PurchaseRecord record)
        {
            throw new System.NotImplementedException();
        }
        public Result<IList<PurchaseRecord>> GetUserHistory()
        {
            return Result.Ok<IList<PurchaseRecord>>(_purchases);
        }

    }
}