using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Confidoc.Server.Models
{
    public class ConfidocRole : IdentityRole
    {
        public string? DisplayName { get; set; }
        public string? OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public ConfidocUser? Owner { get; set; }
    }
}
