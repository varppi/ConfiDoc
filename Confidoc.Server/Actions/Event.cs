using Confidoc.Server.Models;
using ConfidocLib;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Confidoc.Server
{
    public partial class Actions
    {
        /// <summary>
        /// Extracts the binary representation of the download
        /// event ID from a screenshot containing the mark.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public IEnumerable<string>? ExtractDatamarksFromImage(
            ClaimsPrincipal user, 
            Image image)
        {
            if (user is null) return null;
            var extracted = Pdf.ExtractDatamark(image);
            return extracted;
        }

        /// <summary>
        /// Generates 8 long unique ID for an event.
        /// </summary>
        /// <returns></returns>
        public string GenerateEventId()
        {
            using (var sha256Hash = SHA256.Create())
                return string.Join("", Convert.ToHexString(
                    sha256Hash.ComputeHash(
                        Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
                    )
                ).Take(8));
        }

        /// <summary>
        /// Adds the provided event into database.
        /// </summary>
        /// <param name="eve"></param>
        public string AddEvent(Event eve) 
        {
            if (eve.Id is null) eve.Id = GenerateEventId();
            Serilog.Log.Information($"[{eve.Action}] {eve.User} {eve.Ip} {eve.UserAgent}");
            _context.Events.Add(eve);
            _context.SaveChanges();
            return eve.Id;
        }

        /// <summary>
        /// Verifies the user owns the document the event he is trying to view
        /// is tied to and returns the content of the event if the user does 
        /// indeed own the document.
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ParsedEvent? GetEvent(ClaimsPrincipal claim, string id)
        {
            var eve = _context.Events.FirstOrDefault(e => e.Id == id);
            if (eve is null) return null;
            var actionsSections = (eve.Action ?? ":").Split(":");
            if (actionsSections.Length != 2) return null;
            if (DocumentAccessLevel(claim, actionsSections[1]) < 3) return null;
            return ToParsedEvent(eve);
        }

        /// <summary>
        /// Returns all events 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Event> GetEvents()
        {
            return _context.Events;
        }

        /// <summary>
        /// Creates a template encompassing user identifiers.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpContext"></param>
        /// <param name="user"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Event TemplateEvent(
            HttpRequest request,
            HttpContext httpContext,
            ClaimsPrincipal user,
            string action)
            => TemplateEvent(request, httpContext, GetUser(user), action);
        

        /// <summary>
        /// Creates a template encompassing user identifiers.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpContext"></param>
        /// <param name="user"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Event TemplateEvent(
            HttpRequest request, 
            HttpContext httpContext, 
            ConfidocUser? user,
            string action)
        {
            return new Event
            {
                Id = GenerateEventId(),
                Timestamp = DateTime.UtcNow,
                Action = action,
                Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0",
                User = user?.UserName ?? "<not logged in>",
                UserAgent = request.Headers.UserAgent,
            };
        }
    }
}
