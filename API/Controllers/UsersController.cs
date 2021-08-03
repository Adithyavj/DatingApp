using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // api/Users
        [HttpGet]
        // IEnumberable return a collection of users
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            // since we are returning an ActionResult, we wrap it in OK()
            return Ok(await _userRepository.GetUsersAsync());
        }

        // api/Users/adithya
        [HttpGet("{username}")]
        // returns a single user
        public async Task<ActionResult<AppUser>> GetUser(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }
    }
}