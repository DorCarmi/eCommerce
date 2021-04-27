using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace eCommerce.SingleR
{
    public class MessageHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("--> Connection Opened: " + Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
            return base.OnConnectedAsync();
        }
    }
}