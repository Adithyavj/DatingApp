using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
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
        private readonly IUserRepository _userRepository;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You cannot send messages to yourself");
            }

            // Get details of both sender and recipient.
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                throw new HubException("User Not Found");
            }

            // create new message entity
            var message = new Message
            {
                Sender = sender,
                SenderUsername = sender.UserName,
                Recipient = recipient,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);

                await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }


        private string GetGroupName(string caller, string other)
        {
            // stringCompare will have true/false depending upon the caller and other.
            var stringCompare = string.CompareOrdinal(caller, other) < 0; // returns 0 if both string are equal,
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}