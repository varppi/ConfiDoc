using Confidoc.Server.Helpers;
using ConfidocLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Confidoc.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        public class ScanImageForm
        {
            [Required]
            public IFormFile? Image { get; set; }
        }

        private readonly Actions _actions; 
        public ScanController(Actions actions) 
        { 
            _actions = actions;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ScanImage(ScanImageForm form)
        {
            if (form.Image is null) return HttpStatus.NotFoundAllowed;
            Image? img = null;
            using (var imageStream = form.Image.OpenReadStream())
            {
                var memStream = new MemoryStream();
                imageStream.CopyTo(memStream);
                img = Image.FromStream(memStream);
            }
            if (img is null) return HttpStatus.Internal;

            return new JsonResult(new { 
                strings = (_actions.ExtractDatamarksFromImage(User, img) ?? [])
                    .Select(str => str.Split("000").Where(s => s.Length == 8).Distinct())
                    .Select(r => r.ToList()[0])
                    .Distinct(),
            });
        }
    }
}
