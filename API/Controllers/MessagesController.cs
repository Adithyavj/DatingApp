using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        // Create a message
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            // Get details of both sender and recipient.
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                return NotFound();
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
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            // logged in user
            var currentUsername = User.GetUserName();

            var messageThread = await _messageRepository.GetMessageThread(currentUsername, username);

            return Ok(messageThread);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userName = User.GetUserName(); //get user

            var message = await _messageRepository.GetMessage(id); //get message

            // if either sender/recipient is not the username,
            if (message.Sender.UserName != userName && message.Recipient.UserName != userName)
            {
                return Unauthorized();
            }

            if (message.Sender.UserName == userName) message.SenderDeleted = true;

            if (message.Recipient.UserName == userName) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting the message");

        }
    }
}