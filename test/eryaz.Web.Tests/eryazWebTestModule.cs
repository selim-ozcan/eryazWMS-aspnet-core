using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using eryaz.EntityFrameworkCore;
using eryaz.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace eryaz.Web.Tests
{
    [DependsOn(
        typeof(eryazWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class eryazWebTestModule : AbpModule
    {
        public eryazWebTestModule(eryazEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(eryazWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(eryazWebMvcModule).Assembly);
        }
    }
}