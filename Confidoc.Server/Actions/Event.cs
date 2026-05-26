using Confidoc.Server.Models;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Confidoc.Server
{
    public partial class Actions
    {
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

        public IEnumerable<Event> GetEvents()
        {
            return _context.Events;
        }

        public Event TemplateEvent(
            HttpRequest request,
            HttpContext httpContext,
            ClaimsPrincipal user,
            string action)
            => TemplateEvent(request, httpContext, GetUser(user), action);
        

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
