using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Base class for all api controllers
    // The other apicontrollers will inherit this controller and thus will get its attributes and methods
    // makes code DRY
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}