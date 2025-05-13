using Autofac;
using ContentLibrary.DAL.DbContext;
using ContentLibrary.DAL.Patterns;
using ContentLibrary.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ContentLibrary.UI.DI
{
    public class DALModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public DALModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Реєстрація DbContext
            builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ContentLibraryDbContext>();
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                return new ContentLibraryDbContext(optionsBuilder.Options);
            })
            .AsSelf()
            .InstancePerLifetimeScope();

            // Реєстрація UnitOfWork
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            // Реєстрація репозиторіїв
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterGeneric(typeof(ContentRepository<>)).As(typeof(IContentRepository<>));
            builder.RegisterType<StorageRepository>().As<IStorageRepository>();
        }
    }
}