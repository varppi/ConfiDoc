using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConfidocLib;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Xml.XPath;
using Serilog;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly UserManager<ConfidocUser> _userManager;
        private readonly SignInManager<ConfidocUser> _sigInManager;
        private readonly RoleManager<ConfidocRole> _roleManager;
        private readonly Actions _actions;

        public class NewGroupRequest
        {
            [Required]
            [MaxLength(30)]
            public string? Name { get; set; }
        }

        public class GroupUserRequest
        {
            [Required]
            [MaxLength(30)]
            public string? UserName { get; set; }
        }

        public GroupController(
            UserManager<ConfidocUser> userManager,
            SignInManager<ConfidocUser> signInManager,
            RoleManager<ConfidocRole> roleManager,
            Actions actions
            )
        {
            _userManager  = userManager;
            _sigInManager = signInManager;
            _roleManager  = roleManager;
            _actions      = actions;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGroups()
        {
            var user = _actions.GetUser(User);
            if (user is null) return HttpStatus.Login;
            return new JsonResult(await _actions.GetGroups(user));
        }

        [HttpPost]
        [Authorize]
        [Route("{id}/remove")]
        public async Task<IActionResult> RemoveFromGroup(string id, [FromBody] GroupUserRequest req)
        {
            var user = _actions.GetUser(req.UserName ?? "");
            if (user is null) return HttpStatus.NotFoundAllowed;
            var result = await _actions.RemoveFromGroup(id, user, User);
            if (result is null) return HttpStatus.Internal;
            if (!result.Succeeded) 
                return new JsonResult(new {
                    result.Errors
                }){StatusCode=500};

            return HttpStatus.Ok;
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}/delete")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var result = await _actions.DeleteGroup(id, User);
            if (result is null) return HttpStatus.Internal;
            if (!result.Succeeded) 
                return new JsonResult(new {
                    result.Errors
                }){StatusCode=500};
            return HttpStatus.Ok;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetGroup(string id)
        {
            var parsedGroup = await _actions.GetGroup(id, User);
            return new JsonResult(parsedGroup);
        }

        [HttpPost]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> AddToGroup(string id, [FromBody] GroupUserRequest req)
        {
            var user = _actions.GetUser(req.UserName ?? "");
            if (user is null) return HttpStatus.NotFoundAllowed;
            var result = await _actions.AddToGroup(id, user, User);
            if (result is null) return HttpStatus.Internal;
            if (!result.Succeeded) 
                return new JsonResult(new {
                    result.Errors
                }){StatusCode=500};

            return HttpStatus.Ok;
        }

        [HttpPost]
        [Route("new")]
        [Authorize]
        public async Task<IActionResult> NewGroup([FromBody] NewGroupRequest req)
        {
            if (req.Name is null) return HttpStatus.Internal;
            var result = await _actions.CreateGroup(req.Name, User);
            if (result is null || result.Result is null) return HttpStatus.Internal;
            if (!result.Result.Succeeded)
                return HttpStatus.IdentityErrors(result.Result, "Name", 400);
            await _actions.AddToGroup(result.Id!, User);
            return new JsonResult(new {result.Id});
        }
    }
}
