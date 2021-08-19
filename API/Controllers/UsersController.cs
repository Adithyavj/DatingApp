using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            userParams.CurrentUsername = user.UserName;

            // if no particular gender is passed in the query string,
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                // if the current user is a Male, the memberlist should return females and viceversa
                // swapping gender
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await _userRepository.GetMembersAsync(userParams);

            // get access to httpResponse and add pagination to the header of the response.
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

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

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            // getting user based on token using extension method
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            // getting photo based on id coming from client
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            // setting current main photo to false
            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }
            // setting new photo as main
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            // getting photo based on id coming from client
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            // if publicid is not null, delete the photo from cloudinary
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    // if there is an error when deleting from cloudinary, then we stop here and return error.
                    return BadRequest(result.Error.Message);
                }
            }

            // remove the photo from collection
            user.Photos.Remove(photo);
            // update changes in db
            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");

        }

    }
}