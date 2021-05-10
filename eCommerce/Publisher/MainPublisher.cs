using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace eCommerce.Publisher
{
    public class MainPublisher : PublisherObservable
    {
        private ConcurrentDictionary<string, ConcurrentQueue<string>> messages;
        private ConcurrentDictionary<string, bool> connected;

        private ConcurrentBag<UserObserver> observers;
        public void Connect(string userID)
        {
            this.connected[userID] = true;
            if (messages.ContainsKey(userID) && messages[userID].Count > 0)
            {
                NotifyAll();
            }
        }
        
        public void Disconnect(string userId)
        {
            connected.TryRemove(userId, out var tstring);
        }

        public void AddMessageToUser(string userid, string message)
        {
            if (!this.messages.ContainsKey(userid))
            {
                this.messages.TryAdd(userid, new ConcurrentQueue<string>());
            }
            this.messages[userid].Enqueue(message);
        }

        public MainPublisher()
        {
            observers = new ConcurrentBag<UserObserver>();
            
        }

        public void Register(UserObserver userObserver)
        {
            observers.Add(userObserver);
        }

        public async void NotifyAll()
        {
            foreach (var observer in this.observers)
            {
                observer.Notify(null,null);
            }
        }
    }
}