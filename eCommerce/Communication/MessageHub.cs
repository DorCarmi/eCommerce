using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Common;
using eCommerce.Publisher;
using eCommerce.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.Communication
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
        private ConcurrentDictionary<string, string> _connectionToUser;
        private IAuthService _authService;
        private IUserService _userService;
        public MessageHub()
        {
            _mainPublisher = MainPublisher.Instance;
            _mainPublisher.Register(this);
            _userToConnection = new ConcurrentDictionary<string, IList<string>>();
            _connectionToUser = new ConcurrentDictionary<string, string>();
            _authService = new AuthService();
            _userService = new UserService();
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                // close connection
                return base.OnDisconnectedAsync(null);
            }

            var authToken = httpContext.Request.Cookies["authToken"];
            if (!_authService.IsUserConnected(authToken))
            {
                return base.OnDisconnectedAsync(null);
            }
            //Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            
            var userBasicData = _userService.GetUserBasicInfo(authToken).Value;
            var userId = userBasicData.Username;
            if (!_userToConnection.TryRemove(userId, out var connectionList))
            {
                connectionList = new List<string>();
            }

            connectionList.Add(Context.ConnectionId);
            _userToConnection.TryAdd(userId, connectionList);
            _connectionToUser.TryAdd(Context.ConnectionId, userId);

            Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            _mainPublisher.Connect(Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectionToUser.TryRemove(Context.ConnectionId, out var userId) &&
                _userToConnection.TryRemove(userId, out var connectionList))
            {
                connectionList.Remove(Context.ConnectionId);
                _userToConnection.TryAdd(userId, connectionList);

                if (connectionList.Count == 0)
                {
                    _mainPublisher.Disconnect(userId);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task Message(MessageModel message)
        {
            Console.WriteLine($"GetDiscount message {message.Message} from {message.UserName} to {message.ToUser}");
            MainPublisher mainPublisher = MainPublisher.Instance;
            mainPublisher.Connect(message.UserName);
            await Clients.Others.SendAsync("message", message);
        }

        public async void Notify(string userId, ConcurrentQueue<string> messages)
        {
            if (!_userToConnection.TryGetValue(userId, out var connectionsIds))
            {
                return;
            }
            
            while (!messages.IsEmpty)
            {
                if (messages.TryDequeue(out var message))
                {
                    await Clients.Clients(connectionsIds).SendAsync("message", message);
                }
            }
        }
    }
}