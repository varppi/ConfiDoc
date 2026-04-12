using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confidoc.Server.Models
{
    public class Document
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Change>? Changes { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string? Encrypted { get; set; }
        public string? OwnerId { get; set; }
        public ConfidocUser? Owner { get; set; }
        [ForeignKey("OwnerId")]
        public ICollection<ConfidocUser>? ReadAccessUsers { get; set; }
        public ICollection<ConfidocUser>? WriteAccessUsers { get; set; }
        public ICollection<ConfidocRole>? ReadAccessGroups { get; set; }
        public ICollection<ConfidocRole>? WriteAccessGroups { get; set; }
    }
}
