using System.Collections.Generic;

namespace MySuperStats.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string TenantAdmin = "TenantAdmin";
            public const string TenantOwner = "TenantOwner";
            public const string Editor = "Editor";
            public const string Player = "Player";
        }
    }

    public static class StaticRolePermissions
    {
        static StaticRolePermissions()
        {
            TenantOwnerPermissions = new List<string>
            {
                PermissionNames.Pages_Tenants,
                PermissionNames.Pages_Tenants_SetPassive,

                PermissionNames.Pages_Matches_Create,
                PermissionNames.Pages_Matches_Update,
                PermissionNames.Pages_Matches_Delete,

                PermissionNames.Pages_Teams_Create,
                PermissionNames.Pages_Teams_Update,
                PermissionNames.Pages_Teams_Delete,

                PermissionNames.Pages_Players_Create,
                PermissionNames.Pages_Players_Update,
                PermissionNames.Pages_Players_Delete,

                PermissionNames.Pages_Stats_Create,
                PermissionNames.Pages_Stats_Update,
                PermissionNames.Pages_Stats_Delete,

                PermissionNames.Pages_Players_AssignToUser
            };

            EditorPermissions = new List<string>
            {
                PermissionNames.Pages_Tenants,
                PermissionNames.Pages_Tenants_SetPassive,

                PermissionNames.Pages_Matches_Create,
                PermissionNames.Pages_Matches_Update,
                PermissionNames.Pages_Matches_Delete,

                PermissionNames.Pages_Teams_Create,
                PermissionNames.Pages_Teams_Update,
                PermissionNames.Pages_Teams_Delete,

                PermissionNames.Pages_Players_Create,
                PermissionNames.Pages_Players_Update,
                PermissionNames.Pages_Players_Delete,

                PermissionNames.Pages_Stats_Create,
                PermissionNames.Pages_Stats_Update,
                PermissionNames.Pages_Stats_Delete,

                PermissionNames.Pages_Players_AssignToUser
            };

            PlayerPermissions = new List<string>
            {
                PermissionNames.Pages_Tenants,
            };
        }

        public static List<string> TenantOwnerPermissions { get; }
        public static List<string> EditorPermissions { get; }
        public static List<string> PlayerPermissions { get; }
    }
}