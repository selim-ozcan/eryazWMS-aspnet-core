using System.Threading.Tasks;
using Abp.Application.Services;
using eryaz.Authorization.Accounts.Dto;

namespace eryaz.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
