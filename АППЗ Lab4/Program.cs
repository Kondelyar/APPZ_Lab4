using ContentLibrary.BLL;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.DbContext;
using ContentLibrary.DAL.Models;
using ContentLibrary.DAL.Patterns;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace ContentLibrary.UI
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider = null!;
        private static IConfiguration _configuration = null!;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Console.InputEncoding = System.Text.Encoding.UTF8;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.InputEncoding = Encoding.GetEncoding("Windows-1251");

            ConfigureAppSettings();
            ConfigureServices();

            var app = _serviceProvider.GetService<ConsoleApplication>();
            if (app != null)
            {
                app.Run();
            }
            else
            {
                Console.WriteLine("Failed to initialize ConsoleApplication.");
            }

            DisposeServices();
        }

        private static void ConfigureAppSettings()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Подключаем DbContext
            services.AddDbContext<ContentLibraryDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            // Регистрируем generic-репозитории
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Регистрируем специальные репозитории, если есть
            services.AddScoped<ContentRepository<Book>>();
            services.AddScoped<ContentRepository<Document>>();
            services.AddScoped<ContentRepository<Video>>();
            services.AddScoped<ContentRepository<Audio>>();
            services.AddScoped<StorageRepository>();

            // Регистрируем сервисы
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IAudioService, AudioService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ISearchService, SearchService>();

            // Регистрируем консольное приложение
            services.AddScoped<ConsoleApplication>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
