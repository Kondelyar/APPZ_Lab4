using ContentLibrary.BLL.Interfaces;
using ContentLibrary.BLL.Models;
using ContentLibrary.BLL.Services;
using ContentLibrary.DAL.Models;
using ContentLibrary.DAL.Patterns;
using ContentLibrary.DAL.UnitOfWork;
using ContentLibrary.Tests.TestHelpers;
using NSubstitute;
using AutoMapper;
using AutoFixture;

namespace ContentLibrary.Tests
{
    [TestFixture]
    public class StorageServiceTests : TestBase, IDisposable
    {
        private IStorageService _storageService;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IStorageRepository _storageRepository;
        private bool _disposed = false;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Створення заглушок
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _storageRepository = Substitute.For<IStorageRepository>();

            // Налаштування UnitOfWork
            _unitOfWork.GetStorageRepository().Returns(_storageRepository);

            // Створення сервісу
            _storageService = new StorageService(_unitOfWork, _mapper);
        }

        [TearDown]
        public override void Cleanup()
        {
            Dispose(true);
            base.Cleanup();
        }

        // Реалізація IDisposable
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
        public void GetAllStorages_ShouldReturnAllStorages()
        {
            // Arrange
            var storageEntities = new List<Storage>(Fixture.CreateMany<Storage>(3));
            var storageDtos = new List<StorageDto>(Fixture.CreateMany<StorageDto>(3));

            _storageRepository.GetAll().Returns(storageEntities);
            _mapper.Map<IEnumerable<StorageDto>>(Arg.Any<IEnumerable<Storage>>()).Returns(storageDtos);

            // Act
            var result = _storageService.GetAllStorages();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(storageDtos.Count));
            _storageRepository.Received(1).GetAll();
            _mapper.Received(1).Map<IEnumerable<StorageDto>>(Arg.Any<IEnumerable<Storage>>());
        }

        [Test]
        public void GetStorageById_ShouldReturnStorageWithMatchingId()
        {
            // Arrange
            int storageId = 1;
            var storageEntity = Fixture.Create<Storage>();
            var storageDto = Fixture.Create<StorageDto>();

            _storageRepository.GetById(storageId).Returns(storageEntity);
            _mapper.Map<StorageDto>(storageEntity).Returns(storageDto);

            // Act
            var result = _storageService.GetStorageById(storageId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(storageDto));
            _storageRepository.Received(1).GetById(storageId);
            _mapper.Received(1).Map<StorageDto>(storageEntity);
        }

        [Test]
        public void GetStoragesByType_ShouldReturnStoragesOfMatchingType()
        {
            // Arrange
            StorageTypeDto typeDto = StorageTypeDto.LocalDisk;
            StorageType entityType = StorageType.LocalDisk;
            var storageEntities = new List<Storage>(Fixture.CreateMany<Storage>(3));
            var storageDtos = new List<StorageDto>(Fixture.CreateMany<StorageDto>(3));

            _mapper.Map<StorageType>(typeDto).Returns(entityType);
            _storageRepository.Find(Arg.Any<System.Linq.Expressions.Expression<Func<Storage, bool>>>()).Returns(storageEntities);
            _mapper.Map<IEnumerable<StorageDto>>(Arg.Any<IEnumerable<Storage>>()).Returns(storageDtos);

            // Act
            var result = _storageService.GetStoragesByType(typeDto);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(storageDtos.Count));
            _mapper.Received(1).Map<StorageType>(typeDto);
            _storageRepository.Received(1).Find(Arg.Any<System.Linq.Expressions.Expression<Func<Storage, bool>>>());
            _mapper.Received(1).Map<IEnumerable<StorageDto>>(Arg.Any<IEnumerable<Storage>>());
        }

        [Test]
        public void AddStorage_ShouldAddStorageToRepository()
        {
            // Arrange
            var storageDto = Fixture.Create<StorageDto>();  // Створення dummy-об'єкта
            var storageEntity = Fixture.Create<Storage>();

            _mapper.Map<Storage>(storageDto).Returns(storageEntity);

            // Act
            // Dummy-об'єкт storageDto не використовується безпосередньо в тесті,
            // але потрібен для виклику тестованого методу
            _storageService.AddStorage(storageDto);

            // Assert
            _mapper.Received(1).Map<Storage>(storageDto);
            _storageRepository.Received(1).Add(storageEntity);
            _unitOfWork.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateStorage_ShouldUpdateStorageInRepository()
        {
            // Arrange
            var storageDto = Fixture.Create<StorageDto>();
            var storageEntity = Fixture.Create<Storage>();

            _mapper.Map<Storage>(storageDto).Returns(storageEntity);

            // Act
            _storageService.UpdateStorage(storageDto);

            // Assert
            _mapper.Received(1).Map<Storage>(storageDto);
            _storageRepository.Received(1).Update(storageEntity);
            _unitOfWork.Received(1).SaveChanges();
        }

        [Test]
        public void DeleteStorage_ShouldRemoveStorageFromRepository()
        {
            // Arrange
            int storageId = 1;
            var storageEntity = Fixture.Create<Storage>();

            _storageRepository.GetById(storageId).Returns(storageEntity);

            // Act
            _storageService.DeleteStorage(storageId);

            // Assert
            _storageRepository.Received(1).GetById(storageId);
            _storageRepository.Received(1).Delete(storageEntity);
            _unitOfWork.Received(1).SaveChanges();
        }

        [Test]
        public void GetContentInStorage_ShouldReturnAllContentInStorage()
        {
            // Arrange
            int storageId = 1;
            var storage = new Storage { Id = storageId, Name = "Test Storage" };

            // Створення контенту без циклічних посилань
            var book = new Book { Id = 1, Title = "Test Book" };
            var document = new Document { Id = 2, Title = "Test Document" };

            storage.ContentStorages = new List<ContentStorage> {
                new ContentStorage { ContentItemId = 1, StorageId = storageId, ContentItem = book },
                new ContentStorage { ContentItemId = 2, StorageId = storageId, ContentItem = document }
            };

            var contentDtos = new List<ContentItemDto> {
                new BookDto { Id = 1, Title = "Test Book" },
                new DocumentDto { Id = 2, Title = "Test Document" }
            };

            _storageRepository.GetByIdWithContent(storageId).Returns(storage);
            _mapper.Map<IEnumerable<ContentItemDto>>(Arg.Any<IEnumerable<ContentItem>>()).Returns(contentDtos);

            // Act
            var result = _storageService.GetContentInStorage(storageId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(contentDtos.Count));
            _storageRepository.Received(1).GetByIdWithContent(storageId);
            _mapper.Received(1).Map<IEnumerable<ContentItemDto>>(Arg.Any<IEnumerable<ContentItem>>());
        }

        [Test]
        public void GetAvailableSpace_ShouldCalculateCorrectSpace()
        {
            // Arrange
            int storageId = 1;
            var storage = Fixture.Create<Storage>();
            storage.Capacity = 100;
            storage.UsedSpace = 30;

            _storageRepository.GetById(storageId).Returns(storage);

            // Act
            var result = _storageService.GetAvailableSpace(storageId);

            // Assert
            Assert.That(result, Is.EqualTo(70));
            _storageRepository.Received(1).GetById(storageId);
        }
    }
}