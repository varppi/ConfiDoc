using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConfidocLib;
using Newtonsoft.Json;
using SQLitePCL;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly UserManager<ConfidocUser> _userManager;
        private readonly SignInManager<ConfidocUser> _sigInManager;
        private readonly Actions _actions;
        public TestController(
            UserManager<ConfidocUser> userManager,
            SignInManager<ConfidocUser> signInManager,
            Actions actions
            )
        {
            _userManager = userManager;
            _sigInManager = signInManager;
            _actions = actions;
        }

        //[HttpGet]
        //[Route("/testss")]
        //public async Task<string> Get()
        //{
        //    var result = await _userManager.AddToRoleAsync(_actions.GetUser(User), "admin");
        //    return JsonConvert.SerializeObject(result.Errors);
        //    //return $"delete_user_{Guid.NewGuid().ToString().Replace("-", "").Substring(0,10)}";

        //}

        //[Route("/test2")]
        //[Authorize]
        //public async Task<IActionResult> Get2()
        //{
        //    return Ok("you made it");
        //}
    }
}
