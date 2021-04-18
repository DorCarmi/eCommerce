using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace eCommerce.Common
{
    public class ConcurrentIdGenerator
    {
        private SpinLock _mutex;
        private volatile bool _lockTaken;
        private long _id;

        public ConcurrentIdGenerator(long startFromId)
        {
            _mutex = new SpinLock();
            _lockTaken = false;
            _id = startFromId;
        }
        
        public long MoveNext()
        {
            long prevValue = -1;
            _mutex.Enter(ref _lockTaken);
            prevValue = _id;
            _id++;
            _lockTaken = false;
            _mutex.Exit();
            return prevValue;
        }
    }
}