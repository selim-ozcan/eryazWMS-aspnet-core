using System.Threading.Tasks;
using Abp.Application.Services;
using eryaz.Sessions.Dto;

namespace eryaz.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
