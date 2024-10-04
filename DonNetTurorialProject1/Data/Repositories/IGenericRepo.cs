using DonNetTurorialProject1.Data.Services;
using DonNetTurorialProject1.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DonNetTurorialProject1.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(FindOptions? findOptions = null);
        TEntity GetById<ID>(ID id) where ID : struct;
        TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null);
        TEntity Add(TEntity entity);
        IQueryable<TEntity> AddMany(IEnumerable<TEntity> entities);
        TEntity Update(TEntity entity);
        TEntity Delete(TEntity entity);
        void Delete<R>(R id) where R : struct;
        void DeleteMany(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> UpdateMany(Expression<Func<TEntity, bool>> predicate);
        TEntity Attach(TEntity entity);
        TEntity AttachIfNot(TEntity entity);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        int Count(Expression<Func<TEntity, bool>> predicate);
        // Execute stored procedure
        Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string procedureName, params object[] parameters) where TResult : class;

        // Views or any custom queries
        Task<IEnumerable<TResult>> QueryViewAsync<TResult>(string sqlQuery, params object[] parameters) where TResult : class;
        public IEnumerable<TEntity> GetWithLoadingRelated(string includeProperties = "",bool asNoTracking=false,
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
           );
    }
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly LibraryContext _LibraryContext;
        public Repository(LibraryContext LibraryContext)
        {
            _LibraryContext = LibraryContext;
        }
        public TEntity Add(TEntity entity) => GetDbSet().Add(entity).Entity;


        public IQueryable<TEntity> AddMany(IEnumerable<TEntity> entities)
        {
            GetDbSet().AddRange(entities);
            return entities.AsQueryable();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return GetDbSet().Any(predicate);
        }

        public TEntity Attach(TEntity entity)
        {
            var dbSet = GetDbSet();
            var attachedEntity = dbSet.Attach(entity).Entity;
            return attachedEntity;
        }

        public TEntity AttachIfNot(TEntity entity)
        {
            if (_LibraryContext.Entry(entity).State == EntityState.Detached)
                return Attach(entity);
            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity");
            }

            if (_LibraryContext.Entry(entity).State == EntityState.Detached)
                _LibraryContext.Entry(entity).State = EntityState.Deleted;
            else
                GetDbSet().Remove(entity);
            return entity;
        }

        public void DeleteMany(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = Find(predicate);
            GetDbSet().RemoveRange(entities);
        }

        public IQueryable<TEntity> GetAll(FindOptions? findOptions = null)
        {
            return Get(findOptions);
        }

        public TEntity GetById<ID>(ID id) where ID : struct
        {
            return GetDbSet().Find(id);
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)
        {
            return Get(findOptions).AsQueryable().FirstOrDefault(predicate)!;
        }
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, FindOptions? findOptions = null)
        {
            return Get(findOptions).Where(predicate).AsQueryable();
        }
        // Dispose the context
        public void Dispose() => _LibraryContext.Dispose();
        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            return GetDbSet().Update(entity).Entity;
        }

        public IQueryable<TEntity> UpdateMany(Expression<Func<TEntity, bool>> predicate)
        {
            GetDbSet().UpdateRange(GetDbSet().Where(predicate));
            return GetDbSet().Where(predicate).AsQueryable();
        }
        // Execute a stored procedure and map the result
        public async Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string procedureName, params object[] parameters) where TResult : class
            => await _LibraryContext.Set<TResult>().FromSqlRaw(procedureName, parameters).ToListAsync();

        // Execute raw SQL for custom views or complex queries
        public async Task<IEnumerable<TResult>> QueryViewAsync<TResult>(string sqlQuery, params object[] parameters) where TResult : class
            => await _LibraryContext.Set<TResult>().FromSqlRaw(sqlQuery, parameters).ToListAsync();
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return GetDbSet().Count(predicate);
        }

        #region Private Functions
        private DbSet<TEntity>? Get(FindOptions? findOptions = null)
        {
            findOptions ??= new FindOptions(false, false);
            var entity = GetDbSet();
            if (findOptions.IsAsNoTracking && findOptions.IsIgnoreAutoIncludes)
            {
                entity.IgnoreAutoIncludes().AsNoTracking();
            }
            else if (findOptions.IsIgnoreAutoIncludes)
            {
                entity.IgnoreAutoIncludes();
            }
            else if (findOptions.IsAsNoTracking)
            {
                entity.AsNoTracking();
            }
            return entity;
        }
        private DbSet<TEntity>? GetDbSet() => _LibraryContext?.Set<TEntity>();
        #endregion Private Functions

        public virtual IEnumerable<TEntity> GetWithLoadingRelated(
      string includeProperties = "", bool asNoTracking = false,
      Expression<Func<TEntity, bool>> filter = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
     )  // New parameter to control AsNoTracking behavior
        {
            IQueryable<TEntity> query = GetDbSet();

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                query = orderBy(query);  // Apply ordering if provided

            // Apply AsNoTracking only at the end if the parameter is true
            if (asNoTracking)
                query = query.AsNoTracking();  // Add AsNoTracking at the end

            return query.ToList();  // Execute the query and convert to a list
        }


        public void Delete<R>(R id) where R : struct
        {
            TEntity entityToDelete = GetDbSet().Find(id);
            Delete(entityToDelete);
        }
    }
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Book> Books { get; }
        IRepository<BorrowRecord> BorrowBooks { get; }

        #region Transactions Functions
        int Commit();
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        void Rollback();
        void BeginTransaction();
        int Complete();
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
        #endregion Transactions Functions
    }
    public interface IUnitofWorkService
    {
        IBookService BookService { get; }
        IBorrowBookService BorrowBookService { get; }
    }
    public class UnitofWorkService(IBookService bookService, IBorrowBookService borrowBookService) : IUnitofWorkService
    {
        public IBookService BookService => bookService;

        public IBorrowBookService BorrowBookService => borrowBookService;
    }
    public class UnitOfWork(IRepository<Book> _books, IRepository<BorrowRecord> _borrowBooks, LibraryContext _LibraryContext) : IUnitOfWork
    {
        private bool disposed = false;


        protected virtual void Dispose(bool disposing)
        {
            //The following Method is going to Dispose of the Context Object
            if (_LibraryContext is not null)
                if (!disposed)
                {
                    if (disposing)
                    {
                        _transaction?.Dispose();
                        _LibraryContext.Dispose();
                    }
                }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private IDbContextTransaction _transaction;

        public IRepository<Book> Books => _books;

        public IRepository<BorrowRecord> BorrowBooks => _borrowBooks;

        public void BeginTransaction()
        {
            _transaction = _LibraryContext.Database.BeginTransaction();
        }

        public int Commit()
        {
            try
            {
                int x = _LibraryContext.SaveChanges();
                _transaction?.Commit();
                return x;
            }
            catch
            {
                Rollback();
                throw;
            }
        }
        public virtual async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int x = await _LibraryContext.SaveChangesAsync(cancellationToken);
                _transaction?.CommitAsync(cancellationToken);
                return x;
            }
            catch
            {
                Rollback();
                throw;
            }
        }
        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction.Dispose();
        }

        public int Complete() => _LibraryContext.SaveChanges();
        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default) =>
            await _LibraryContext.SaveChangesAsync(cancellationToken);
    }
}