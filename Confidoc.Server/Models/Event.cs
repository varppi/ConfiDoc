using System.ComponentModel.DataAnnotations;

namespace Confidoc.Server.Models
{
    public class Event
    {
        [Key]
        public string? Id { get; set; }
        public string? User { get; set; }
        public string? UserAgent { get; set; }
        public string? Ip { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? Action { get; set; }
    }
}
