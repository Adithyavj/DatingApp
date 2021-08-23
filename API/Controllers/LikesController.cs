using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            // calling externsion method on user entity to get userId
            var sourceUserId = User.GetUserId(); // loggedIn user

            var likedUser = await _userRepository.GetUserByUsernameAsync(username); // all details of the person user liked

            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId); // all details of the person

            if (likedUser == null)
            {
                return NotFound();
            }
            if (sourceUser.UserName == username) // if user liked himself
            {
                return BadRequest("You can't like yourself!");
            }

            // calling method to check if the user has liked this user
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null)
            {
                return BadRequest("You have already liked this user");
            }

            // add a new UserLike
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());

            return Ok(users);
        }


    }
}