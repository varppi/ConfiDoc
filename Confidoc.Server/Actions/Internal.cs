using Microsoft.EntityFrameworkCore;

namespace Confidoc.Server
{
    public partial class Actions
    {
        /// <summary>
        /// Checks if the admin role and user exists. If not,
        /// downstream this information will be used to render
        /// an admin account setup screen.
        /// </summary>
        /// <returns></returns>
        public bool HasAdmin()
        {
            var users = _userManager.GetUsersInRoleAsync("admin")
                            .GetAwaiter()
                            .GetResult();
            var adminUser = _userManager.FindByNameAsync("admin");
            return users.Any() && adminUser is not null;
        }
    }
}
