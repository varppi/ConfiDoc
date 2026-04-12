using Microsoft.AspNetCore.Identity;

namespace Confidoc.Server.Models
{
    public class ConfidocUser : IdentityUser
    {
        public string? PrivateKey { get; set; }
        public string? PublicKey { get; set; }
    }
}
