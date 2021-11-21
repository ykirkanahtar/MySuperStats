using System.Collections.Generic;
using Abp.Authorization;

namespace MySuperStats.MultiTenancy.Dto
{
    public class TenantWithUserPermissionsDto
    {
        public long Id { get; set; }
        public string TenancyName { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<string> PermissionNames { get; set; }
    }
}