using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConfidocLib;
using Newtonsoft.Json;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ConfidocUser> _userManager;
        private readonly SignInManager<ConfidocUser> _sigInManager;

        public TestController(
            UserManager<ConfidocUser> userManager,
            SignInManager<ConfidocUser> signInManager
            )
        {
            _userManager = userManager;
            _sigInManager = signInManager;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<string> Get(string id)
        {
            return $"delete_user_{Guid.NewGuid().ToString().Replace("-", "").Substring(0,10)}";

        }

        [Route("/test2")]
        [Authorize]
        public async Task<IActionResult> Get2()
        {
            return Ok("you made it");
        }
    }
}
