using ContentLibrary.WebApi.Models.Storage;

namespace ContentLibrary.WebAPI.Models.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Size { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }
        public ICollection<ContentStorageViewModel> ContentStorages { get; set; }
    }
}