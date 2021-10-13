using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace MySuperStats.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "Tektonik.1234";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string tenantName, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = $"{tenantName}_tenantadmin",
                Name = "tenantadmin",
                Surname = "tenantadmin",
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
