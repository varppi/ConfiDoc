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
    public class CryptoController : ControllerBase
    {

        private readonly Actions _actions;

        public CryptoController(
            Actions actions)
        {
            _actions = actions;
        }

        public class HashRequest
        {
            public string? Text { get; set; }
        }

        [Route("hash")]
        [HttpPost]
        public async Task<IActionResult> HashText([FromBody] HashRequest req)
            => new JsonResult(new
            {
                Hash=Security.HashString(req.Text??"")
            });
    }
}
