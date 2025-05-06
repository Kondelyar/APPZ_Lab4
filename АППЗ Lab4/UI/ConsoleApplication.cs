using AutoMapper;
using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Models;
using ContentLibrary.DAL.Models;
using ContentLibrary.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace ContentLibrary.UI
{
    // Консольний інтерфейс з використанням View-моделей замість Entity
    public class ConsoleApplication
    {
        private readonly IBookService _bookService;
        private readonly IDocumentService _documentService;
        private readonly IVideoService _videoService;
        private readonly IAudioService _audioService;
        private readonly IStorageService _storageService;
        private readonly ISearchService _searchService;
        private readonly IMapper _mapper;
        private readonly ConsoleUIFactory _uiFactory;

        public ConsoleApplication(
            IBookService bookService,
            IDocumentService documentService,
            IVideoService videoService,
            IAudioService audioService,
            IStorageService storageService,
            ISearchService searchService,
            IMapper mapper)
        {
            _bookService = bookService;
            _documentService = documentService;
            _videoService = videoService;
            _audioService = audioService;
            _storageService = storageService;
            _searchService = searchService;
            _mapper = mapper;
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
                        DisplayBooks(GetAllBooks());
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

        private List<BookViewModel> GetAllBooks()
        {
            // Отримуємо DTO з сервісу
            var bookDtos = _bookService.GetAllContent();

            // Мапимо DTO в View-моделі
            return _mapper.Map<List<BookViewModel>>(bookDtos);
        }

        private void DisplayBooks(IEnumerable<BookViewModel> books)
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
            var bookData = (Book)bookForm.GetInputData();

            var bookViewModel = new BookViewModel
            {
                Title = bookData.Title,
                Description = bookData.Description,
                Size = bookData.Size,
                Format = bookData.Format.ToString(),
                Type = ContentType.Book.ToString(), // Додано встановлення типу
                Author = bookData.Author,
                Publisher = bookData.Publisher,
                ISBN = bookData.ISBN,
                PageCount = bookData.PageCount
            };

            var bookDto = _mapper.Map<BookDto>(bookViewModel);
            _bookService.AddContent(bookDto);

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

            // Отримуємо DTO з сервісу
            var bookDtos = _bookService.GetBooksByAuthor(author);

            // Мапимо в ViewModel
            var books = _mapper.Map<List<BookViewModel>>(bookDtos);

            DisplayBooks(books);
        }

        private void SearchBooksByPublisher()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук книг за видавцем =====");
            Console.Write("Введіть назву видавця: ");
            string publisher = Console.ReadLine() ?? string.Empty;

            // Отримуємо DTO з сервісу
            var bookDtos = _bookService.GetBooksByPublisher(publisher);

            // Мапимо в ViewModel
            var books = _mapper.Map<List<BookViewModel>>(bookDtos);

            DisplayBooks(books);
        }

        private void SearchBookByISBN()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук книги за ISBN =====");
            Console.Write("Введіть ISBN: ");
            string isbn = Console.ReadLine() ?? string.Empty;

            // Отримуємо DTO з сервісу
            var bookDto = _bookService.GetBookByISBN(isbn);

            if (bookDto != null)
            {
                // Мапимо в ViewModel
                var book = _mapper.Map<BookViewModel>(bookDto);
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
            var books = GetAllBooks();
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
            var storages = GetAllStorages();
            if (storages.Count == 0)
            {
                Console.WriteLine("Немає доступних сховищ.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nДоступні сховища:");
            foreach (var storage in storages)
            {
                Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {storage.AvailableSpace} ГБ");
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

        private List<StorageViewModel> GetAllStorages()
        {
            // Отримуємо DTO з сервісу
            var storageDtos = _storageService.GetAllStorages();

            // Мапимо DTO в View-моделі
            return _mapper.Map<List<StorageViewModel>>(storageDtos);
        }

        // Реалізація для документів
        private void ManageDocuments()
        {
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
                        DisplayDocuments(GetAllDocuments());
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

        private List<DocumentViewModel> GetAllDocuments()
        {
            var documentDtos = _documentService.GetAllContent();
            return _mapper.Map<List<DocumentViewModel>>(documentDtos);
        }

        private void DisplayDocuments(IEnumerable<DocumentViewModel> documents)
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
            var documentData = (Document)documentForm.GetInputData(); // Явне приведення типу

            var documentViewModel = new DocumentViewModel
            {
                Title = documentData.Title,
                Description = documentData.Description,
                Size = documentData.Size,
                Format = documentData.Format.ToString(),
                Type = ContentType.Document.ToString(), // Додано встановлення типу
                Author = documentData.Author,
                DocumentType = documentData.DocumentType
            };

            var documentDto = _mapper.Map<DocumentDto>(documentViewModel);
            _documentService.AddContent(documentDto);

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

            var documentDtos = _documentService.GetDocumentsByAuthor(author);
            var documents = _mapper.Map<List<DocumentViewModel>>(documentDtos);

            DisplayDocuments(documents);
        }

        private void SearchDocumentsByType()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук документів за типом =====");
            Console.Write("Введіть тип документа: ");
            string documentType = Console.ReadLine() ?? string.Empty;

            var documentDtos = _documentService.GetDocumentsByType(documentType);
            var documents = _mapper.Map<List<DocumentViewModel>>(documentDtos);

            DisplayDocuments(documents);
        }

        private void AddDocumentToStorage()
        {
            Console.Clear();
            Console.WriteLine("===== Додавання документа до сховища =====");

            var documents = GetAllDocuments();
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

            var storages = GetAllStorages();
            if (storages.Count == 0)
            {
                Console.WriteLine("Немає доступних сховищ.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nДоступні сховища:");
            foreach (var storage in storages)
            {
                Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {storage.AvailableSpace} ГБ");
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

        // Ці методи потрібно додати до класу ConsoleApplication

        private void ManageVideos()
        {
            bool back = false;
            while (!back)
            {
                var menu = _uiFactory.CreateContentMenu("Керування відео", new[]
                {
            "Переглянути всі відео",
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
                        DisplayVideos(GetAllVideos());
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

        private List<VideoViewModel> GetAllVideos()
        {
            var videoDtos = _videoService.GetAllContent();
            return _mapper.Map<List<VideoViewModel>>(videoDtos);
        }

        private void DisplayVideos(IEnumerable<VideoViewModel> videos)
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
            var videoData = (Video)videoForm.GetInputData(); // Явне приведення типу

            var videoViewModel = new VideoViewModel
            {
                Title = videoData.Title,
                Description = videoData.Description,
                Size = videoData.Size,
                Format = videoData.Format.ToString(),
                Type = ContentType.Video.ToString(), // Додано встановлення типу
                Director = videoData.Director,
                Duration = videoData.Duration,
                Resolution = videoData.Resolution
            };

            var videoDto = _mapper.Map<VideoDto>(videoViewModel);
            _videoService.AddContent(videoDto);

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

            var videoDtos = _videoService.GetVideosByDirector(director);
            var videos = _mapper.Map<List<VideoViewModel>>(videoDtos);

            DisplayVideos(videos);
        }

        private void SearchVideosByResolution()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук відео за роздільною здатністю =====");
            Console.Write("Введіть роздільну здатність (наприклад, 1920x1080): ");
            string resolution = Console.ReadLine() ?? string.Empty;

            var videoDtos = _videoService.GetVideosByResolution(resolution);
            var videos = _mapper.Map<List<VideoViewModel>>(videoDtos);

            DisplayVideos(videos);
        }

        private void AddVideoToStorage()
        {
            Console.Clear();
            Console.WriteLine("===== Додавання відео до сховища =====");

            var videos = GetAllVideos();
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

            var storages = GetAllStorages();
            if (storages.Count == 0)
            {
                Console.WriteLine("Немає доступних сховищ.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nДоступні сховища:");
            foreach (var storage in storages)
            {
                Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {storage.AvailableSpace} ГБ");
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
            "Додати аудіо до сховища",
            "Повернутися назад"
        });

                int choice = menu.Show();

                switch (choice)
                {
                    case 1:
                        DisplayAudios(GetAllAudios());
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
                        AddAudioToStorage();
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

        private List<AudioViewModel> GetAllAudios()
        {
            var audioDtos = _audioService.GetAllContent();
            return _mapper.Map<List<AudioViewModel>>(audioDtos);
        }

        private void DisplayAudios(IEnumerable<AudioViewModel> audios)
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
            var audioData = (Audio)audioForm.GetInputData(); // Явне приведення типу

            var audioViewModel = new AudioViewModel
            {
                Title = audioData.Title,
                Description = audioData.Description,
                Size = audioData.Size,
                Format = audioData.Format.ToString(),
                Type = ContentType.Audio.ToString(), // Додано встановлення типу
                Artist = audioData.Artist,
                Duration = audioData.Duration
            };

            var audioDto = _mapper.Map<AudioDto>(audioViewModel);
            _audioService.AddContent(audioDto);

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

            var audioDtos = _audioService.GetAudiosByArtist(artist);
            var audios = _mapper.Map<List<AudioViewModel>>(audioDtos);

            DisplayAudios(audios);
        }
        private void AddAudioToStorage()
        {
            Console.Clear();
            Console.WriteLine("===== Додавання аудіо до сховища =====");

            var audios = GetAllAudios();
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

            var storages = GetAllStorages();
            if (storages.Count == 0)
            {
                Console.WriteLine("Немає доступних сховищ.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nДоступні сховища:");
            foreach (var storage in storages)
            {
                Console.WriteLine($"ID: {storage.Id}, Назва: {storage.Name}, Тип: {storage.Type}, Вільно: {storage.AvailableSpace} ГБ");
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

        // Метод для роботи зі сховищами
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

            var storages = GetAllStorages();

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
                    Console.WriteLine($"Вільно: {storage.AvailableSpace} ГБ");
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }

        private void AddStorage()
        {
            var storageForm = _uiFactory.CreateStorageInputForm();
            var storageData = (Storage)storageForm.GetInputData(); // Явне приведення типу

            var storageViewModel = new StorageViewModel
            {
                Name = storageData.Name,
                Type = storageData.Type.ToString(),
                Location = storageData.Location,
                Capacity = storageData.Capacity,
                UsedSpace = 0 // Початково сховище порожнє
            };

            var storageDto = _mapper.Map<StorageDto>(storageViewModel);
            _storageService.AddStorage(storageDto);

            Console.WriteLine("Сховище успішно додано!");
            Console.ReadKey();
        }

        private void DeleteStorage()
        {
            Console.Clear();
            Console.WriteLine("===== Видалення сховища =====");

            var storages = GetAllStorages();
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

            var storages = GetAllStorages();
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
                // Отримуємо контент зі сховища через сервіс
                var contentDtos = _storageService.GetContentInStorage(storageId);

                // Мапимо DTO в ViewModel
                var contentItems = _mapper.Map<List<ContentItemViewModel>>(contentDtos);

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
                        Console.WriteLine($"Тип: {item.Type}");
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

        // Пошук контенту
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

            var contentDtos = _searchService.SearchByTitle(title);
            var contentItems = _mapper.Map<List<ContentItemViewModel>>(contentDtos);

            DisplaySearchResults(contentItems);
        }

        private void SearchByFormat()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук за форматом =====");

            // Виведення списку форматів
            Console.WriteLine("Доступні формати:");
            var formatValues = Enum.GetValues(typeof(ContentFormatDto));
            for (int i = 0; i < formatValues.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {formatValues.GetValue(i)}");
            }

            Console.Write("Виберіть формат (номер): ");
            if (int.TryParse(Console.ReadLine(), out int formatIndex) && formatIndex > 0 && formatIndex <= formatValues.Length)
            {
                var format = (ContentFormatDto)formatValues.GetValue(formatIndex - 1);
                var contentDtos = _searchService.SearchByFormat(format);
                var contentItems = _mapper.Map<List<ContentItemViewModel>>(contentDtos);

                DisplaySearchResults(contentItems);
            }
            else
            {
                Console.WriteLine("Неправильний вибір формату.");
                Console.ReadKey();
            }
        }

        private void SearchByType()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук за типом =====");

            // Виведення списку типів контенту
            Console.WriteLine("Доступні типи контенту:");
            var typeValues = Enum.GetValues(typeof(ContentTypeDto));
            for (int i = 0; i < typeValues.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {typeValues.GetValue(i)}");
            }

            Console.Write("Виберіть тип контенту (номер): ");
            if (int.TryParse(Console.ReadLine(), out int typeIndex) && typeIndex > 0 && typeIndex <= typeValues.Length)
            {
                var type = (ContentTypeDto)typeValues.GetValue(typeIndex - 1);
                var contentDtos = _searchService.SearchByType(type);
                var contentItems = _mapper.Map<List<ContentItemViewModel>>(contentDtos);

                DisplaySearchResults(contentItems);
            }
            else
            {
                Console.WriteLine("Неправильний вибір типу.");
                Console.ReadKey();
            }
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

            var contentDtos = _searchService.SearchByDateRange(startDate, endDate);
            var contentItems = _mapper.Map<List<ContentItemViewModel>>(contentDtos);

            DisplaySearchResults(contentItems);
        }

        private void SearchByStorageLocation()
        {
            Console.Clear();
            Console.WriteLine("===== Пошук за розташуванням сховища =====");
            Console.Write("Введіть розташування сховища: ");
            string location = Console.ReadLine() ?? string.Empty;

            var contentDtos = _searchService.SearchByStorageLocation(location);
            var contentItems = _mapper.Map<List<ContentItemViewModel>>(contentDtos);

            DisplaySearchResults(contentItems);
        }

        private void DisplaySearchResults(IEnumerable<ContentItemViewModel> results)
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
                    Console.WriteLine($"Тип: {item.Type}");
                    Console.WriteLine($"Розмір: {item.Size} МБ");
                    Console.WriteLine($"Формат: {item.Format}");
                    Console.WriteLine($"Дата створення: {item.CreatedDate}");

                    // Виведення специфічних властивостей в залежності від типу
                    if (item is BookViewModel book)
                    {
                        Console.WriteLine($"Автор: {book.Author}");
                        Console.WriteLine($"Видавець: {book.Publisher}");
                        Console.WriteLine($"ISBN: {book.ISBN}");
                    }
                    else if (item is DocumentViewModel doc)
                    {
                        Console.WriteLine($"Автор: {doc.Author}");
                        Console.WriteLine($"Тип документа: {doc.DocumentType}");
                    }
                    else if (item is VideoViewModel video)
                    {
                        Console.WriteLine($"Режисер: {video.Director}");
                        Console.WriteLine($"Тривалість: {video.Duration}");
                        Console.WriteLine($"Роздільна здатність: {video.Resolution}");
                    }
                    else if (item is AudioViewModel audio)
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
    }
}