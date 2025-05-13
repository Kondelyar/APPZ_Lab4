using Autofac;
using Microsoft.Extensions.Configuration;

namespace ContentLibrary.UI.DI
{
    public static class DependencyConfig
    {
        public static IContainer Configure(IConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            // Реєстрація конфігурації
            builder.RegisterInstance(configuration).As<IConfiguration>();

            // Реєстрація модулів
            builder.RegisterModule(new DALModule(configuration));

            // Замінюємо окремі модулі маперів на інтегрований
            builder.RegisterModule(new IntegratedMapperModule());

            // Реєструємо сервіси без маперів (вони будуть використовувати інтегрований)
            builder.RegisterType<ContentLibrary.BLL.Services.BookService>().As<ContentLibrary.BLL.Interfaces.IBookService>();
            builder.RegisterType<ContentLibrary.BLL.Services.DocumentService>().As<ContentLibrary.BLL.Interfaces.IDocumentService>();
            builder.RegisterType<ContentLibrary.BLL.Services.VideoService>().As<ContentLibrary.BLL.Interfaces.IVideoService>();
            builder.RegisterType<ContentLibrary.BLL.Services.AudioService>().As<ContentLibrary.BLL.Interfaces.IAudioService>();
            builder.RegisterType<ContentLibrary.BLL.Services.StorageService>().As<ContentLibrary.BLL.Interfaces.IStorageService>();
            builder.RegisterType<ContentLibrary.BLL.Services.SearchService>().As<ContentLibrary.BLL.Interfaces.ISearchService>();

            // Реєстрація UI компонентів
            builder.RegisterType<ConsoleUIFactory>().AsSelf().SingleInstance();
            builder.RegisterType<ConsoleApplication>().AsSelf();

            return builder.Build();
        }
    }
}