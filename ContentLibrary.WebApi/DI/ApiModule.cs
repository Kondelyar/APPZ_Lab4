using Autofac;
using ContentLibrary.WebAPI.Mapping;

namespace ContentLibrary.WebAPI.DI
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Реєстрація профілів маппінгу
            builder.RegisterType<ApiMappingProfile>().AsSelf();

        }
    }
}