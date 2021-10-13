using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using MySuperStats.Authorization.Accounts;
using MySuperStats.Authorization.Accounts.Dto;
using MySuperStats.Authorization.Roles;
using MySuperStats.MultiTenancy;
using MySuperStats.MultiTenancy.Dto;
using MySuperStats.Users;
using MySuperStats.Users.Dto;
using Shouldly;
using Xunit;

namespace MySuperStats.Tests.Tenants
{
    public class TenantAppService_Tests : MySuperStatsTestBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IAccountAppService _accountAppService;

        public TenantAppService_Tests()
        {
            _tenantAppService = Resolve<ITenantAppService>();
            _accountAppService = Resolve<IAccountAppService>();
        }


        [Fact]
        public async Task Create_Tenant()
        {
            //Arrange 
            await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = "Tenant1"
            });

            await UsingDbContextAsync(async context =>
            {
                var tenant = await context.Tenants.FirstOrDefaultAsync(u => u.TenancyName == "Tenant1");
                tenant.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task CreatedTenantHasStaticRoles_Test()
        {
            await _accountAppService.Register(GetNewUser());

            LoginAsTenant("Default", UserName);

            await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = "Tenant1"
            });

            await UsingDbContextAsync(async context =>
            {
                var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == "Tenant1");

                var roles = await context.Roles.Where(r => r.TenantId == tenant.Id).ToListAsync();

                roles.Count.ShouldBe(4);
            });
        }

        [Fact]
        public async Task CreatedTenantHasOwnerRoleUser_Test()
        {
            await _accountAppService.Register(GetNewUser());

            LoginAsTenant("Default", UserName);

            await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = "Tenant1"
            });

            await UsingDbContextAsync(async context =>
            {
                var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == "Tenant1");

                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == UserName);

                var tenantOwnerRole = await context.Roles.FirstOrDefaultAsync(r =>
                    r.Name == StaticRoleNames.Tenants.TenantOwner && r.TenantId == tenant.Id);

                var userRole = await context.UserRoles.FirstOrDefaultAsync(p =>
                    p.UserId == user.Id && p.RoleId == tenantOwnerRole.Id && p.TenantId == tenant.Id);

                userRole.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task GetUserTenants_Test()
        {
            //Arrange
            var userOwnerTenantName = "tenant_owner";
            var userPlayerTenantName = "tenant_player";
            var userNotInTenantName = "Tenant_absent";

            LoginAsHostAdmin();

            await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = userPlayerTenantName
            });

            await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = userNotInTenantName
            });

            await _accountAppService.Register(GetNewUser());

            LoginAsTenant("Default", UserName);


            await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = userOwnerTenantName
            });

            await UsingDbContextAsync(async context =>
            {
                var userOwnerTenant =
                    await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == userOwnerTenantName);
                var userPlayerTenant =
                    await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == userPlayerTenantName);
                var userNotInTenant =
                    await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == userNotInTenantName);

                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");

                // ownerRole
                await context.Roles.FirstOrDefaultAsync(r =>
                    r.DisplayName == StaticRoleNames.Tenants.TenantOwner && r.TenantId == userOwnerTenant.Id);

                var playerRole =
                    await context.Roles.FirstOrDefaultAsync(r =>
                        r.DisplayName == StaticRoleNames.Tenants.Player && r.TenantId == userPlayerTenant.Id);

                // playerTenantRole
                await context.UserRoles.AddAsync(new UserRole(userPlayerTenant.Id, user.Id, playerRole.Id));
                await context.SaveChangesAsync();

                var userTenants = await _tenantAppService.GetAllForSessionUserAsync();

                var userTenantsIds = userTenants.Items.Select(p => p.Id).ToList();
                var expectedTenantIds = new List<int> { userOwnerTenant.Id, userPlayerTenant.Id };
                expectedTenantIds.ShouldBeSubsetOf(userTenantsIds);
            });
        }
    }
}