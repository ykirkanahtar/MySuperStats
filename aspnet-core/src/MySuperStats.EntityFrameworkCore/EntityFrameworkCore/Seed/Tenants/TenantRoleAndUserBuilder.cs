using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using MySuperStats.Authorization;
using MySuperStats.Authorization.Roles;
using MySuperStats.Authorization.Users;

namespace MySuperStats.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly MySuperStatsDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(MySuperStatsDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            // Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.TenantAdmin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.TenantAdmin, StaticRoleNames.Tenants.TenantAdmin) { IsStatic = true }).Entity;
                adminRole.IsDefault = false;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new MySuperStatsAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = _tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    })
                );
                _context.SaveChanges();
            }

            // Admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == "tenantadmin");
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "tenantadmin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "Tektonik.1234");
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }
            
            // TenantOwner role

            var tenantOwnerRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.TenantOwner);
            if (tenantOwnerRole == null)
            {
                tenantOwnerRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.TenantOwner, StaticRoleNames.Tenants.TenantOwner) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }
            
            _context.Permissions.AddRange(
                 CreateRolePermissionSettings(new List<string>
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
                 }, _tenantId, tenantOwnerRole.Id)
            );
            _context.SaveChanges();

            // Editor role

            var editorRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Editor);
            if (editorRole == null)
            {
                editorRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Editor, StaticRoleNames.Tenants.Editor) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }
            
            _context.Permissions.AddRange(
                CreateRolePermissionSettings(new List<string>
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
                }, _tenantId, editorRole.Id)
            );
            _context.SaveChanges();
            
            // Player role

            var playerRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Player);
            if (playerRole == null)
            {
                playerRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Player, StaticRoleNames.Tenants.Player) { IsStatic = true }).Entity;
                playerRole.IsDefault = true;
                _context.SaveChanges();
            }
            
            _context.Permissions.AddRange(
                CreateRolePermissionSettings(new List<string>
                {
                    PermissionNames.Pages_Tenants,
                }, _tenantId, playerRole.Id)
            );
            _context.SaveChanges();
        }

        private static List<RolePermissionSetting> CreateRolePermissionSettings(List<string> permissionNames, int? tenantId, int roleId)
        {
            return permissionNames.Select(permissionName => new RolePermissionSetting { TenantId = tenantId, Name = permissionName, IsGranted = true, RoleId = roleId }).ToList();
        }
    }
}
