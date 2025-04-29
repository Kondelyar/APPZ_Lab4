using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.DbContext;
using ContentLibrary.DAL.UnitOfWork;
using ContentLibrary.UI.Mapping;
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
                Console.WriteLine("Помилка ініціалізації додатку.");
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

            // Підключення DbContext
            services.AddDbContext<ContentLibraryDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            // Реєстрація UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Реєстрація AutoMapper
            services.AddAutoMapper(cfg =>
            {
                // Профілі для мапінгу між Entity і DTO (DAL -> BLL)
                cfg.AddProfile<EntityToDtoMappingProfile>();
                cfg.AddProfile<DtoToEntityMappingProfile>();

                // Профілі для мапінгу між DTO і ViewModel (BLL -> UI)
                cfg.AddProfile<DtoToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDtoMappingProfile>();
            });

            // Реєстрація сервісів
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IAudioService, AudioService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ISearchService, SearchService>();

            // Реєстрація консольного додатку
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