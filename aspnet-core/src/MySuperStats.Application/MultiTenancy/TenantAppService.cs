﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using MySuperStats.Authorization;
using MySuperStats.Authorization.Roles;
using MySuperStats.Authorization.Users;
using MySuperStats.Editions;
using MySuperStats.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MySuperStats.MultiTenancy
{
    [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantAppService :
        AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>,
        ITenantAppService
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly ICacheManager _cacheManager;

        public TenantAppService(
            IRepository<Tenant, int> repository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            IAbpZeroDbMigrator abpZeroDbMigrator, IPermissionManager permissionManager,
            IRepository<Tenant> tenantRepository, IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IUnitOfWorkManager unitOfWorkManager, IRepository<RolePermissionSetting, long> rolePermissionRepository,
            ICacheManager cacheManager)
            : base(repository)
        {
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _permissionManager = permissionManager;
            _tenantRepository = tenantRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _rolePermissionRepository = rolePermissionRepository;
            _cacheManager = cacheManager;
        }

        public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
        {
            CheckCreatePermission();

            // Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);

            tenant.IsActive = true;
            tenant.Name = tenant.TenancyName;
            var adminEmailAddress = $"{tenant.TenancyName}.tenant@mysuperstats.com";

            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            User sessionUser;

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.MustHaveTenant))
            {
                sessionUser = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            }

            sessionUser.TenantId = tenant.Id;
            await CurrentUnitOfWork.SaveChangesAsync();

            // We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.TenantAdmin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, tenant.TenancyName, adminEmailAddress);
                await _userManager.InitializeOptionsAsync(tenant.Id);
                CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                // Assign admin user to role!
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();

                //Setting roles
                await CreateRoleWithPermissions(StaticRoleNames.Tenants.TenantOwner,
                    StaticRolePermissions.TenantOwnerPermissions);

                await CreateRoleWithPermissions(StaticRoleNames.Tenants.Editor,
                    StaticRolePermissions.EditorPermissions);

                await CreateRoleWithPermissions(StaticRoleNames.Tenants.Player,
                    StaticRolePermissions.PlayerPermissions);

                await _userManager.AddToRoleAsync(sessionUser, StaticRoleNames.Tenants.TenantOwner);
            }

            sessionUser.TenantId = 1;
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(tenant);
        }

        private async Task CreateRoleWithPermissions(string roleName, List<string> rolePermissions,
            bool addSessionUserToRole = false)
        {
            var role = _roleManager.Roles.Single(r => r.Name == roleName);
            foreach (var rolePermission in rolePermissions)
            {
                var permission = _permissionManager.GetPermission(rolePermission);
                await _roleManager.GrantPermissionAsync(role, permission);
            }

            if (addSessionUserToRole)
            {
                var sessionUser = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
                await _userManager.AddToRoleAsync(sessionUser, role.Name);
            }
        }

        protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword));
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await _tenantManager.GetByIdAsync(input.Id);
            await _tenantManager.DeleteAsync(tenant);
        }

        public async Task<PagedResultDto<TenantWithUserPermissionsDto>> GetAllForSessionUserAsync(
            PagedTenantResultRequestDto input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.MustHaveTenant))
            {
                var baseQuery = from t in _tenantRepository.GetAll()
                    join ur in _userRoleRepository.GetAll() on t.Id equals ur.TenantId
                    join r in _roleRepository.GetAll() on ur.RoleId equals r.Id
                    join rolePermission in _rolePermissionRepository.GetAll() on new { roleId = r.Id, tenantId = t.Id }
                        equals new
                        {
                            roleId = rolePermission.RoleId, tenantId = rolePermission.TenantId.GetValueOrDefault()
                        }
                    where (r.Name == StaticRoleNames.Tenants.Player || r.Name == StaticRoleNames.Tenants.TenantOwner ||
                           r.Name == StaticRoleNames.Tenants.Editor)
                          && ur.UserId == AbpSession.GetUserId() && rolePermission.IsGranted == true &&
                          t.Name != "Default"
                    group t by new
                    {
                        TenantId = t.Id,
                        TenantName = t.Name,
                        t.TenancyName,
                        t.IsActive,
                        PermissionName = rolePermission.Name,
                        RoleName = r.Name,
                    }
                    into grouping
                    select new
                    {
                        Id = grouping.Key.TenantId,
                        Name = grouping.Key.TenantName,
                        grouping.Key.TenancyName,
                        grouping.Key.IsActive,
                        grouping.Key.PermissionName,
                        grouping.Key.RoleName,
                    };

                var grouppedTenants = from p in (await baseQuery.ToListAsync())
                    group new { p.Id, p.PermissionName, p.RoleName } by new
                    {
                        TenantId = p.Id,
                        TenantName = p.Name,
                        p.TenancyName,
                        p.IsActive
                    }
                    into grouping
                    select new TenantWithUserPermissionsDto
                    {
                        Id = grouping.Key.TenantId,
                        TenancyName = grouping.Key.TenancyName,
                        Name = grouping.Key.TenantName,
                        IsActive = grouping.Key.IsActive,
                        RoleName = grouping.Select(p => p.RoleName).Distinct().Single(),
                        PermissionNames = grouping.Select(p => p.PermissionName).Distinct()
                    };

                var filteredGrouppedTenants = grouppedTenants.Where(p =>
                    p.PermissionNames.Contains(PermissionNames.Pages_Tenants_SetPassive) || p.IsActive);

                var tenantsWithPermissions = filteredGrouppedTenants.AsQueryable()
                    .OrderBy(input.Sorting ?? $"{nameof(TenantDto.TenancyName)} ASC")
                    .PageBy(input)
                    .ToList();

                return new PagedResultDto<TenantWithUserPermissionsDto>(filteredGrouppedTenants.Count(),
                    ObjectMapper.Map<List<TenantWithUserPermissionsDto>>(tenantsWithPermissions));
            }
        }

        public async Task LoginToTenant(int tenantId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.MustHaveTenant))
            {
                var tenant = await _tenantManager.GetByIdAsync(tenantId);
                var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());

                AbpSession.Use(tenant.Id, user.Id);
                user.TenantId = tenant.Id;

                await _cacheManager.GetUserSettingsCache().ClearAsync();
                await _cacheManager.GetRolePermissionCache().ClearAsync();
            }
        }

        public async Task LeaveTenant(int tenantId)
        {
            await RemoveUserFromTenant(tenantId, AbpSession.GetUserId());
        }

        public async Task RemoveUserFromTenant(int tenantId, long userId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.MustHaveTenant))
            {
                var tenant = await _tenantManager.GetByIdAsync(tenantId);
                var user = await _userManager.GetUserByIdAsync(userId);

                if (userId == AbpSession.GetUserId() && user.TenantId == tenant.Id)
                {
                    AbpSession.Use(1, user.Id);
                    user.TenantId = 1;

                    await _cacheManager.GetUserSettingsCache().ClearAsync();
                    await _cacheManager.GetRolePermissionCache().ClearAsync();
                }

                var userRole = await _userRoleRepository.GetAll()
                    .Where(p => p.UserId == user.Id && p.TenantId == tenant.Id).SingleAsync();

                await _userRoleRepository.DeleteAsync(userRole);
            }
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}