using Autofac;
using AutoMapper;
using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.UnitOfWork;
using NSubstitute;

namespace ContentLibrary.Tests.DI
{
    public class TestsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Реєстрація заглушок для інтерфейсів
            builder.Register(c => Substitute.For<IUnitOfWork>()).As<IUnitOfWork>().SingleInstance();

            // Реєстрація AutoMapper
            builder.Register(context => {
                var config = new MapperConfiguration(cfg => {
                    cfg.AddProfile<EntityToDtoMappingProfile>();
                    cfg.AddProfile<DtoToEntityMappingProfile>();
                });
                return config.CreateMapper();
            }).As<IMapper>().SingleInstance();

            // Реєстрація сервісів
            builder.RegisterType<BookService>().As<IBookService>();
            builder.RegisterType<SearchService>().As<ISearchService>();
            builder.RegisterType<StorageService>().As<IStorageService>();
        }
    }
}