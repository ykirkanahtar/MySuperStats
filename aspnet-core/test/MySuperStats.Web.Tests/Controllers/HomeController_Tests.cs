using System.Threading.Tasks;
using MySuperStats.Models.TokenAuth;
using MySuperStats.Web.Controllers;
using Shouldly;
using Xunit;

namespace MySuperStats.Web.Tests.Controllers
{
    public class HomeController_Tests: MySuperStatsWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}