using Autofac;
using AutoMapper;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.UI.Mapping;

namespace ContentLibrary.UI.DI
{
    public class IntegratedMapperModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Реєстрація єдиного AutoMapper з усіма профілями
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                // DAL -> BLL маппінг
                cfg.AddProfile<EntityToDtoMappingProfile>();
                cfg.AddProfile<DtoToEntityMappingProfile>();

                // BLL -> UI маппінг
                cfg.AddProfile<DtoToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDtoMappingProfile>();
            }))
            .AsSelf().SingleInstance();

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>()
                .SingleInstance();
        }
    }
}