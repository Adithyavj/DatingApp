using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // api/Users
        [HttpGet]
        // IEnumberable return a collection of users
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            // old code
            #region Old-Code
            // var users = await _userRepository.GetUsersAsync();

            // // using automapper to map users to MemberDto
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            // // since we are returning an ActionResult, we wrap it in OK()
            // return Ok(usersToReturn);
            #endregion
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);

        }

        // api/Users/adithya
        // naming this route so that we can return CreatedAtRoute in photo post method
        [HttpGet("{username}", Name = "GetUser")]
        // returns a single user
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // fetches the user's username from the token that the api is using to authenticate this endpoint
            // fetch userdetails using the username 
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName()); // calling extension method to get username

            // mapping memberupdateDto coming from client to user(AppUser) object
            _mapper.Map(memberUpdateDto, user);

            // adding track of user object
            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update user");
        }

        // method to add a photo
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // fetch userdetails using the username 
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName()); // calling extension method to get username

            // send photo from client(Angular) to the Cloudinary API
            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            // mapping values obtained from result(Cloudinary API) to our Photo enitity
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // if it's the first photo, set it to main
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            // track the photo entity
            user.Photos.Add(photo);
            // save changes to DataBase
            if (await _userRepository.SaveAllAsync())
            {
                // map result entity to photoDto to pass to client with a 201 Created response
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem Adding Photo");
        }

    }
}