using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        public class FormRequest
        {
            [Required(ErrorMessage = "username is required")]
            [MaxLength(30)]
            public string? Username { get; set; }

            [Required(ErrorMessage = "password is required")]
            public string? Password { get; set; }
        }

        private readonly SignInManager<ConfidocUser> _signInManager;
        private readonly UserManager<ConfidocUser> _userManager;
        private readonly Actions _actions;

        public RegisterController(
            SignInManager<ConfidocUser> signInManager,
            UserManager<ConfidocUser> userManager,
            Actions actions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _actions = actions;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FormRequest request)
        {
            var registerResult = await _actions.CreateUser(request.Username!, request.Password!);
            if (!registerResult.Succeeded)
                return HttpStatus.IdentityErrors(registerResult, "Password");

            return HttpStatus.Ok;
        }
    }
}
