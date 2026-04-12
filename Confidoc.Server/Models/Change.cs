using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confidoc.Server.Models
{
    public class Change
    {
        [Key]
        public string? Id { get; set; }
        public string? Patch { get; set; }
        public string? Signature { get; set; }
        [ForeignKey("DocumentId")]
        public string? DocumentId { get; set; }
        public string? OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ConfidocUser? Owner { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
