using System.Threading.Tasks;
using eryaz.Models.TokenAuth;
using eryaz.Web.Controllers;
using Shouldly;
using Xunit;

namespace eryaz.Web.Tests.Controllers
{
    public class HomeController_Tests: eryazWebTestBase
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