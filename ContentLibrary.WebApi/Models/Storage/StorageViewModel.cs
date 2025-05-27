using ContentLibrary.WebApi.Models.Storage;
using System.Collections.Generic;

namespace ContentLibrary.WebAPI.Models.Storage
{
    // Модель представлення сховища
    public class StorageViewModel
    {
        // Ідентифікатор сховища
        public int Id { get; set; }

        // Назва сховища
        public string Name { get; set; }

        // Тип сховища
        public string Type { get; set; }

        // Розташування сховища
        public string Location { get; set; }

        // Загальна ємність сховища в ГБ
        public decimal Capacity { get; set; }

        // Використаний простір в ГБ
        public decimal UsedSpace { get; set; }

        // Доступний простір в ГБ
        public decimal AvailableSpace { get; set; }

        // Зв'язки з контентом
        public ICollection<ContentStorageViewModel> ContentStorages { get; set; }
    }
}