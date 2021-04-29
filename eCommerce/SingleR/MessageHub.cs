using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.SingleR
{
    public class MessageModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }
    
    public class MessageHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task Message(MessageModel message)
        {
            Console.WriteLine($"Get message {message.Message} from {message.UserName}");
            await Clients.Others.SendAsync("message", message);
        }
    }
}