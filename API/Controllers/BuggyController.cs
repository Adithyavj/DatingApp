using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }

        // 400 Bad Request Error
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was a bad request");
        }

        // 401 Unauthorized Error
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        // 404 Not Found error
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var nFound = _context.Users.Find(-1);
            if (nFound == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(nFound);
            }
        }

        // 500 Internal Server Error
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var nFound = _context.Users.Find(-1);
            var thingToReturn = nFound.ToString(); // nFound will be null since there is no user with id -1(primary key), so this will create an exception

            return thingToReturn;
        }

    }
}