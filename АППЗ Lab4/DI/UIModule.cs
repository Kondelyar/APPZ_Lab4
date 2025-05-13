using Autofac;
using AutoMapper;
using ContentLibrary.UI.Mapping;

namespace ContentLibrary.UI.DI
{
    public class UIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Реєстрація AutoMapper профілів для UI
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDtoMappingProfile>();
            }))
            .AsSelf().SingleInstance();

            // Реєстрація ConsoleUIFactory
            builder.RegisterType<ConsoleUIFactory>().AsSelf().SingleInstance();

            // Реєстрація ConsoleApplication
            builder.RegisterType<ConsoleApplication>().AsSelf();
        }
    }
}