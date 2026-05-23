using Confidoc.Server.Models;
using Microsoft.AspNetCore.Identity;
using Confidoc.Server.Database;

namespace Confidoc.Server;

public partial class Actions
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ConfidocUser> _userManager;
    private readonly RoleManager<ConfidocRole> _roleManager;

    public async void Initialize()
    {
        try
        {
            await _roleManager.CreateAsync(new ConfidocRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "admin"
            });
            _context.SaveChanges();
        }catch { }
    }

    public Actions(
        ApplicationDbContext context,
        UserManager<ConfidocUser> userManager,
        RoleManager<ConfidocRole> roleManager
        )
    {
        _context     = context;
        _userManager = userManager;
        _roleManager = roleManager;
        Initialize();
    }
}

public static class ActionService
{
    public static void AddConfidocActions(this IServiceCollection services)
    {
        services.AddScoped<Actions>();
    }
}

