using Abp.Authorization;
using eryaz.Authorization.Roles;
using eryaz.Authorization.Users;

namespace eryaz.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
