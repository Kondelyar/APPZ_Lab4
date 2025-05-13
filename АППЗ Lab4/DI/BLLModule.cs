using Autofac;
using AutoMapper;
using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.BLL.Services;

namespace ContentLibrary.UI.DI
{
    public class BLLModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Реєстрація AutoMapper профілів для BLL
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityToDtoMappingProfile>();
                cfg.AddProfile<DtoToEntityMappingProfile>();
            }))
            .AsSelf().SingleInstance();

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>()
                .SingleInstance();

            // Реєстрація сервісів
            builder.RegisterType<BookService>().As<IBookService>();
            builder.RegisterType<DocumentService>().As<IDocumentService>();
            builder.RegisterType<VideoService>().As<IVideoService>();
            builder.RegisterType<AudioService>().As<IAudioService>();
            builder.RegisterType<StorageService>().As<IStorageService>();
            builder.RegisterType<SearchService>().As<ISearchService>();
        }
    }
}