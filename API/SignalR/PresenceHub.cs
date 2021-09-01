using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // when a client is online,
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // when a client is offline,pass back his userName
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            await base.OnDisconnectedAsync(exception);
        }
    }
}