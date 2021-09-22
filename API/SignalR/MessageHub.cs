using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            // create a group for each user.
            // define the group name (combination of sender username and reciever username)
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext.Request.Query["user"].ToString(); // the user who recieves the message.

            // eg:- Lisa-Todd or Todd-Lisa
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            // stringCompare will have true/false depending upon the caller and other.
            var stringCompare = string.CompareOrdinal(caller, other) < 0; // returns 0 if both string are equal,
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}