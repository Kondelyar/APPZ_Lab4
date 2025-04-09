using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContentLibrary.DAL.DbContext
{
    public class ContentLibraryDbContextFactory : IDesignTimeDbContextFactory<ContentLibraryDbContext>
    {
        public ContentLibraryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContentLibraryDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ContentLibraryDb;Trusted_Connection=True;Encrypt=False;");
                                          
            return new ContentLibraryDbContext(optionsBuilder.Options);
        }
    }
}
