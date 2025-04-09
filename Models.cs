using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentLibrary.DAL.Models
{
  
    // Базовий клас для всіх елементів контенту
    public abstract class ContentItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Size { get; set; } // розмір в МБ

        public ContentType Type { get; set; }

        public ContentFormat Format { get; set; }

        // Навігаційна властивість для зв'язку з ContentStorage
        public virtual ICollection<ContentStorage> ContentStorages { get; set; }

        protected ContentItem()
        {
            ContentStorages = new HashSet<ContentStorage>();
            CreatedDate = DateTime.Now;
        }
    }

    // Клас для книг
    public class Book : ContentItem
    {
        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [MaxLength(100)]
        public string Publisher { get; set; }

        [MaxLength(20)]
        public string ISBN { get; set; }

        public int PageCount { get; set; }

        public Book()
        {
            Type = ContentType.Book;
        }
    }

    // Клас для документів
    public class Document : ContentItem
    {
        [MaxLength(100)]
        public string Author { get; set; }

        [MaxLength(50)]
        public string DocumentType { get; set; } // Contract, Report, etc.

        public Document()
        {
            Type = ContentType.Document;
        }
    }

    // Клас для відео
    public class Video : ContentItem
    {
        [MaxLength(100)]
        public string Director { get; set; }

        public TimeSpan Duration { get; set; }

        [MaxLength(20)]
        public string Resolution { get; set; } // e.g., "1920x1080"

        public Video()
        {
            Type = ContentType.Video;
        }
    }

    // Клас для аудіо
    public class Audio : ContentItem
    {
        [MaxLength(100)]
        public string Artist { get; set; }

        public TimeSpan Duration { get; set; }

        public Audio()
        {
            Type = ContentType.Audio;
        }
    }

    // Клас для сховищ
    public class Storage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public StorageType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Capacity { get; set; } // in GB

        [Column(TypeName = "decimal(18,2)")]
        public decimal UsedSpace { get; set; } // in GB

        // Навігаційна властивість для зв'язку з ContentStorage
        public virtual ICollection<ContentStorage> ContentStorages { get; set; }

        public Storage()
        {
            ContentStorages = new HashSet<ContentStorage>();
        }
    }

    // Проміжна таблиця для зв'язку Many-to-Many між ContentItem і Storage
    public class ContentStorage
    {
        public int ContentItemId { get; set; }
        public int StorageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Path { get; set; }

        public DateTime AddedDate { get; set; }

        // Навігаційні властивості
        public virtual ContentItem ContentItem { get; set; }
        public virtual Storage Storage { get; set; }

        public ContentStorage()
        {
            AddedDate = DateTime.Now;
        }
    }
}