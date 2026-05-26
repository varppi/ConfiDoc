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
using System.Text;
using System.Buffers.Text;

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
            public string? Token { get; set; }
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
            _actions.AddEvent(_actions.TemplateEvent(
                Request,
                HttpContext,
                User,
                $"create:{id}"
                )
            );
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
                doc = _actions.GetDocument(id, User, req.Password, download: false);
            }
            catch (CryptographicException)
            {
                return HttpStatus.Forbidden;
            }
            if (doc is null)
                return HttpStatus.NotFoundAllowed;

            _actions.AddEvent(_actions.TemplateEvent(
                Request,
                HttpContext,
                User,
                $"view:{id}"
                )
            );

            return new JsonResult(doc);
        }

        [Route("{id}/pdf")]
        [HttpPost]
        public async Task<IActionResult> GetPdfVersion(string id, [FromForm] GetDocumentRequest req)
        {
            ConfidocUser? user = null;
            if (User.Identity is null || User.Identity.Name is null)
            {
                var jwtName = Jwt.GetUsernameFromToken(req.Token ?? "");
                if (jwtName is not null) user = _actions.GetUser(jwtName);
            }
            else user = _actions.GetUser(User);

            if (user is null) return HttpStatus.NotFoundAllowed;
            ParsedDocument? doc;
            try
            {
                doc = _actions.GetDocument(id, user, req.Password, downLevel: 1, download: true);
                var eventId = _actions.AddEvent(_actions.TemplateEvent(
                    Request,
                    HttpContext,
                    user,
                    $"download:{id}"
                    )
                );
                doc = _actions.GetDocument(
                    id, 
                    user, 
                    req.Password, 
                    downLevel: 1, 
                    download: true, 
                    data: 
                    eventId
                );
            }
            catch (CryptographicException)
            {
                return HttpStatus.Forbidden;
            }
            if (doc is null)
                return HttpStatus.NotFoundAllowed;

            return File(Convert.FromBase64String(doc.Data ?? ""), "application/pdf", $"{doc.Name}.pdf");
        }

        [Route("{id}/modify")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveDoc(string id, [FromBody] SaveDocumentRequest req)
        {
            if (req.Data is null) return BadRequest();

            var success = _actions.SaveDocument(id, req.Data, User, req.Password);
            if (!success) return HttpStatus.NotFoundAllowed;

            _actions.AddEvent(_actions.TemplateEvent(
                Request,
                HttpContext,
                User,
                $"update:{id}"
                )
            );

            return HttpStatus.Ok;
        }

        [Route("{id}/delete")]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteDoc(string id)
        {
            var success = _actions.DeleteDocument(id, User);
            if (!success) return HttpStatus.NotFoundAllowed;
            _actions.AddEvent(_actions.TemplateEvent(
                Request,
                HttpContext,
                User,
                $"delete:{id}"
                )
            );
            return HttpStatus.Ok;
        }

        [Route("{id}/remove/user")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveUserDocAccess(string id, [FromBody] RemoveAccessRequest req)
        {
            var user = _actions.GetUser(req.Name!);
            if (_actions.DocumentAccessLevel(User, id) < 3 && req.Name != user?.UserName) return HttpStatus.NotFoundAllowed;
            if (user is null) return HttpStatus.NotFoundAllowed;
            _actions.RemoveDocumentAccess(user, id);

            _actions.AddEvent(_actions.TemplateEvent(
                Request,
                HttpContext,
                User,
                $"remove access from user '{user.UserName?.Replace(":", "")}':{id}"
                )
            );
            return HttpStatus.Ok;
        }

        [Route("{id}/remove/group")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveGroupDocAccess(string id, [FromBody] RemoveAccessRequest req)
        {
            if (_actions.DocumentAccessLevel(User, id) < 3) return HttpStatus.NotFoundAllowed;
            _actions.RemoveDocumentAccess(_actions.GetGroupUnsafe(req.Name!), id);

            _actions.AddEvent(_actions.TemplateEvent(
                Request,
                HttpContext,
                User,
                $"remove access from group '{req.Name?.Replace(":", "")}':{id}"
                )
            );
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
                    _actions.AddEvent(_actions.TemplateEvent(
                        Request,
                        HttpContext,
                        User,
                        $"add read access to user '{user.UserName?.Replace(":", "")}' for {req.Duration}h:{id}"
                        )
                    );
                    break;
                case "write":
                    _actions.AddDocumentWriteAccess(User, user, id, (double)(req.Duration ?? 0));
                    _actions.AddEvent(_actions.TemplateEvent(
                        Request,
                        HttpContext,
                        User,
                        $"add read & write access to user '{user.UserName?.Replace(":", "")}' for {req.Duration}h:{id}"
                        )
                    );
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
                    _actions.AddEvent(_actions.TemplateEvent(
                        Request,
                        HttpContext,
                        User,
                        $"add read access to group '{req.Name?.Replace(":", "")}' for {req.Duration}h:{id}"
                        )
                    );
                    break;
                case "write":
                    _actions.AddDocumentWriteAccess(User, group, id, (double)(req.Duration??0));
                    _actions.AddEvent(_actions.TemplateEvent(
                        Request,
                        HttpContext,
                        User,
                        $"add read & write access to group '{req.Name?.Replace(":", "")}' for {req.Duration}h:{id}"
                        )
                    );
                    break;
            }
            return HttpStatus.Ok;
        }
    }
}
