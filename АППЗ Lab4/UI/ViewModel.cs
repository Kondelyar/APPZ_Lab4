using System;
using System.Collections.Generic;

namespace ContentLibrary.UI.ViewModels
{
    // Базова View-модель для ContentItem
    public abstract class ContentItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Size { get; set; } // розмір в МБ
        public string Type { get; set; }
        public string Format { get; set; }
    }

    // View-модель для книги
    public class BookViewModel : ContentItemViewModel
    {
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }
    }

    // View-модель для документа
    public class DocumentViewModel : ContentItemViewModel
    {
        public string Author { get; set; }
        public string DocumentType { get; set; } // Contract, Report, etc.
    }

    // View-модель для відео
    public class VideoViewModel : ContentItemViewModel
    {
        public string Director { get; set; }
        public TimeSpan Duration { get; set; }
        public string Resolution { get; set; } // e.g., "1920x1080"
    }

    // View-модель для аудіо
    public class AudioViewModel : ContentItemViewModel
    {
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
    }

    // View-модель для сховища
    public class StorageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public decimal Capacity { get; set; } // in GB
        public decimal UsedSpace { get; set; } // in GB
        public decimal AvailableSpace => Capacity - UsedSpace; // in GB
    }

    // View-модель для зв'язку між контентом і сховищем
    public class ContentStorageViewModel
    {
        public int ContentItemId { get; set; }
        public string ContentTitle { get; set; }
        public int StorageId { get; set; }
        public string StorageName { get; set; }
        public string Path { get; set; }
        public DateTime AddedDate { get; set; }
    }

    // View-модель для результатів пошуку
    public class SearchResultViewModel
    {
        public List<ContentItemViewModel> Results { get; set; } = new List<ContentItemViewModel>();
        public int TotalCount => Results.Count;
        public string SearchTerm { get; set; }
        public string SearchType { get; set; }
    }
}