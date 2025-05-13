using Autofac;
using AutoMapper;
using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.Patterns;
using ContentLibrary.DAL.UnitOfWork;
using NSubstitute;

namespace Lab6.BLL.Tests.DI
{
    public static class TestModule
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            // Реєстрація мок-об'єктів для залежностей
            var unitOfWork = Substitute.For<IUnitOfWork>();
            builder.RegisterInstance(unitOfWork).As<IUnitOfWork>().SingleInstance();

            // Створення мок-об'єктів для репозиторіїв
            var bookRepository = Substitute.For<IContentRepository<ContentLibrary.DAL.Models.Book>>();
            var documentRepository = Substitute.For<IContentRepository<ContentLibrary.DAL.Models.Document>>();
            var videoRepository = Substitute.For<IContentRepository<ContentLibrary.DAL.Models.Video>>();
            var audioRepository = Substitute.For<IContentRepository<ContentLibrary.DAL.Models.Audio>>();
            var storageRepository = Substitute.For<IStorageRepository>();
            var contentStorageRepository = Substitute.For<IRepository<ContentLibrary.DAL.Models.ContentStorage>>();

            // Налаштування unitOfWork для повернення репозиторіїв
            unitOfWork.GetContentRepository<ContentLibrary.DAL.Models.Book>().Returns(bookRepository);
            unitOfWork.GetContentRepository<ContentLibrary.DAL.Models.Document>().Returns(documentRepository);
            unitOfWork.GetContentRepository<ContentLibrary.DAL.Models.Video>().Returns(videoRepository);
            unitOfWork.GetContentRepository<ContentLibrary.DAL.Models.Audio>().Returns(audioRepository);
            unitOfWork.GetStorageRepository().Returns(storageRepository);
            unitOfWork.GetRepository<ContentLibrary.DAL.Models.ContentStorage>().Returns(contentStorageRepository);

            // Реєстрація AutoMapper
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityToDtoMappingProfile>();
                cfg.AddProfile<DtoToEntityMappingProfile>();
            })).AsSelf().SingleInstance();

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

            return builder.Build();
        }
    }
}