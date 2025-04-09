using ContentLibrary.BLL;
using ContentLibrary.DAL.Models;
using ContentLibrary.DAL.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentLibrary.BLL.Services
{
    // Базовий сервіс для всіх типів контенту
    public abstract class ContentService<T> : IContentService<T> where T : ContentItem
    {
        protected readonly ContentRepository<T> _contentRepository;
        protected readonly IRepository<Storage> _storageRepository;
        protected readonly IRepository<ContentStorage> _contentStorageRepository;

        public ContentService(
            ContentRepository<T> contentRepository,
            IRepository<Storage> storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
        {
            _contentRepository = contentRepository;
            _storageRepository = storageRepository;
            _contentStorageRepository = contentStorageRepository;
        }

        public IEnumerable<T> GetAllContent()
        {
            // Жадібне завантаження контенту разом зі сховищами
            return _contentRepository.GetAllWithStorages();
        }

        public T GetContentById(int id)
        {
            // Жадібне завантаження
            return _contentRepository.GetByIdWithStorages(id);
        }

        public IEnumerable<T> FindContent(Func<T, bool> predicate)
        {
            // Жадібне завантаження і фільтрація в пам'яті
            return _contentRepository.GetAllWithStorages().Where(predicate);
        }

        public void AddContent(T content)
        {
            _contentRepository.Add(content);
            _contentRepository.Save();
        }

        public void UpdateContent(T content)
        {
            _contentRepository.Update(content);
            _contentRepository.Save();
        }

        public void DeleteContent(int id)
        {
            var content = _contentRepository.GetById(id);
            if (content != null)
            {
                _contentRepository.Delete(content);
                _contentRepository.Save();
            }
        }

        public void AddContentToStorage(int contentId, int storageId, string path)
        {
            var existing = _contentStorageRepository
                .Find(cs => cs.ContentItemId == contentId && cs.StorageId == storageId)
                .FirstOrDefault();

            if (existing == null)
            {
                var contentStorage = new ContentStorage
                {
                    ContentItemId = contentId,
                    StorageId = storageId,
                    Path = path
                };

                _contentStorageRepository.Add(contentStorage);
                _contentStorageRepository.Save();

                // Оновлення використаного простору сховища
                var content = _contentRepository.GetById(contentId);
                var storage = _storageRepository.GetById(storageId);

                if (content != null && storage != null)
                {
                    storage.UsedSpace += content.Size / 1024; // конвертуємо з МБ в ГБ
                    _storageRepository.Update(storage);
                    _storageRepository.Save();
                }
            }
        }

        public void RemoveContentFromStorage(int contentId, int storageId)
        {
            var contentStorage = _contentStorageRepository
                .Find(cs => cs.ContentItemId == contentId && cs.StorageId == storageId)
                .FirstOrDefault();

            if (contentStorage != null)
            {
                _contentStorageRepository.Delete(contentStorage);
                _contentStorageRepository.Save();

                // Оновлення використаного простору сховища
                var content = _contentRepository.GetById(contentId);
                var storage = _storageRepository.GetById(storageId);

                if (content != null && storage != null)
                {
                    storage.UsedSpace -= content.Size / 1024; // конвертуємо з МБ в ГБ
                    if (storage.UsedSpace < 0) storage.UsedSpace = 0;

                    _storageRepository.Update(storage);
                    _storageRepository.Save();
                }
            }
        }

        public IEnumerable<Storage> GetStoragesForContent(int contentId)
        {
            var contentStorages = _contentStorageRepository.Find(cs => cs.ContentItemId == contentId);
            var storageIds = contentStorages.Select(cs => cs.StorageId).ToList();

            return _storageRepository.GetAll().Where(s => storageIds.Contains(s.Id));
        }
    }

    // Сервіс для роботи з книгами
    public class BookService : ContentService<Book>, IBookService
    {
        public BookService(
            ContentRepository<Book> contentRepository,
            IRepository<Storage> storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
            : base(contentRepository, storageRepository, contentStorageRepository)
        {
        }

        public IEnumerable<Book> GetBooksByAuthor(string author)
        {
            return FindContent(b => b.Author.Contains(author));
        }

        public IEnumerable<Book> GetBooksByPublisher(string publisher)
        {
            return FindContent(b => b.Publisher.Contains(publisher));
        }

        public Book GetBookByISBN(string isbn)
        {
            return FindContent(b => b.ISBN == isbn).FirstOrDefault();
        }
    }

    // Сервіс для роботи з документами
    public class DocumentService : ContentService<Document>, IDocumentService
    {
        public DocumentService(
            ContentRepository<Document> contentRepository,
            IRepository<Storage> storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
            : base(contentRepository, storageRepository, contentStorageRepository)
        {
        }

        public IEnumerable<Document> GetDocumentsByType(string documentType)
        {
            return FindContent(d => d.DocumentType == documentType);
        }

        public IEnumerable<Document> GetDocumentsByAuthor(string author)
        {
            return FindContent(d => d.Author.Contains(author));
        }
    }

    // Сервіс для роботи з відео
    public class VideoService : ContentService<Video>, IVideoService
    {
        public VideoService(
            ContentRepository<Video> contentRepository,
            IRepository<Storage> storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
            : base(contentRepository, storageRepository, contentStorageRepository)
        {
        }

        public IEnumerable<Video> GetVideosByDirector(string director)
        {
            return FindContent(v => v.Director.Contains(director));
        }

        public IEnumerable<Video> GetVideosByResolution(string resolution)
        {
            return FindContent(v => v.Resolution == resolution);
        }
    }

    // Сервіс для роботи з аудіо
    public class AudioService : ContentService<Audio>, IAudioService
    {
        public AudioService(
            ContentRepository<Audio> contentRepository,
            IRepository<Storage> storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
            : base(contentRepository, storageRepository, contentStorageRepository)
        {
        }

        public IEnumerable<Audio> GetAudiosByArtist(string artist)
        {
            return FindContent(a => a.Artist.Contains(artist));
        }

        public IEnumerable<Audio> GetAudiosByDuration(TimeSpan minDuration, TimeSpan maxDuration)
        {
            return FindContent(a => a.Duration >= minDuration && a.Duration <= maxDuration);
        }
    }

    // Сервіс для роботи зі сховищами
    public class StorageService : IStorageService
    {
        private readonly StorageRepository _storageRepository;
        private readonly IRepository<ContentStorage> _contentStorageRepository;

        public StorageService(
            StorageRepository storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
        {
            _storageRepository = storageRepository;
            _contentStorageRepository = contentStorageRepository;
        }

        public IEnumerable<Storage> GetAllStorages()
        {
            return _storageRepository.GetAll();
        }

        public Storage GetStorageById(int id)
        {
            return _storageRepository.GetById(id);
        }

        public IEnumerable<Storage> GetStoragesByType(StorageType type)
        {
            return _storageRepository.Find(s => s.Type == type);
        }

        public void AddStorage(Storage storage)
        {
            _storageRepository.Add(storage);
            _storageRepository.Save();
        }

        public void UpdateStorage(Storage storage)
        {
            _storageRepository.Update(storage);
            _storageRepository.Save();
        }

        public void DeleteStorage(int id)
        {
            var storage = _storageRepository.GetById(id);
            if (storage != null)
            {
                _storageRepository.Delete(storage);
                _storageRepository.Save();
            }
        }

        public IEnumerable<ContentItem> GetContentInStorage(int storageId)
        {
            // Використовуємо жадібне завантаження
            var storage = _storageRepository.GetByIdWithContent(storageId);
            return storage?.ContentStorages.Select(cs => cs.ContentItem).ToList() ?? new List<ContentItem>();
        }

        public decimal GetAvailableSpace(int storageId)
        {
            var storage = _storageRepository.GetById(storageId);
            if (storage == null) return 0;

            return storage.Capacity - storage.UsedSpace;
        }
    }

    // Сервіс для пошуку
    public class SearchService : ISearchService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly IRepository<Video> _videoRepository;
        private readonly IRepository<Audio> _audioRepository;
        private readonly IRepository<Storage> _storageRepository;
        private readonly IRepository<ContentStorage> _contentStorageRepository;

        public SearchService(
            IRepository<Book> bookRepository,
            IRepository<Document> documentRepository,
            IRepository<Video> videoRepository,
            IRepository<Audio> audioRepository,
            IRepository<Storage> storageRepository,
            IRepository<ContentStorage> contentStorageRepository)
        {
            _bookRepository = bookRepository;
            _documentRepository = documentRepository;
            _videoRepository = videoRepository;
            _audioRepository = audioRepository;
            _storageRepository = storageRepository;
            _contentStorageRepository = contentStorageRepository;
        }

        public IEnumerable<ContentItem> SearchByTitle(string title)
        {
            var result = new List<ContentItem>();

            // Пошук у всіх типах контенту
            result.AddRange(_bookRepository.Find(b => b.Title.Contains(title)));
            result.AddRange(_documentRepository.Find(d => d.Title.Contains(title)));
            result.AddRange(_videoRepository.Find(v => v.Title.Contains(title)));
            result.AddRange(_audioRepository.Find(a => a.Title.Contains(title)));

            return result;
        }

        public IEnumerable<ContentItem> SearchByFormat(ContentFormat format)
        {
            var result = new List<ContentItem>();

            // Пошук у всіх типах контенту за форматом
            result.AddRange(_bookRepository.Find(b => b.Format == format));
            result.AddRange(_documentRepository.Find(d => d.Format == format));
            result.AddRange(_videoRepository.Find(v => v.Format == format));
            result.AddRange(_audioRepository.Find(a => a.Format == format));

            return result;
        }

        public IEnumerable<ContentItem> SearchByType(ContentType type)
        {
            // Залежно від типу контенту, повертаємо відповідні елементи
            switch (type)
            {
                case ContentType.Book:
                    return _bookRepository.GetAll();
                case ContentType.Document:
                    return _documentRepository.GetAll();
                case ContentType.Video:
                    return _videoRepository.GetAll();
                case ContentType.Audio:
                    return _audioRepository.GetAll();
                default:
                    return new List<ContentItem>();
            }
        }

        public IEnumerable<ContentItem> SearchByDateRange(DateTime startDate, DateTime endDate)
        {
            var result = new List<ContentItem>();

            // Пошук у всіх типах контенту за діапазоном дат
            result.AddRange(_bookRepository.Find(b => b.CreatedDate >= startDate && b.CreatedDate <= endDate));
            result.AddRange(_documentRepository.Find(d => d.CreatedDate >= startDate && d.CreatedDate <= endDate));
            result.AddRange(_videoRepository.Find(v => v.CreatedDate >= startDate && v.CreatedDate <= endDate));
            result.AddRange(_audioRepository.Find(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate));

            return result;
        }

        public IEnumerable<ContentItem> SearchByStorageLocation(string location)
        {
            // Спочатку знаходимо всі сховища за вказаною локацією
            var storages = _storageRepository.Find(s => s.Location.Contains(location));
            var storageIds = storages.Select(s => s.Id).ToList();

            // Знаходимо всі зв'язки ContentStorage для знайдених сховищ
            var contentStorages = _contentStorageRepository.Find(cs => storageIds.Contains(cs.StorageId));
            var contentIds = contentStorages.Select(cs => cs.ContentItemId).Distinct().ToList();

            var result = new List<ContentItem>();

            // Шукаємо контент за отриманими ID
            result.AddRange(_bookRepository.Find(b => contentIds.Contains(b.Id)));
            result.AddRange(_documentRepository.Find(d => contentIds.Contains(d.Id)));
            result.AddRange(_videoRepository.Find(v => contentIds.Contains(v.Id)));
            result.AddRange(_audioRepository.Find(a => contentIds.Contains(a.Id)));

            return result;
        }
    }
}