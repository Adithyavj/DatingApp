namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
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
        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

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

        _unitOfWork.MessageRepository.AddMessage(message);

        if (await _unitOfWork.Complete())
        {
            return Ok(_mapper.Map<MessageDto>(message));
        }

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();

        var messages = await _unitOfWork.MessageRepository.GetMessageForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        // logged in user
        var currentUsername = User.GetUserName();

        var messageThread = await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username);

        return Ok(messageThread);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var userName = User.GetUserName(); //get user

        var message = await _unitOfWork.MessageRepository.GetMessage(id); //get message

        // if either sender/recipient is not the username,
        if (message.Sender.UserName != userName && message.Recipient.UserName != userName)
        {
            return Unauthorized();
        }

        if (message.Sender.UserName == userName) message.SenderDeleted = true;

        if (message.Recipient.UserName == userName) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
        {
            _unitOfWork.MessageRepository.DeleteMessage(message);
        }

        if (await _unitOfWork.Complete())
        {
            return Ok();
        }

        return BadRequest("Problem deleting the message");

    }
}
