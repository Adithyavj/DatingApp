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

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "Secret Text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var nFound = _context.Users.Find(-1);
            if (nFound == null) return NotFound();

            return Ok(nFound);
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var nFound = _context.Users.Find(-1);
            var thingToReturn = nFound.ToString(); // nFound will be null since there is no user with id -1(primary key), so this will create an exception

            return thingToReturn;

        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was a bad request");
        }
    }
}