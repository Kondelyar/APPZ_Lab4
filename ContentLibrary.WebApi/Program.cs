using Autofac;
using Autofac.Extensions.DependencyInjection;
using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Mapping;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.DbContext;
using ContentLibrary.DAL.Patterns;
using ContentLibrary.DAL.UnitOfWork;
using ContentLibrary.WebAPI.DI;
using ContentLibrary.WebAPI.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<ContentLibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("ContentLibrary.DAL")));



// Налаштування Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Реєстрація DAL
    containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

    // Реєстрація BLL сервісів
    containerBuilder.RegisterType<BookService>().As<IBookService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<DocumentService>().As<IDocumentService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<VideoService>().As<IVideoService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<AudioService>().As<IAudioService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<StorageService>().As<IStorageService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<SearchService>().As<ISearchService>().InstancePerLifetimeScope();

    // Регистрация модуля AutoMapper
    containerBuilder.RegisterModule<AutoMapperModule>();

    // Реєстрація API модуля
    containerBuilder.RegisterModule(new ApiModule());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Додаємо підтримку статичних файлів
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();