using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Message> GetMessage(int id)
        {
            // find message using messageId (primary key)
            return await _context.Messages
                    .Include(u => u.Sender)
                    .Include(u => u.Recipient)
                    .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            // Get all message order by descending of send datetime
            var query = _context.Messages
                            .OrderByDescending(m => m.MessageSent)
                            .AsQueryable();

            // now add where condition based on Container using switch statement.
            // add checks for returning only not deleted messages
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username &&
                                            u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username &&
                                             u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username &&
                                      u.RecipientDeleted == false &&
                                      u.DateRead == null)
            };

            // project message to messageDto (using automapper)
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                        .Include(x => x.Connections)
                        .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        // Get conversation of a user
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            // get list of messages between 2 users - Messages, Sender, Recipient, and their photos
            var messages = await _context.Messages
                                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                                .Where(m => m.Recipient.UserName == currentUsername
                                    && m.RecipientDeleted == false
                                    && m.Sender.UserName == recipientUsername
                                    || m.Recipient.UserName == recipientUsername
                                    && m.Sender.UserName == currentUsername
                                    && m.SenderDeleted == false
                                )
                                .OrderBy(m => m.MessageSent)
                                .ToListAsync();

            // update unread messages as read.
            var unreadMessages = messages.Where(m => m.DateRead == null
                                             && m.Recipient.UserName == currentUsername)
                                         .ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                // update changes in DB
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // if > 0 changes have been made and return true
        }
    }
}