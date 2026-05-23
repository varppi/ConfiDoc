using Confidoc.Server.Database;
using Confidoc.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Confidoc.Server.Helpers;
using System.Net;
using ConfidocLib;
using System.Security.Cryptography;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        public class NewDocumentFormRequest
        {
            [Required(ErrorMessage = "document name is required")]
            [MaxLength(30)]
            public string? Name { get; set; }
            public string? Password { get; set; }
        }

        public class SaveDocumentRequest
        {
            [Required]
            public string? Data { get; set; }
            public string? Password { get; set; }
        }

        public class GetDocumentRequest
        {
            public string? Password { get; set; }
        }

        public class RemoveAccessRequest
        {
            [Required]
            public string? Name { get; set; }
        }

        public class AddAccessRequest
        {
            [Required]
            public string? Name { get; set; }
            [Required]
            public string? Level { get; set; }
            [Required]
            public double? Duration { get; set; }
        }

        private readonly SignInManager<ConfidocUser> _signInManager;
        private readonly UserManager<ConfidocUser> _userManager;
        private readonly Actions _actions;

        public DocumentController(
            SignInManager<ConfidocUser> signInManager,
            UserManager<ConfidocUser> userManager,
            Actions actions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _actions = actions;
        }

        [Route("new")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNew([FromBody] NewDocumentFormRequest req)
        {
            var user = _actions.GetUser(User);
            if (user is null) return HttpStatus.Login;

            var id = _actions.CreateDocument(new Document
            {
                Name = req.Name,
                Owner = user,
                Encrypted = req.Password is null 
                ? null
                : Security.Encrypt("confidoc", Security.HashString(req.Password))
            });
            return new JsonResult(new
            {
                Id = id,
                Password = Security.HashString(req.Password??""),
            });
        }

        [Route("")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDocs()
            => new JsonResult(_actions.GetDocuments(User));


        [Route("{id}")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetDoc(string id, [FromBody] GetDocumentRequest req)
        {
            ParsedDocument? doc;
            try
            {
                doc = _actions.GetDocument(id, User, req.Password);
            }
            catch (CryptographicException)
            {
                return HttpStatus.Forbidden;
            }
            if (doc is null)
                return HttpStatus.NotFoundAllowed;

            return new JsonResult(doc);
        }

        [Route("{id}/modify")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveDoc(string id, [FromBody] SaveDocumentRequest req)
        {
            if (req.Data is null) return BadRequest();

            var success = _actions.SaveDocument(id, req.Data, User, req.Password);
            if (!success) return HttpStatus.NotFoundAllowed;

            return HttpStatus.Ok;
        }

        [Route("{id}/delete")]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteDoc(string id)
        {
            var success = _actions.DeleteDocument(id, User);
            if (!success) return HttpStatus.NotFoundAllowed;

            return HttpStatus.Ok;
        }

        [Route("{id}/remove/user")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveUserDocAccess(string id, [FromBody] RemoveAccessRequest req)
        {
            if (_actions.DocumentAccessLevel(User, id) < 3) return HttpStatus.NotFoundAllowed;
            var user = _actions.GetUser(req.Name!);
            if (user is null) return HttpStatus.NotFoundAllowed;
            _actions.RemoveDocumentAccess(user, id);
            return HttpStatus.Ok;
        }

        [Route("{id}/remove/group")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveGroupDocAccess(string id, [FromBody] RemoveAccessRequest req)
        {
            if (_actions.DocumentAccessLevel(User, id) < 3) return HttpStatus.NotFoundAllowed;
            _actions.RemoveDocumentAccess(_actions.GetGroupUnsafe(req.Name!), id);
            return HttpStatus.Ok;
        }


        [Route("{id}/add/user")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUserDocAccess(string id, [FromBody] AddAccessRequest req)
        {
            if (_actions.DocumentAccessLevel(User, id) < 3) return HttpStatus.NotFoundAllowed;
            var user = _actions.GetUser(req.Name!);
            if (user is null) return HttpStatus.NotFoundAllowed;
            switch(req.Level)
            {
                case "read":
                    _actions.AddDocumentReadAccess(User, user, id, (double)(req.Duration ?? 0));
                    break;
                case "write":
                    _actions.AddDocumentWriteAccess(User, user, id, (double)(req.Duration ?? 0));
                    break;
            }
            return HttpStatus.Ok;
        }


        [Route("{id}/add/group")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddGroupDocAccess(string id, [FromBody] AddAccessRequest req)
        {
            if (_actions.DocumentAccessLevel(User, id) < 3) return HttpStatus.NotFoundAllowed;
            var group = _actions.GetGroupUnsafe(req.Name!);
            switch(req.Level)
            {
                case "read":
                    _actions.AddDocumentReadAccess(User, group, id, (double)(req.Duration??0));
                    break;
                case "write":
                    _actions.AddDocumentWriteAccess(User, group, id, (double)(req.Duration??0));
                    break;
            }
            return HttpStatus.Ok;
        }
    }
}
