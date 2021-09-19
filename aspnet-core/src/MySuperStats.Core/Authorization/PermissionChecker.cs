using Abp.Authorization;
using MySuperStats.Authorization.Roles;
using MySuperStats.Authorization.Users;

namespace MySuperStats.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
