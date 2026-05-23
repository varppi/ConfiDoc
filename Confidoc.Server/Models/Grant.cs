using System.ComponentModel.DataAnnotations;

namespace Confidoc.Server.Models
{
    public class Grant
    {
        [Key]
        public string? Id { get; set; }

        public DateTime? Ends { get; set; }
        public DateTime? Starts { get; set; }
        public ConfidocUser? Grantee { get; set; }
        public int? Level { get; set; }
        public string? Receiver { get; set; }
        public string? ReceiverType { get; set; }
        public string? ResourceType { get; set; }
        public string? ResourceId { get; set; }
    }
}
