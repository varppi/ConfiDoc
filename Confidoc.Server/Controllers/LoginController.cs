using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
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

        public LoginController(
            SignInManager<ConfidocUser> signInManager,
            UserManager<ConfidocUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FormRequest req)
        {
            var invalidCredentials = HttpStatus.FieldErrors(new()
            {
                {"Password", ["invalid username or password"]}
            });
            
            var user = await _userManager.FindByNameAsync(req.Username!);
            if (user is null) return invalidCredentials;

            var loginResult = await _signInManager.PasswordSignInAsync(req.Username!, req.Password!, true, false);
            if (!loginResult.Succeeded) return invalidCredentials;

            var token = Jwt.GenerateToken(user);
            return new JsonResult(new { token });
        }
    }
}
