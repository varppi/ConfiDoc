using Confidoc.Server.Database;
using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.ComponentModel.DataAnnotations;

namespace Confidoc.Server.Controllers
{
    public class AdminCreationForm
    {
        [Required]
        public string? Password { get; set; }
    }

    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly RoleManager<ConfidocRole> _roleManager;
        private readonly UserManager<ConfidocUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly Actions _actions;
        public InternalController(
            Actions actions, 
            UserManager<ConfidocUser> userManager,
            RoleManager<ConfidocRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _actions = actions;
            _context = context;
        }

        [Route("/setup")]
        [HttpGet]
        public IActionResult GetIsSetup()
        {
            return new JsonResult(new
            {
                hasAdmin = _actions.HasAdmin()
            });
        }

        [Route("/setup")]
        [HttpPost]
        public async Task<IActionResult> CreateAdminAccount(AdminCreationForm form)
        {
            if (_actions.HasAdmin()) return HttpStatus.NotFoundAllowed;
            if (form.Password is null) return HttpStatus.BadRequest;
            var result = await _actions.CreateUser("admin", form.Password);
            if (!result.Succeeded) return HttpStatus.IdentityErrors(result, "password");
            var user = _actions.GetUser("admin");
            if (user is null) return HttpStatus.Internal;
            result = await _userManager.AddToRoleAsync(user, "admin");
            var role = _context.Roles.First(role => role.Name == "admin");
            role.DisplayName = "admin accounts";
            role.Owner = user;
            _context.SaveChanges();
            if (result is null) return HttpStatus.Internal;
            if (!result.Succeeded) return HttpStatus.IdentityErrors(result, "password");
            return HttpStatus.Ok;
        }
    }
}
