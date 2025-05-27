using System.ComponentModel.DataAnnotations;
namespace ContentLibrary.WebAPI.Models.Book
{
    public class BookCreateModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Size { get; set; }

        [Required]
        public string Format { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [MaxLength(100)]
        public string Publisher { get; set; }

        [MaxLength(20)]
        public string ISBN { get; set; }

        [Range(1, int.MaxValue)]
        public int PageCount { get; set; }
    }
}
