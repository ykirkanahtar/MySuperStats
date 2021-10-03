using Abp.MultiTenancy;
using Abp.Zero.Configuration;

namespace MySuperStats.Authorization.Roles
{
    public static class AppRoleConfig
    {
        public static void Configure(IRoleManagementConfig roleManagementConfig)
        {
            // Static host roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Host.Admin,
                    MultiTenancySides.Host
                )
            );

            // Static tenant roles

            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.TenantAdmin,
                    MultiTenancySides.Tenant
                )
            );
            
            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.TenantOwner,
                    MultiTenancySides.Tenant
                )
            );
            
            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.Editor,
                    MultiTenancySides.Tenant
                )
            );
            
            roleManagementConfig.StaticRoles.Add(
                new StaticRoleDefinition(
                    StaticRoleNames.Tenants.Player,
                    MultiTenancySides.Tenant
                )
            );
        }
    }
}
