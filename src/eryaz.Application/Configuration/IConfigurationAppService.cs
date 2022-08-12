using System.Threading.Tasks;
using eryaz.Configuration.Dto;

namespace eryaz.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
