using System;

namespace ContentLibrary.WebApi.Models.Storage
{
    // Модель представлення звязку між контентом и сховищем
    public class ContentStorageViewModel
    {
        // Ідентіфикатор елемента контента
        public int ContentItemId { get; set; }

        // Ідентифікатор сховища
        public int StorageId { get; set; }

        // Шлях к файлу в сховищі
        public string Path { get; set; }

        // Дата додававння
        public DateTime AddedDate { get; set; }
    }
}