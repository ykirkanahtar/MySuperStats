using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace MySuperStats.Authorization
{
    public class MySuperStatsAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"));
            
            //CustomPermissions
            context.CreatePermission(PermissionNames.Pages_Matches_Create, L("CreateMatch"));
            context.CreatePermission(PermissionNames.Pages_Matches_Update, L("UpdateMatch"));
            context.CreatePermission(PermissionNames.Pages_Matches_Delete, L("DeleteMatch"));

            context.CreatePermission(PermissionNames.Pages_Teams_Create, L("CreateTeam"));
            context.CreatePermission(PermissionNames.Pages_Teams_Update, L("UpdateTeam"));
            context.CreatePermission(PermissionNames.Pages_Teams_Delete, L("DeleteTeam"));
            
            context.CreatePermission(PermissionNames.Pages_Players_Create, L("CreatePlayer"));
            context.CreatePermission(PermissionNames.Pages_Players_Update, L("UpdatePlayer"));
            context.CreatePermission(PermissionNames.Pages_Players_Delete, L("DeletePlayer"));
                        
            context.CreatePermission(PermissionNames.Pages_Stats_Create, L("CreateStats"));
            context.CreatePermission(PermissionNames.Pages_Stats_Update, L("UpdateStats"));
            context.CreatePermission(PermissionNames.Pages_Stats_Delete, L("DeleteStats"));
            
            context.CreatePermission(PermissionNames.Pages_Players_AssignToUser, L("AssignPlayerToUser"));
            context.CreatePermission(PermissionNames.Pages_Tenants_Edit, L("EditTenant"));
            context.CreatePermission(PermissionNames.Pages_Tenants_SetPassive, L("SetTenantsPassive"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MySuperStatsConsts.LocalizationSourceName);
        }
    }
}
