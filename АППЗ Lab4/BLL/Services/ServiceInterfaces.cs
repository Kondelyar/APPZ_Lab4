using ContentLibrary.DAL.Models;
using System;
using System.Collections.Generic;

namespace ContentLibrary.BLL
{
    // Базовий інтерфейс для сервісів контенту
    public interface IContentService<T> where T : ContentItem
    {
        IEnumerable<T> GetAllContent();
        T GetContentById(int id);
        IEnumerable<T> FindContent(Func<T, bool> predicate);
        void AddContent(T content);
        void UpdateContent(T content);
        void DeleteContent(int id);
        void AddContentToStorage(int contentId, int storageId, string path);
        void RemoveContentFromStorage(int contentId, int storageId);
        IEnumerable<Storage> GetStoragesForContent(int contentId);
    }

    // Інтерфейс для сервісу книг
    public interface IBookService : IContentService<Book>
    {
        IEnumerable<Book> GetBooksByAuthor(string author);
        IEnumerable<Book> GetBooksByPublisher(string publisher);
        Book GetBookByISBN(string isbn);
    }

    // Інтерфейс для сервісу документів
    public interface IDocumentService : IContentService<Document>
    {
        IEnumerable<Document> GetDocumentsByType(string documentType);
        IEnumerable<Document> GetDocumentsByAuthor(string author);
    }

    // Інтерфейс для сервісу відео
    public interface IVideoService : IContentService<Video>
    {
        IEnumerable<Video> GetVideosByDirector(string director);
        IEnumerable<Video> GetVideosByResolution(string resolution);
    }

    // Інтерфейс для сервісу аудіо
    public interface IAudioService : IContentService<Audio>
    {
        IEnumerable<Audio> GetAudiosByArtist(string artist);
        IEnumerable<Audio> GetAudiosByDuration(TimeSpan minDuration, TimeSpan maxDuration);
    }

    // Інтерфейс для сервісу сховищ
    public interface IStorageService
    {
        IEnumerable<Storage> GetAllStorages();
        Storage GetStorageById(int id);
        IEnumerable<Storage> GetStoragesByType(StorageType type);
        void AddStorage(Storage storage);
        void UpdateStorage(Storage storage);
        void DeleteStorage(int id);
        IEnumerable<ContentItem> GetContentInStorage(int storageId);
        decimal GetAvailableSpace(int storageId);
    }

    // Інтерфейс для сервісу пошуку
    public interface ISearchService
    {
        IEnumerable<ContentItem> SearchByTitle(string title);
        IEnumerable<ContentItem> SearchByFormat(ContentFormat format);
        IEnumerable<ContentItem> SearchByType(ContentType type);
        IEnumerable<ContentItem> SearchByDateRange(DateTime startDate, DateTime endDate);
        IEnumerable<ContentItem> SearchByStorageLocation(string location);
    }
}
