using ContentLibrary.BLL;
using ContentLibrary.DAL.Models;
using ContentLibrary.DAL.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;

// Консольний інтерфейс для роботи з бібліотекою контенту
public class ConsoleApplication
{
    private readonly IBookService _bookService;
    private readonly IDocumentService _documentService;
    private readonly IVideoService _videoService;
    private readonly IAudioService _audioService;
    private readonly IStorageService _storageService;
    private readonly ISearchService _searchService;
    private readonly ConsoleUIFactory _uiFactory;

    public ConsoleApplication(
        IBookService bookService,
        IDocumentService documentService,
        IVideoService videoService,
        IAudioService audioService,
        IStorageService storageService,
        ISearchService searchService)
    {
        _bookService = bookService;
        _documentService = documentService;
        _videoService = videoService;
        _audioService = audioService;
        _storageService = storageService;
        _searchService = searchService;
        _uiFactory = new ConsoleUIFactory();
    }

    public void Run()
    {
        bool exit = false;
        while (!exit)
        {
            var mainMenu = _uiFactory.CreateContentMenu("Бібліотека контенту", new[]
            {
                "Керування книгами",
                "Керування документами",
                "Керування відео",
                "Керування аудіо",
                "Керування сховищами",
                "Пошук",
                "Вихід"
            });

            int choice = mainMenu.Show();

            switch (choice)
            {
                case 1:
                    ManageBooks();
                    break;
                case 2:
                    ManageDocuments();
                    break;
                case 3:
                    ManageVideos();
                    break;
                case 4:
                    ManageAudios();
                    break;
                case 5:
                    ManageStorages();
                    break;
                case 6:
                    Search();
                    break;
                case 7:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void ManageBooks()
    {
        bool back = false;
        while (!back)
        {
            var menu = _uiFactory.CreateContentMenu("Керування книгами", new[]
            {
                "Переглянути всі книги",
                "Додати нову книгу",
                "Видалити книгу",
                "Пошук книг за автором",
                "Пошук книг за видавцем",
                "Пошук книги за ISBN",
                "Додати книгу до сховища",
                "Повернутися назад"
            });

            int choice = menu.Show();

            switch (choice)
            {
                case 1:
                    DisplayBooks(_bookService.GetAllContent());
                    break;
                case 2:
                    AddBook();
                    break;
                case 3:
                    DeleteBook();
                    break;
                case 4:
                    SearchBooksByAuthor();
                    break;
                case 5:
                    SearchBooksByPublisher();
                    break;
                case 6:
                    SearchBookByISBN();
                    break;
                case 7:
                    AddBookToStorage();
                    break;
                case 8:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void DisplayBooks(IEnumerable<Book> books)
    {
        Console.Clear();
        Console.WriteLine("===== Список книг =====");

        if (!books.Any())
        {
            Console.WriteLine("Книги не знайдено.");
        }
        else
        {
            foreach (var book in books)
            {
                Console.WriteLine($"ID: {book.Id}");
                Console.WriteLine($"Назва: {book.Title}");
                Console.WriteLine($"Автор: {book.Author}");
                Console.WriteLine($"Видавець: {book.Publisher}");
                Console.WriteLine($"ISBN: {book.ISBN}");
                Console.WriteLine($"Кількість сторінок: {book.PageCount}");
                Console.WriteLine($"Розмір: {book.Size} МБ");
                Console.WriteLine($"Формат: {book.Format}");
                Console.WriteLine($"Дата створення: {book.CreatedDate}");
                Console.WriteLine(new string('-', 50));
            }
        }

        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    private void AddBook()
    {
        var bookForm = _uiFactory.CreateBookInputForm();
        var book = (Book)bookForm.GetInputData();

        _bookService.AddContent(book);
        Console.WriteLine("Книгу успішно додано!");
        Console.ReadKey();
    }

    private void DeleteBook()
    {
        Console.Clear();
        Console.WriteLine("===== Видалення книги =====");
        Console.Write("Введіть ID книги для видалення: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _bookService.DeleteContent(id);
            Console.WriteLine("Книгу видалено.");
        }
        else
        {
            Console.WriteLine("Неправильний формат ID.");
        }

        Console.ReadKey();
    }

    private void SearchBooksByAuthor()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук книг за автором =====");
        Console.Write("Введіть ім'я автора: ");
        string author = Console.ReadLine() ?? string.Empty;

        var books = _bookService.GetBooksByAuthor(author);
        DisplayBooks(books);
    }

    private void SearchBooksByPublisher()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук книг за видавцем =====");
        Console.Write("Введіть назву видавця: ");
        string publisher = Console.ReadLine() ?? string.Empty;

        var books = _bookService.GetBooksByPublisher(publisher);
        DisplayBooks(books);
    }

    private void SearchBookByISBN()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук книги за ISBN =====");
        Console.Write("Введіть ISBN: ");
        string isbn = Console.ReadLine() ?? string.Empty;

        var book = _bookService.GetBookByISBN(isbn);

        if (book != null)
        {
            DisplayBooks(new[] { book });
        }
        else
        {
            Console.WriteLine("Книгу не знайдено.");
            Console.ReadKey();
        }
    }

    private void AddBookToStorage()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання книги до сховища =====");

        // Вивести список книг
        var books = _bookService.GetAllContent().ToList();
        if (books.Count == 0)
        {
            Console.WriteLine("Немає книг для додавання до сховища.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Доступні книги:");
        foreach (var book in books)
        {
            Console.WriteLine($"ID: {book.Id}, Назва: {book.Title}, Автор: {book.Author}");
        }

        Console.Write("\nВведіть ID книги: ");
        if (!int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        // Вивести список сховищ
        var storages = _storageService.GetAllStorages().ToList();
        if (storages.Count == 0)
        {
            Console.WriteLine("Немає доступних сховищ.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nДоступні сховища:");
        foreach (var storage in storages)
        {
            Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {_storageService.GetAvailableSpace(storage.Id)} ГБ");
        }

        Console.Write("\nВведіть ID сховища: ");
        if (!int.TryParse(Console.ReadLine(), out int storageId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        Console.Write("Введіть шлях до файлу в сховищі: ");
        string path = Console.ReadLine() ?? string.Empty;

        try
        {
            _bookService.AddContentToStorage(bookId, storageId, path);
            Console.WriteLine("Книгу успішно додано до сховища!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        Console.ReadKey();
    }

    private void ManageDocuments()
    {
        // Аналогічно до ManageBooks, але для документів
        bool back = false;
        while (!back)
        {
            var menu = _uiFactory.CreateContentMenu("Керування документами", new[]
            {
                "Переглянути всі документи",
                "Додати новий документ",
                "Видалити документ",
                "Пошук документів за автором",
                "Пошук документів за типом",
                "Додати документ до сховища",
                "Повернутися назад"
            });

            int choice = menu.Show();

            switch (choice)
            {
                case 1:
                    DisplayDocuments(_documentService.GetAllContent());
                    break;
                case 2:
                    AddDocument();
                    break;
                case 3:
                    DeleteDocument();
                    break;
                case 4:
                    SearchDocumentsByAuthor();
                    break;
                case 5:
                    SearchDocumentsByType();
                    break;
                case 6:
                    AddDocumentToStorage();
                    break;
                case 7:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void DisplayDocuments(IEnumerable<Document> documents)
    {
        Console.Clear();
        Console.WriteLine("===== Список документів =====");

        if (!documents.Any())
        {
            Console.WriteLine("Документи не знайдено.");
        }
        else
        {
            foreach (var document in documents)
            {
                Console.WriteLine($"ID: {document.Id}");
                Console.WriteLine($"Назва: {document.Title}");
                Console.WriteLine($"Автор: {document.Author}");
                Console.WriteLine($"Тип документа: {document.DocumentType}");
                Console.WriteLine($"Розмір: {document.Size} МБ");
                Console.WriteLine($"Формат: {document.Format}");
                Console.WriteLine($"Дата створення: {document.CreatedDate}");
                Console.WriteLine(new string('-', 50));
            }
        }

        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    private void AddDocument()
    {
        var documentForm = _uiFactory.CreateDocumentInputForm();
        var document = (Document)documentForm.GetInputData();

        _documentService.AddContent(document);
        Console.WriteLine("Документ успішно додано!");
        Console.ReadKey();
    }

    private void DeleteDocument()
    {
        Console.Clear();
        Console.WriteLine("===== Видалення документу =====");
        Console.Write("Введіть ID документу для видалення: ");

        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _documentService.DeleteContent(id);
            Console.WriteLine("Документ видалено.");
        }
        else
        {
            Console.WriteLine("Неправильний формат ID.");
        }

        Console.ReadKey();
    }

    private void SearchDocumentsByAuthor()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук документів за автором =====");
        Console.Write("Введіть ім'я автора: ");
        string author = Console.ReadLine() ?? string.Empty;

        var documents = _documentService.GetDocumentsByAuthor(author);
        DisplayDocuments(documents);
    }

    private void SearchDocumentsByType()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук документів за типом =====");
        Console.Write("Введіть тип документа: ");
        string documentType = Console.ReadLine() ?? string.Empty;

        var documents = _documentService.GetDocumentsByType(documentType);
        DisplayDocuments(documents);
    }

    private void AddDocumentToStorage()
    {
        // Аналогічно до AddBookToStorage
        Console.Clear();
        Console.WriteLine("===== Додавання документа до сховища =====");

        var documents = _documentService.GetAllContent().ToList();
        if (documents.Count == 0)
        {
            Console.WriteLine("Немає документів для додавання до сховища.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Доступні документи:");
        foreach (var document in documents)
        {
            Console.WriteLine($"ID: {document.Id}, Назва: {document.Title}, Тип: {document.DocumentType}");
        }

        Console.Write("\nВведіть ID документа: ");
        if (!int.TryParse(Console.ReadLine(), out int documentId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        var storages = _storageService.GetAllStorages().ToList();
        if (storages.Count == 0)
        {
            Console.WriteLine("Немає доступних сховищ.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nДоступні сховища:");
        foreach (var storage in storages)
        {
            Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {_storageService.GetAvailableSpace(storage.Id)} ГБ");
        }

        Console.Write("\nВведіть ID сховища: ");
        if (!int.TryParse(Console.ReadLine(), out int storageId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        Console.Write("Введіть шлях до файлу в сховищі: ");
        string path = Console.ReadLine() ?? string.Empty;

        try
        {
            _documentService.AddContentToStorage(documentId, storageId, path);
            Console.WriteLine("Документ успішно додано до сховища!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        Console.ReadKey();
    }

    // Реалізація для відео та аудіо подібна, тому я додам лише базові методи

    private void ManageVideos()
    {
        bool back = false;
        while (!back)
        {
            var menu = _uiFactory.CreateContentMenu("Керування відео", new[]
            {
                "Переглянути все відео",
                "Додати нове відео",
                "Видалити відео",
                "Пошук відео за режисером",
                "Пошук відео за роздільною здатністю",
                "Додати відео до сховища",
                "Повернутися назад"
            });

            int choice = menu.Show();

            switch (choice)
            {
                case 1:
                    DisplayVideos(_videoService.GetAllContent());
                    break;
                case 2:
                    AddVideo();
                    break;
                case 3:
                    DeleteVideo();
                    break;
                case 4:
                    SearchVideosByDirector();
                    break;
                case 5:
                    SearchVideosByResolution();
                    break;
                case 6:
                    AddVideoToStorage();
                    break;
                case 7:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void DisplayVideos(IEnumerable<Video> videos)
    {
        Console.Clear();
        Console.WriteLine("===== Список відео =====");
        if (!videos.Any())
        {
            Console.WriteLine("Відео не знайдено.");
        }
        else
        {
            foreach (var video in videos)
            {
                Console.WriteLine($"ID: {video.Id}");
                Console.WriteLine($"Назва: {video.Title}");
                Console.WriteLine($"Режисер: {video.Director}");
                Console.WriteLine($"Тривалість: {video.Duration}");
                Console.WriteLine($"Роздільна здатність: {video.Resolution}");
                Console.WriteLine($"Розмір: {video.Size} МБ");
                Console.WriteLine($"Формат: {video.Format}");
                Console.WriteLine($"Дата створення: {video.CreatedDate}");
                Console.WriteLine(new string('-', 50));
            }
        }
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    private void AddVideo()
    {
        var videoForm = _uiFactory.CreateVideoInputForm();
        var video = (Video)videoForm.GetInputData();
        _videoService.AddContent(video);
        Console.WriteLine("Відео успішно додано!");
        Console.ReadKey();
    }

    private void DeleteVideo()
    {
        Console.Clear();
        Console.WriteLine("===== Видалення відео =====");
        Console.Write("Введіть ID відео для видалення: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _videoService.DeleteContent(id);
            Console.WriteLine("Відео видалено.");
        }
        else
        {
            Console.WriteLine("Неправильний формат ID.");
        }
        Console.ReadKey();
    }

    private void SearchVideosByDirector()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук відео за режисером =====");
        Console.Write("Введіть ім'я режисера: ");
        string director = Console.ReadLine() ?? string.Empty;
        var videos = _videoService.GetVideosByDirector(director);
        DisplayVideos(videos);
    }

    private void SearchVideosByResolution()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук відео за роздільною здатністю =====");
        Console.Write("Введіть роздільну здатність (наприклад, 1920x1080): ");
        string resolution = Console.ReadLine() ?? string.Empty;
        var videos = _videoService.GetVideosByResolution(resolution);
        DisplayVideos(videos);
    }

    private void AddVideoToStorage()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання відео до сховища =====");
        var videos = _videoService.GetAllContent().ToList();
        if (videos.Count == 0)
        {
            Console.WriteLine("Немає відео для додавання до сховища.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Доступні відео:");
        foreach (var video in videos)
        {
            Console.WriteLine($"ID: {video.Id}, Назва: {video.Title}, Режисер: {video.Director}");
        }

        Console.Write("\nВведіть ID відео: ");
        if (!int.TryParse(Console.ReadLine(), out int videoId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        var storages = _storageService.GetAllStorages().ToList();
        if (storages.Count == 0)
        {
            Console.WriteLine("Немає доступних сховищ.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nДоступні сховища:");
        foreach (var storage in storages)
        {
            Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {_storageService.GetAvailableSpace(storage.Id)} ГБ");
        }

        Console.Write("\nВведіть ID сховища: ");
        if (!int.TryParse(Console.ReadLine(), out int storageId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        Console.Write("Введіть шлях до файлу в сховищі: ");
        string path = Console.ReadLine() ?? string.Empty;

        try
        {
            _videoService.AddContentToStorage(videoId, storageId, path);
            Console.WriteLine("Відео успішно додано до сховища!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
        Console.ReadKey();
    }

    private void ManageAudios()
    {
        bool back = false;
        while (!back)
        {
            var menu = _uiFactory.CreateContentMenu("Керування аудіо", new[]
            {
            "Переглянути всі аудіо",
            "Додати нове аудіо",
            "Видалити аудіо",
            "Пошук аудіо за виконавцем",
            "Пошук аудіо за тривалістю",
            "Додати аудіо до сховища",
            "Повернутися назад"
        });

            int choice = menu.Show();
            switch (choice)
            {
                case 1:
                    DisplayAudios(_audioService.GetAllContent());
                    break;
                case 2:
                    AddAudio();
                    break;
                case 3:
                    DeleteAudio();
                    break;
                case 4:
                    SearchAudiosByArtist();
                    break;
                case 5:
                    SearchAudiosByDuration();
                    break;
                case 6:
                    AddAudioToStorage();
                    break;
                case 7:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void DisplayAudios(IEnumerable<Audio> audios)
    {
        Console.Clear();
        Console.WriteLine("===== Список аудіо =====");
        if (!audios.Any())
        {
            Console.WriteLine("Аудіо не знайдено.");
        }
        else
        {
            foreach (var audio in audios)
            {
                Console.WriteLine($"ID: {audio.Id}");
                Console.WriteLine($"Назва: {audio.Title}");
                Console.WriteLine($"Виконавець: {audio.Artist}");
                Console.WriteLine($"Тривалість: {audio.Duration}");
                Console.WriteLine($"Розмір: {audio.Size} МБ");
                Console.WriteLine($"Формат: {audio.Format}");
                Console.WriteLine($"Дата створення: {audio.CreatedDate}");
                Console.WriteLine(new string('-', 50));
            }
        }
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    private void AddAudio()
    {
        var audioForm = _uiFactory.CreateAudioInputForm();
        var audio = (Audio)audioForm.GetInputData();
        _audioService.AddContent(audio);
        Console.WriteLine("Аудіо успішно додано!");
        Console.ReadKey();
    }

    private void DeleteAudio()
    {
        Console.Clear();
        Console.WriteLine("===== Видалення аудіо =====");
        Console.Write("Введіть ID аудіо для видалення: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _audioService.DeleteContent(id);
            Console.WriteLine("Аудіо видалено.");
        }
        else
        {
            Console.WriteLine("Неправильний формат ID.");
        }
        Console.ReadKey();
    }

    private void SearchAudiosByArtist()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук аудіо за виконавцем =====");
        Console.Write("Введіть ім'я виконавця: ");
        string artist = Console.ReadLine() ?? string.Empty;
        var audios = _audioService.GetAudiosByArtist(artist);
        DisplayAudios(audios);
    }

    private void SearchAudiosByDuration()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук аудіо за тривалістю =====");

        TimeSpan minDuration = TimeSpan.Zero;
        Console.Write("Введіть мінімальну тривалість (hh:mm:ss): ");
        if (!TimeSpan.TryParse(Console.ReadLine(), out minDuration))
        {
            Console.WriteLine("Неправильний формат часу. Використовується значення 00:00:00");
        }

        TimeSpan maxDuration = TimeSpan.FromHours(24);
        Console.Write("Введіть максимальну тривалість (hh:mm:ss): ");
        if (!TimeSpan.TryParse(Console.ReadLine(), out maxDuration))
        {
            Console.WriteLine("Неправильний формат часу. Використовується значення 24:00:00");
        }

        var audios = _audioService.GetAudiosByDuration(minDuration, maxDuration);
        DisplayAudios(audios);
    }

    private void AddAudioToStorage()
    {
        Console.Clear();
        Console.WriteLine("===== Додавання аудіо до сховища =====");
        var audios = _audioService.GetAllContent().ToList();
        if (audios.Count == 0)
        {
            Console.WriteLine("Немає аудіо для додавання до сховища.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Доступні аудіо:");
        foreach (var audio in audios)
        {
            Console.WriteLine($"ID: {audio.Id}, Назва: {audio.Title}, Виконавець: {audio.Artist}");
        }

        Console.Write("\nВведіть ID аудіо: ");
        if (!int.TryParse(Console.ReadLine(), out int audioId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        var storages = _storageService.GetAllStorages().ToList();
        if (storages.Count == 0)
        {
            Console.WriteLine("Немає доступних сховищ.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nДоступні сховища:");
        foreach (var storage in storages)
        {
            Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {_storageService.GetAvailableSpace(storage.Id)} ГБ");
        }

        Console.Write("\nВведіть ID сховища: ");
        if (!int.TryParse(Console.ReadLine(), out int storageId))
        {
            Console.WriteLine("Неправильний формат ID.");
            Console.ReadKey();
            return;
        }

        Console.Write("Введіть шлях до файлу в сховищі: ");
        string path = Console.ReadLine() ?? string.Empty;

        try
        {
            _audioService.AddContentToStorage(audioId, storageId, path);
            Console.WriteLine("Аудіо успішно додано до сховища!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
        Console.ReadKey();
    }

    private void ManageStorages()
    {
        bool back = false;
        while (!back)
        {
            var menu = _uiFactory.CreateContentMenu("Керування сховищами", new[]
            {
            "Переглянути всі сховища",
            "Додати нове сховище",
            "Видалити сховище",
            "Переглянути контент у сховищі",
            "Повернутися назад"
        });

            int choice = menu.Show();
            switch (choice)
            {
                case 1:
                    DisplayStorages();
                    break;
                case 2:
                    AddStorage();
                    break;
                case 3:
                    DeleteStorage();
                    break;
                case 4:
                    ViewContentInStorage();
                    break;
                case 5:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void DisplayStorages()
    {
        Console.Clear();
        Console.WriteLine("===== Список сховищ =====");
        var storages = _storageService.GetAllStorages();
        if (!storages.Any())
        {
            Console.WriteLine("Сховища не знайдено.");
        }
        else
        {
            foreach (var storage in storages)
            {
                Console.WriteLine($"ID: {storage.Id}");
                Console.WriteLine($"Назва: {storage.Name}");
                Console.WriteLine($"Тип: {storage.Type}");
                Console.WriteLine($"Розташування: {storage.Location}");
                Console.WriteLine($"Ємність: {storage.Capacity} ГБ");
                Console.WriteLine($"Використано: {storage.UsedSpace} ГБ");
                Console.WriteLine($"Вільно: {_storageService.GetAvailableSpace(storage.Id)} ГБ");
                Console.WriteLine(new string('-', 50));
            }
        }
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    private void AddStorage()
    {
        var storageForm = _uiFactory.CreateStorageInputForm();
        var storage = (Storage)storageForm.GetInputData();
        _storageService.AddStorage(storage);
        Console.WriteLine("Сховище успішно додано!");
        Console.ReadKey();
    }

    private void DeleteStorage()
    {
        Console.Clear();
        Console.WriteLine("===== Видалення сховища =====");

        var storages = _storageService.GetAllStorages();
        if (!storages.Any())
        {
            Console.WriteLine("Немає сховищ для видалення.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Доступні сховища:");
        foreach (var storage in storages)
        {
            Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}");
        }

        Console.Write("\nВведіть ID сховища для видалення: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            _storageService.DeleteStorage(id);
            Console.WriteLine("Сховище видалено.");
        }
        else
        {
            Console.WriteLine("Неправильний формат ID.");
        }
        Console.ReadKey();
    }

    private void ViewContentInStorage()
    {
        Console.Clear();
        Console.WriteLine("===== Перегляд контенту в сховищі =====");

        var storages = _storageService.GetAllStorages();
        if (!storages.Any())
        {
            Console.WriteLine("Немає доступних сховищ.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Доступні сховища:");
        foreach (var storage in storages)
        {
            Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}");
        }

        Console.Write("\nВведіть ID сховища: ");
        if (int.TryParse(Console.ReadLine(), out int storageId))
        {
            var contentItems = _storageService.GetContentInStorage(storageId);
            if (!contentItems.Any())
            {
                Console.WriteLine("У цьому сховищі немає контенту.");
            }
            else
            {
                Console.WriteLine("\nКонтент у сховищі:");
                foreach (var item in contentItems)
                {
                    Console.WriteLine($"ID: {item.Id}");
                    Console.WriteLine($"Назва: {item.Title}");
                    Console.WriteLine($"Тип: {item.GetType().Name}");
                    Console.WriteLine($"Розмір: {item.Size} МБ");
                    Console.WriteLine(new string('-', 30));
                }
            }
        }
        else
        {
            Console.WriteLine("Неправильний формат ID.");
        }
        Console.ReadKey();
    }

    private void Search()
    {
        bool back = false;
        while (!back)
        {
            var menu = _uiFactory.CreateContentMenu("Пошук", new[]
            {
            "Пошук за назвою",
            "Пошук за форматом",
            "Пошук за типом",
            "Пошук за діапазоном дат",
            "Пошук за розташуванням сховища",
            "Повернутися назад"
        });

            int choice = menu.Show();
            switch (choice)
            {
                case 1:
                    SearchByTitle();
                    break;
                case 2:
                    SearchByFormat();
                    break;
                case 3:
                    SearchByType();
                    break;
                case 4:
                    SearchByDateRange();
                    break;
                case 5:
                    SearchByStorageLocation();
                    break;
                case 6:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void SearchByTitle()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук за назвою =====");
        Console.Write("Введіть назву для пошуку: ");
        string title = Console.ReadLine() ?? string.Empty;
        var results = _searchService.SearchByTitle(title);
        DisplaySearchResults(results);
    }

    private void SearchByFormat()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук за форматом =====");
        ContentFormat format = GetEnumInput<ContentFormat>("Виберіть формат");
        var results = _searchService.SearchByFormat(format);
        DisplaySearchResults(results);
    }

    private T GetEnumInput<T>(string prompt) where T : struct, Enum
    {
        Console.WriteLine($"{prompt}:");
        var values = Enum.GetValues(typeof(T));
        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {values.GetValue(i)}");
        }
        Console.Write("Виберіть опцію: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= values.Length)
        {
            return (T)values.GetValue(choice - 1);
        }
        Console.WriteLine("Неправильний вибір. Використовуємо перше значення за замовчуванням.");
        return (T)values.GetValue(0);
    }

    private void SearchByType()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук за типом =====");
        ContentType type = GetEnumInput<ContentType>("Виберіть тип контенту");
        var results = _searchService.SearchByType(type);
        DisplaySearchResults(results);
    }

    private void SearchByDateRange()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук за діапазоном дат =====");

        DateTime startDate = DateTime.MinValue;
        Console.Write("Введіть початкову дату (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out startDate))
        {
            Console.WriteLine("Некоректний формат дати. Використовується мінімальна дата.");
        }

        DateTime endDate = DateTime.MaxValue;
        Console.Write("Введіть кінцеву дату (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out endDate))
        {
            Console.WriteLine("Некоректний формат дати. Використовується максимальна дата.");
        }

        var results = _searchService.SearchByDateRange(startDate, endDate);
        DisplaySearchResults(results);
    }

    private void SearchByStorageLocation()
    {
        Console.Clear();
        Console.WriteLine("===== Пошук за розташуванням сховища =====");
        Console.Write("Введіть розташування сховища: ");
        string location = Console.ReadLine() ?? string.Empty;
        var results = _searchService.SearchByStorageLocation(location);
        DisplaySearchResults(results);
    }

    private void DisplaySearchResults(IEnumerable<ContentItem> results)
    {
        Console.Clear();
        Console.WriteLine("===== Результати пошуку =====");
        if (!results.Any())
        {
            Console.WriteLine("За вашим запитом нічого не знайдено.");
        }
        else
        {
            Console.WriteLine($"Знайдено {results.Count()} елементів:");
            Console.WriteLine(new string('-', 50));

            foreach (var item in results)
            {
                Console.WriteLine($"ID: {item.Id}");
                Console.WriteLine($"Назва: {item.Title}");
                Console.WriteLine($"Опис: {item.Description}");
                Console.WriteLine($"Тип: {item.GetType().Name}");
                Console.WriteLine($"Розмір: {item.Size} МБ");
                Console.WriteLine($"Формат: {item.Format}");
                Console.WriteLine($"Дата створення: {item.CreatedDate}");

                // Виведення специфічних властивостей в залежності від типу
                if (item is Book book)
                {
                    Console.WriteLine($"Автор: {book.Author}");
                    Console.WriteLine($"Видавець: {book.Publisher}");
                    Console.WriteLine($"ISBN: {book.ISBN}");
                }
                else if (item is Document doc)
                {
                    Console.WriteLine($"Автор: {doc.Author}");
                    Console.WriteLine($"Тип документа: {doc.DocumentType}");
                }
                else if (item is Video video)
                {
                    Console.WriteLine($"Режисер: {video.Director}");
                    Console.WriteLine($"Тривалість: {video.Duration}");
                    Console.WriteLine($"Роздільна здатність: {video.Resolution}");
                }
                else if (item is Audio audio)
                {
                    Console.WriteLine($"Виконавець: {audio.Artist}");
                    Console.WriteLine($"Тривалість: {audio.Duration}");
                }

                Console.WriteLine(new string('-', 50));
            }
        }
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    // Фабрика для UI компонентів
    public class ConsoleUIFactory
    {
        public ContentMenu CreateContentMenu(string title, IEnumerable<string> options)
        {
            return new ContentMenu(title, options);
        }

        public BookInputForm CreateBookInputForm()
        {
            return new BookInputForm();
        }

        public DocumentInputForm CreateDocumentInputForm()
        {
            return new DocumentInputForm();
        }

        public VideoInputForm CreateVideoInputForm()
        {
            return new VideoInputForm();
        }

        public AudioInputForm CreateAudioInputForm()
        {
            return new AudioInputForm();
        }

        public StorageInputForm CreateStorageInputForm()
        {
            return new StorageInputForm();
        }
    }

}
