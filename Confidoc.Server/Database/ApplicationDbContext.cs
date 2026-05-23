using Confidoc.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Confidoc.Server.Database
{
    public class ApplicationDbContext : IdentityDbContext<ConfidocUser, ConfidocRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Change> Changes { get; set; }
        public DbSet<Grant> Grants { get; set; }
    }
}
