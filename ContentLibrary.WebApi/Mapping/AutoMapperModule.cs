using Autofac;
using AutoMapper;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.WebAPI.Mapping;
using System.Reflection;

namespace ContentLibrary.WebAPI.DI
{
    public class AutoMapperModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Регистрация MapperConfiguration
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                // API профиль
                cfg.AddProfile<ApiMappingProfile>();

                // BLL профили
                cfg.AddProfile<EntityToDtoMappingProfile>();
                cfg.AddProfile<DtoToEntityMappingProfile>();
            }))
            .AsSelf()
            .SingleInstance();

            // Регистрация IMapper
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}