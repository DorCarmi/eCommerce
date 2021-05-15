using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Publisher;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.SingleR
{
    public class MessageModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public string ToUser { get; set; }
    }
    
    public class MessageHub : Hub, UserObserver
    {
        private MainPublisher _mainPublisher;
        private ConcurrentDictionary<string, IList<string>> _userToConnection;
        public MessageHub()
        {
            _mainPublisher = MainPublisher.Instance;
            _mainPublisher.Register(this);
            _userToConnection = new ConcurrentDictionary<string, IList<string>>();
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            
            _mainPublisher.Connect(Context.ConnectionId);
            this._userToConnection.TryAdd(Context.ConnectionId, new List<string>(){Context.ConnectionId});
            
            return base.OnConnectedAsync();
        }

        public async Task Message(MessageModel message)
        {
            Console.WriteLine($"Get message {message.Message} from {message.UserName} to {message.ToUser}");
            MainPublisher mainPublisher = MainPublisher.Instance;
            mainPublisher.Connect(message.UserName);
            await Clients.Others.SendAsync("message", message);
        }

        public async void Notify(string userName, ConcurrentQueue<string> messages)
        {
            foreach (var user in _userToConnection[userName])
            {
                while (!messages.IsEmpty)
                {
                    string result;
                    messages.TryDequeue(out result);
                    await Clients.User(user).SendAsync(result);
                }
                
            }
        }
    }
}