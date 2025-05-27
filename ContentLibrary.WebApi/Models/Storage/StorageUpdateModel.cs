using System.ComponentModel.DataAnnotations;

namespace ContentLibrary.WebAPI.Models.Storage
{
    // Модель для обновления существующего хранилища
    public class StorageUpdateModel
    {
        // Назва сховища
        /// <example>Оновлений диск</example>
        [Required(ErrorMessage = "Назва сховища обов'язкова")]
        [MaxLength(100, ErrorMessage = "Назва сховища не повинна перевищувати 100 символів")]
        public string Name { get; set; }

        // Тип сховища (LocalDisk, NetworkDrive, Cloud, ExternalDrive)
        /// <example>NetworkDrive</example>
        [Required(ErrorMessage = "Тип сховища обов'язковий")]
        public string Type { get; set; }

        // Розташування сховища
        /// <example>\\server\storage</example>
        [Required(ErrorMessage = "Розташування сховища обов'язкове")]
        [MaxLength(200, ErrorMessage = "Розташування сховища не повинно перевищувати 200 символів")]
        public string Location { get; set; }

        // Ємність сховища в ГБ
        /// <example>2000</example>
        [Required(ErrorMessage = "Ємність сховища обов'язкова")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ємність сховища повинна бути додатним числом")]
        public decimal Capacity { get; set; }
    }
}