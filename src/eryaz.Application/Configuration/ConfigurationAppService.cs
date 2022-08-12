using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using eryaz.Configuration.Dto;

namespace eryaz.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : eryazAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
