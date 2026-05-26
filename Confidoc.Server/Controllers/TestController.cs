using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using ConfidocLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PuppeteerSharp;
using SQLitePCL;
using System.Drawing;

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
        //[Route("/test")]
        //public async Task<IActionResult> Get(string? id)
        //{
        //    if (id is not null) _actions.CreateUserDocumentGrant(User, "fff", id, 1, .01);
        //    return new JsonResult(_actions.GetGrants());
        //}

        //[HttpGet]
        //[Route("/test2")]
        //public async Task<IActionResult> Get2(string? id)
        //{
        //    if (id is not null) return new JsonResult(_actions.GrantLevelForDocument(User, id));
        //    return new JsonResult(_actions.GetGrants());
        //}

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
