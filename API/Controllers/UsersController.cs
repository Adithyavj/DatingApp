using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // api/Users
        [HttpGet]
        // IEnumberable return a collection of users
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();

            // using automapper to map users to MemberDto
            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            // since we are returning an ActionResult, we wrap it in OK()
            return Ok(usersToReturn);
        }

        // api/Users/adithya
        [HttpGet("{username}")]
        // returns a single user
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            return _mapper.Map<MemberDto>(user);
        }
    }
}