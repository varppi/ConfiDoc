using Confidoc.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly Actions _actions;
        public EventController(Actions actions) {
            _actions = actions;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetEvent(string id)
        {
            if (User is null) return HttpStatus.NotFoundAllowed;
            return new JsonResult(_actions.GetEvent(User, id));
        }
    }
}
