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
    public class BookServiceTests : TestBase, IDisposable
    {
        private IBookService _bookService;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IContentRepository<Book> _bookRepository;
        private bool _disposed = false;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            // Створення заглушок
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _bookRepository = Substitute.For<IContentRepository<Book>>();

            // Налаштування поведінки UnitOfWork
            _unitOfWork.GetContentRepository<Book>().Returns(_bookRepository);

            // Створення сервісу з заглушками
            _bookService = new BookService(_unitOfWork, _mapper);
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
                    // Звільнення ресурсів
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
        public void GetAllContent_ShouldReturnAllBooks()
        {
            // Замість використання AutoFixture для створення складних об'єктів,
            // створимо простіші заглушки
            var bookDtos = new List<BookDto> {
                new BookDto { Id = 1, Title = "Test Book 1" },
                new BookDto { Id = 2, Title = "Test Book 2" },
                new BookDto { Id = 3, Title = "Test Book 3" }
            };

            // Використовуємо порожній список для вхідних даних,
            // оскільки ми підміняємо результат маппера
            _bookRepository.GetAllWithStorages().Returns(new List<Book>());
            _mapper.Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>()).Returns(bookDtos);

            // Act
            var result = _bookService.GetAllContent();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(bookDtos.Count));
            _bookRepository.Received(1).GetAllWithStorages();
            _mapper.Received(1).Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>());
        }

        [Test]
        public void GetBooksByAuthor_ShouldReturnBooksByAuthor()
        {
            // Arrange
            var authorName = "Test Author";
            var bookDtos = new List<BookDto> {
                new BookDto { Id = 1, Title = "Test Book 1", Author = authorName },
                new BookDto { Id = 2, Title = "Test Book 2", Author = authorName },
                new BookDto { Id = 3, Title = "Test Book 3", Author = authorName }
            };

            _bookRepository.GetAllWithStorages().Returns(new List<Book>());
            _mapper.Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>()).Returns(bookDtos);

            // Act
            var result = _bookService.GetBooksByAuthor(authorName);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(bookDtos.Count));
            Assert.That(result.All(b => b.Author == authorName), Is.True);
            _bookRepository.Received(1).GetAllWithStorages();
        }

        [Test]
        public void GetBookByISBN_ShouldReturnBookWithMatchingISBN()
        {
            // Arrange
            var isbn = "1234567890123";
            var bookDtos = new List<BookDto> {
                new BookDto { Id = 1, Title = "Test Book 1", ISBN = isbn },
                new BookDto { Id = 2, Title = "Test Book 2", ISBN = "9876543210987" },
                new BookDto { Id = 3, Title = "Test Book 3", ISBN = "5432167890123" }
            };

            _bookRepository.GetAllWithStorages().Returns(new List<Book>());
            _mapper.Map<IEnumerable<BookDto>>(Arg.Any<IEnumerable<Book>>()).Returns(bookDtos);

            // Act
            var result = _bookService.GetBookByISBN(isbn);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ISBN, Is.EqualTo(isbn));
            _bookRepository.Received(1).GetAllWithStorages();
        }

        [Test]
        public void AddContent_ShouldAddBookToRepository()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890123"
            };

            var bookEntity = new Book();

            _mapper.Map<Book>(bookDto).Returns(bookEntity);

            // Act
            _bookService.AddContent(bookDto);

            // Assert
            _bookRepository.Received(1).Add(Arg.Any<Book>());
            _unitOfWork.Received(1).SaveChanges();
        }

        [Test]
        public void DeleteContent_ShouldRemoveBookFromRepository()
        {
            // Arrange
            int bookId = 1;
            var bookEntity = new Book { Id = bookId };

            _bookRepository.GetById(bookId).Returns(bookEntity);

            // Act
            _bookService.DeleteContent(bookId);

            // Assert
            _bookRepository.Received(1).Delete(Arg.Any<Book>());
            _unitOfWork.Received(1).SaveChanges();
        }
    }
}