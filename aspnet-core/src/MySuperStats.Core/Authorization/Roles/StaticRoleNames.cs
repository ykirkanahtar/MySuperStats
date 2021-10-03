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
}
