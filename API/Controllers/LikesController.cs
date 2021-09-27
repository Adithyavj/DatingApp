using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            // calling externsion method on user entity to get userId
            var sourceUserId = User.GetUserId(); // loggedIn user

            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username); // all details of the person user liked

            var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId); // all details of the person

            if (likedUser == null)
            {
                return NotFound();
            }
            if (sourceUser.UserName == username) // if user liked himself
            {
                return BadRequest("You can't like yourself!");
            }

            // calling method to check if the user has liked this user
            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

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

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

            // since the above method returns a pagedList, it will have info in header about pageSize, number etc
            // add this to response header
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }


    }
}