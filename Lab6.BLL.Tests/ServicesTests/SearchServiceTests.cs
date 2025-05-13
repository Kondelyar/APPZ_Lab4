using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Models;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.Models;
using ContentLibrary.DAL.Patterns;
using ContentLibrary.DAL.UnitOfWork;
using ContentLibrary.Tests.TestHelpers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace ContentLibrary.Tests
{
    [TestFixture]
    public class SearchServiceTests : TestBase, IDisposable
    {
        private ISearchService _searchService;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IContentRepository<Book> _bookRepository;
        private IContentRepository<Document> _documentRepository;
        private IContentRepository<Video> _videoRepository;
        private IContentRepository<Audio> _audioRepository;
        private IRepository<ContentStorage> _contentStorageRepository;
        private IStorageRepository _storageRepository;
        private bool _disposed = false;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Створення заглушок
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IContentRepository<Book>>();
            _documentRepository = Substitute.For<IContentRepository<Document>>();
            _videoRepository = Substitute.For<IContentRepository<Video>>();
            _audioRepository = Substitute.For<IContentRepository<Audio>>();
            _contentStorageRepository = Substitute.For<IRepository<ContentStorage>>();
            _storageRepository = Substitute.For<IStorageRepository>();

            // Налаштування UnitOfWork
            _unitOfWork.GetContentRepository<Book>().Returns(_bookRepository);
            _unitOfWork.GetContentRepository<Document>().Returns(_documentRepository);
            _unitOfWork.GetContentRepository<Video>().Returns(_videoRepository);
            _unitOfWork.GetContentRepository<Audio>().Returns(_audioRepository);
            _unitOfWork.GetRepository<ContentStorage>().Returns(_contentStorageRepository);
            _unitOfWork.GetStorageRepository().Returns(_storageRepository);

            // Створення сервісу
            _searchService = new SearchService(_unitOfWork, _mapper);
        }

        [TearDown]
        public override void Cleanup()
        {
            Dispose(true);
            base.Cleanup();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_unitOfWork != null)
                    {
                        _unitOfWork.Dispose();
                        _unitOfWork = null;
                    }
                }
                _disposed = true;
            }
        }

        [Test]
        public void SearchByTitle_ShouldReturnContentWithMatchingTitle()
        {
            // Arrange
            string searchTitle = "Test";

            var books = new List<Book>();
            var documents = new List<Document>();
            var videos = new List<Video>();
            var audios = new List<Audio>();

            var contentDtos = new List<ContentItemDto> {
                new BookDto { Id = 1, Title = "Test Book" },
                new DocumentDto { Id = 2, Title = "Test Document" },
                new VideoDto { Id = 3, Title = "Test Video" },
                new AudioDto { Id = 4, Title = "Test Audio" }
            };

            // Налаштування репозиторіїв
            _bookRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>()).Returns(books);
            _documentRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>()).Returns(documents);
            _videoRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>()).Returns(videos);
            _audioRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>()).Returns(audios);

            _mapper.Map<IEnumerable<ContentItemDto>>(Arg.Any<IEnumerable<ContentItem>>()).Returns(contentDtos);

            // Act
            var result = _searchService.SearchByTitle(searchTitle);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(contentDtos.Count));
            _bookRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>());
            _documentRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>());
            _videoRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>());
            _audioRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>());
        }

        [Test]
        public void SearchByFormat_ShouldReturnContentWithMatchingFormat()
        {
            // Arrange
            ContentFormatDto format = ContentFormatDto.PDF;
            ContentFormat entityFormat = ContentFormat.PDF;

            var books = new List<Book>();
            var documents = new List<Document>();
            var videos = new List<Video>();
            var audios = new List<Audio>();

            var contentDtos = new List<ContentItemDto> {
                new BookDto { Id = 1, Title = "Test Book", Format = ContentFormatDto.PDF },
                new DocumentDto { Id = 2, Title = "Test Document", Format = ContentFormatDto.PDF }
            };

            _mapper.Map<ContentFormat>(format).Returns(entityFormat);

            _bookRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>()).Returns(books);
            _documentRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>()).Returns(documents);
            _videoRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>()).Returns(videos);
            _audioRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>()).Returns(audios);

            _mapper.Map<IEnumerable<ContentItemDto>>(Arg.Any<IEnumerable<ContentItem>>()).Returns(contentDtos);

            // Act
            var result = _searchService.SearchByFormat(format);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(contentDtos.Count));
            _mapper.Received(1).Map<ContentFormat>(format);
            _bookRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>());
            _documentRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>());
            _videoRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>());
            _audioRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>());
        }

        [Test]
        public void SearchByType_ShouldReturnContentOfMatchingType()
        {
            // Arrange
            ContentTypeDto typeDto = ContentTypeDto.Book;
            ContentType entityType = ContentType.Book;

            var books = new List<Book>();
            var contentDtos = new List<ContentItemDto> {
                new BookDto { Id = 1, Title = "Test Book 1", Type = ContentTypeDto.Book },
                new BookDto { Id = 2, Title = "Test Book 2", Type = ContentTypeDto.Book },
                new BookDto { Id = 3, Title = "Test Book 3", Type = ContentTypeDto.Book }
            };

            _mapper.Map<ContentType>(typeDto).Returns(entityType);
            _bookRepository.GetAll().Returns(books);
            _mapper.Map<IEnumerable<ContentItemDto>>(books).Returns(contentDtos);

            // Act
            var result = _searchService.SearchByType(typeDto);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(contentDtos.Count));
            _mapper.Received(1).Map<ContentType>(typeDto);
            _bookRepository.Received(1).GetAll();
            _mapper.Received(1).Map<IEnumerable<ContentItemDto>>(books);
        }

        [Test]
        public void SearchByDateRange_ShouldReturnContentCreatedWithinRange()
        {
            // Arrange
            DateTime startDate = DateTime.Now.AddDays(-10);
            DateTime endDate = DateTime.Now;

            var books = new List<Book>();
            var documents = new List<Document>();
            var videos = new List<Video>();
            var audios = new List<Audio>();

            var contentDtos = new List<ContentItemDto> {
                new BookDto { Id = 1, Title = "Test Book", CreatedDate = DateTime.Now.AddDays(-5) },
                new DocumentDto { Id = 2, Title = "Test Document", CreatedDate = DateTime.Now.AddDays(-3) },
                new VideoDto { Id = 3, Title = "Test Video", CreatedDate = DateTime.Now.AddDays(-1) }
            };

            _bookRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>()).Returns(books);
            _documentRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>()).Returns(documents);
            _videoRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>()).Returns(videos);
            _audioRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>()).Returns(audios);

            _mapper.Map<IEnumerable<ContentItemDto>>(Arg.Any<IEnumerable<ContentItem>>()).Returns(contentDtos);

            // Act
            var result = _searchService.SearchByDateRange(startDate, endDate);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(contentDtos.Count));
            _bookRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>());
            _documentRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>());
            _videoRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>());
            _audioRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>());
        }

        [Test]
        public void SearchByStorageLocation_ShouldReturnContentInMatchingStorage()
        {
            // Arrange
            string location = "Test Location";

            var storages = new List<Storage> { new Storage { Id = 1, Name = "Test Storage", Location = location } };
            var contentStorages = new List<ContentStorage> {
                new ContentStorage { ContentItemId = 1, StorageId = 1 },
                new ContentStorage { ContentItemId = 2, StorageId = 1 }
            };

            var books = new List<Book>();
            var documents = new List<Document>();
            var videos = new List<Video>();
            var audios = new List<Audio>();

            var contentDtos = new List<ContentItemDto> {
                new BookDto { Id = 1, Title = "Test Book" },
                new DocumentDto { Id = 2, Title = "Test Document" }
            };

            // Налаштування заглушок
            _storageRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Storage, bool>>>()).Returns(storages);
            _contentStorageRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<ContentStorage, bool>>>()).Returns(contentStorages);

            _bookRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Book, bool>>>()).Returns(books);
            _documentRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Document, bool>>>()).Returns(documents);
            _videoRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Video, bool>>>()).Returns(videos);
            _audioRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Audio, bool>>>()).Returns(audios);

            _mapper.Map<IEnumerable<ContentItemDto>>(Arg.Any<IEnumerable<ContentItem>>()).Returns(contentDtos);

            // Act
            var result = _searchService.SearchByStorageLocation(location);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(contentDtos.Count));
            _storageRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Storage, bool>>>());
            _contentStorageRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<ContentStorage, bool>>>());
        }
    }
}