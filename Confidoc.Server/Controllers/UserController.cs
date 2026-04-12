using Microsoft.AspNetCore.Mvc;
using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ConfidocLib;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public class PasswordFormRequest
        {
            [Required(ErrorMessage = "current password is required")]
            public string? Password { get; set; }

            [Required(ErrorMessage = "new password is required")]
            public string? NewPassword { get; set; }

            [Required(ErrorMessage = "password confirmation is required")]
            public string? NewPassword2 { get; set; }
        }

        public class DeleteAccountFormRequest
        {
            [Required(ErrorMessage = "current password is required")]
            public string? Password { get; set; }
        }


        private readonly UserManager<ConfidocUser> _userManager;
        private readonly SignInManager<ConfidocUser> _sigInManager;
        private readonly Actions _actions;

        public UserController(
            UserManager<ConfidocUser> userManager,
            SignInManager<ConfidocUser> signInManager,
            Actions actions
            )
        {
            _userManager = userManager;
            _sigInManager = signInManager;
            _actions = actions;
        }

        [Authorize]
        [Route("changepassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordFormRequest req)
        {
            var user = _actions.GetUser(User);
            if (user is null) return HttpStatus.Forbidden;

            if (req.NewPassword != req.NewPassword2) 
                return HttpStatus.FieldErrors(
                    new(){{"newpassword", ["new passwords do not match"]}}, 
                    400
                );

            var success = await _actions.ChangePassword(user, req.NewPassword??"", req.Password??"");
            if (!success)                 
                return HttpStatus.FieldErrors(
                    new(){{"password", ["something went wrong. make sure your current password is correct."] }}, 
                    400
                );
            return HttpStatus.Ok;
        }

        [Authorize]
        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountFormRequest req)
        {
            var user = _actions.GetUser(User);
            if (user is null) return HttpStatus.Forbidden;

            var result = await _actions.DeleteUser(User, req.Password??"");
            if (!result) return HttpStatus.FieldErrors(new Dictionary<string, IEnumerable<string>>
            {
                { "password", new string[] {"something went wrong. make sure your current password is correct."}},
            });
            return Redirect("/logout");
        }
    }
}