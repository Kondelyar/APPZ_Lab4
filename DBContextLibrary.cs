using ContentLibrary.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContentLibrary.DAL.DbContext
{
    public class ContentLibraryDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;
        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<Storage> Storages { get; set; } = null!;
        public DbSet<ContentStorage> ContentStorages { get; set; } = null!;

        public ContentLibraryDbContext(DbContextOptions<ContentLibraryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфігурація для ContentItem (абстрактний клас)
            modelBuilder.Entity<ContentItem>()
                .HasDiscriminator<ContentType>("Type")
                .HasValue<Book>(ContentType.Book)
                .HasValue<Document>(ContentType.Document)
                .HasValue<Video>(ContentType.Video)
                .HasValue<Audio>(ContentType.Audio);

            // Конфігурація для ContentStorage (зв'язок Many-to-Many)
            modelBuilder.Entity<ContentStorage>()
                .HasKey(cs => new { cs.ContentItemId, cs.StorageId });

            modelBuilder.Entity<ContentStorage>()
                .HasOne(cs => cs.ContentItem)
                .WithMany(c => c.ContentStorages)
                .HasForeignKey(cs => cs.ContentItemId);

            modelBuilder.Entity<ContentStorage>()
                .HasOne(cs => cs.Storage)
                .WithMany(s => s.ContentStorages)
                .HasForeignKey(cs => cs.StorageId);
        }
    }
}