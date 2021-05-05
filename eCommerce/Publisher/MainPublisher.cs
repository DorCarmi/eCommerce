using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace eCommerce.Publisher
{
    public class MainPublisher : PublisherObservable
    {
        private ConcurrentDictionary<string, List<string>> messages;
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

        public void AddMessageToUser(string userid, string message)
        {
            try
            {
                this.messages[userid].Add(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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