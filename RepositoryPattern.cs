using ContentLibrary.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ContentLibrary.DAL.Patterns
{
    // Інтерфейс репозиторія
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }

    // Базова реалізація репозиторія
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ContentLibraryDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ContentLibraryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }

    // Спеціалізований репозиторій для ContentItem з прикладами жадібного та лінивого завантаження
    public class ContentRepository<T> : Repository<T> where T : Models.ContentItem
    {
        public ContentRepository(ContentLibraryDbContext context) : base(context)
        {
        }

        // Жадібне завантаження - завантажує контент разом зі зв'язаними сховищами
        public IEnumerable<T> GetAllWithStorages()
        {
            return _dbSet
                .Include(c => c.ContentStorages)
                .ThenInclude(cs => cs.Storage)
                .ToList();
        }

        // Жадібне завантаження - знаходить контент за id з усіма пов'язаними сховищами
        public T GetByIdWithStorages(int id)
        {
            return _dbSet
                .Include(c => c.ContentStorages)
                .ThenInclude(cs => cs.Storage)
                .FirstOrDefault(c => c.Id == id);
        }

        // Лініве завантаження - повертає об'єкт без явного завантаження пов'язаних даних
        // Дані будуть завантажені за потреби при зверненні до них, якщо увімкнено lazy loading
        public T GetByIdLazy(int id)
        {
            return _dbSet.Find(id);
        }
    }

    // Спеціалізований репозиторій для Storage
    public class StorageRepository : Repository<Models.Storage>
    {
        public StorageRepository(ContentLibraryDbContext context) : base(context)
        {
        }

        // Жадібне завантаження сховища з усім контентом
        public IEnumerable<Models.Storage> GetAllWithContent()
        {
            return _dbSet
                .Include(s => s.ContentStorages)
                .ThenInclude(cs => cs.ContentItem)
                .ToList();
        }

        // Жадібне завантаження конкретного сховища з його контентом
        public Models.Storage GetByIdWithContent(int id)
        {
            return _dbSet
                .Include(s => s.ContentStorages)
                .ThenInclude(cs => cs.ContentItem)
                .FirstOrDefault(s => s.Id == id);
        }
    }
}