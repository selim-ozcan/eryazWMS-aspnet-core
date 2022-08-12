using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using eryaz.Authorization;

namespace eryaz
{
    [DependsOn(
        typeof(eryazCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class eryazApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<eryazAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(eryazApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
