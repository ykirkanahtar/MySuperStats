using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Abp.Application.Services.Dto;
using MySuperStats.Authorization.Accounts;
using MySuperStats.Authorization.Accounts.Dto;
using MySuperStats.Users;
using MySuperStats.Users.Dto;

namespace MySuperStats.Tests.Users
{
    public class UserAppService_Tests : MySuperStatsTestBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IAccountAppService _accountAppService;

        public UserAppService_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
            _accountAppService = Resolve<IAccountAppService>();
        }

        [Fact]
        public async Task GetUsers_Test()
        {
            // Act
            var output = await _userAppService.GetAllAsync(new PagedUserResultRequestDto
                { MaxResultCount = 20, SkipCount = 0 });

            // Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateUser_Test()
        {
            // Act
            await _userAppService.CreateAsync(
                new CreateUserDto
                {
                    EmailAddress = "john@volosoft.com",
                    IsActive = true,
                    Name = "John",
                    Surname = "Nash",
                    Password = "123qwe",
                    UserName = "john.nash"
                });

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Register_Test()
        {
            await _accountAppService.Register(GetNewUser());

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == UserName);
                johnNashUser.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task RegisteredUserIsInDefaultTenant_Test()
        {
            await _accountAppService.Register(GetNewUser());

            await UsingDbContextAsync(async context =>
            {
                var johnNashUser =
                    await context.Users.FirstOrDefaultAsync(u => u.UserName == UserName && u.TenantId == 1);
                johnNashUser.ShouldNotBeNull();
            });
        }
    }
}