using Abp.Application.Services;
using eryaz.MultiTenancy.Dto;

namespace eryaz.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

