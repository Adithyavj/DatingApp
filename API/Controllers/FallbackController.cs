using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    ///
    /// Controller for routing the Angular routes.
    /// If API doesn't know what to do with a route, then it falls back to this controller.
    ///
    public class FallbackController : Controller // Class for MVC controller with View support
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                   "wwwroot", "index.html"), "text/HTML");
        }
    }
}