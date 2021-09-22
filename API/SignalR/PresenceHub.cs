using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    // SignalR websockets cannot send authentication headers, we user query string to send data.
    [Authorize]
    public class PresenceHub : Hub
    {
        // we us the tracker to store the users in a dictionary to keep track of the users.
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        // when a client connect, we update the presence tracker and send the updated list of online users to all the clients.
        public override async Task OnConnectedAsync()
        {
            // passing username and connectionid to the presence tracker class to get the connection status
            await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            // when a client is online,
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

            // get all online users
            var currentUsers = await _tracker.GetOnlineUsers();
            // pass down the currently active user[] to all users (SignalR emits this to the client)
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            // when a client is offline,pass back his userName
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());
            // get all online users
            var currentUsers = await _tracker.GetOnlineUsers();
            // pass down the currently active user[] to all users (SignalR emits this to the client)
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}