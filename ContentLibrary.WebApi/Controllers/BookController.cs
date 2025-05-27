using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Models;
using ContentLibrary.WebAPI.Models.Book;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ContentLibrary.WebAPI.Models.Storage;
using System.Threading.Tasks;

namespace ContentLibrary.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;

        public BookController(IBookService bookService, IStorageService storageService, IMapper mapper)
        {
            _bookService = bookService;
            _storageService = storageService;
            _mapper = mapper;
        }

        // GET: api/Book
        [HttpGet]
        public ActionResult<IEnumerable<BookViewModel>> Get()
        {
            var books = _bookService.GetAllContent();
            return Ok(_mapper.Map<IEnumerable<BookViewModel>>(books));
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public ActionResult<BookViewModel> Get(int id)
        {
            var book = _bookService.GetContentById(id);
            if (book == null)
                return NotFound();

            return Ok(_mapper.Map<BookViewModel>(book));
        }

        // GET: api/Book/ByAuthor/{author}
        [HttpGet("ByAuthor/{author}")]
        public ActionResult<IEnumerable<BookViewModel>> GetByAuthor(string author)
        {
            var books = _bookService.GetBooksByAuthor(author);
            return Ok(_mapper.Map<IEnumerable<BookViewModel>>(books));
        }

        // GET: api/Book/ByPublisher/{publisher}
        [HttpGet("ByPublisher/{publisher}")]
        public ActionResult<IEnumerable<BookViewModel>> GetByPublisher(string publisher)
        {
            var books = _bookService.GetBooksByPublisher(publisher);
            return Ok(_mapper.Map<IEnumerable<BookViewModel>>(books));
        }

        // GET: api/Book/ByISBN/{isbn}
        [HttpGet("ByISBN/{isbn}")]
        public ActionResult<BookViewModel> GetByISBN(string isbn)
        {
            var book = _bookService.GetBookByISBN(isbn);
            if (book == null)
                return NotFound();

            return Ok(_mapper.Map<BookViewModel>(book));
        }

        // POST: api/Book
        [HttpPost]
        public ActionResult<BookViewModel> Post([FromBody] BookCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = _mapper.Map<BookDto>(model);
            _bookService.AddContent(bookDto);

            var createdBook = _bookService.GetBookByISBN(model.ISBN);
            return CreatedAtAction(nameof(Get), new { id = createdBook.Id }, _mapper.Map<BookViewModel>(createdBook));
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] BookUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBook = _bookService.GetContentById(id);
            if (existingBook == null)
                return NotFound();

            var bookDto = _mapper.Map<BookDto>(model);
            bookDto.Id = id;
            _bookService.UpdateContent(bookDto);

            return NoContent();
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetContentById(id);
            if (book == null)
                return NotFound();

            _bookService.DeleteContent(id);
            return NoContent();
        }


        // POST: api/Book/{bookId}/Storage/{storageId}
        [HttpPost("{bookId}/Storage/{storageId}")] // Залишаємо оригінальний маршрут для сумісності
        public IActionResult AddBookToStorage(int bookId, int storageId, [FromBody] string path)
        {
            var book = _bookService.GetContentById(bookId);
            if (book == null)
                return NotFound("Книгу не знайдено");

            var storage = _storageService.GetStorageById(storageId);
            if (storage == null)
                return NotFound("Сховище не знайдено");

            try
            {
                // Перевіряємо, чи достатньо місця в сховищі
                decimal bookSizeInGB = book.Size / 1024m; // Конвертуємо МБ в ГБ
                decimal availableSpace = storage.Capacity - storage.UsedSpace;

                if (bookSizeInGB > availableSpace)
                {
                    return BadRequest($"Недостатньо місця в сховищі. Потрібно: {bookSizeInGB:F2} ГБ, доступно: {availableSpace:F2} ГБ");
                }

                _bookService.AddContentToStorage(bookId, storageId, path);

                // Отримуємо оновлені дані після збереження змін
                var updatedBook = _bookService.GetContentById(bookId);
                var storages = _bookService.GetStoragesForContent(bookId);

                return Ok(new
                {
                    message = "Книгу успішно додано до сховища",
                    book = _mapper.Map<BookViewModel>(updatedBook),
                    storages = _mapper.Map<IEnumerable<StorageViewModel>>(storages)
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Помилка при додаванні книги до сховища: {ex.Message}");
            }
        }

        // DELETE: api/Book/{bookId}/Storage/{storageId}
        [HttpDelete("{bookId}/Storage/{storageId}")]
        public IActionResult RemoveFromStorage(int bookId, int storageId)
        {
            var book = _bookService.GetContentById(bookId);
            if (book == null)
                return NotFound("Book not found");

            try
            {
                _bookService.RemoveContentFromStorage(bookId, storageId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Book/{bookId}/Storage
        [HttpGet("{bookId}/Storage")]
        public ActionResult<IEnumerable<StorageViewModel>> GetStorages(int bookId)
        {
            var book = _bookService.GetContentById(bookId);
            if (book == null)
                return NotFound("Book not found");

            var storages = _bookService.GetStoragesForContent(bookId);
            return Ok(_mapper.Map<IEnumerable<StorageViewModel>>(storages));
        }
    }
}