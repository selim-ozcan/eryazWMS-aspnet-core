using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace eryaz.Controllers
{
    public abstract class eryazControllerBase: AbpController
    {
        protected eryazControllerBase()
        {
            LocalizationSourceName = eryazConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
