using DonNetTurorialProject1.Data.Repositories;
using DonNetTurorialProject1.Models.Entities;

namespace DonNetTurorialProject1.Data.Services
{
    public interface IBookService : IGenericService<Book>
    {
        public IEnumerable<Book> GetAllWithRelatedNavigationProperites(string inludededNavigationProperties, bool asNoTracking = false);
        public int Add(Book book);
        public int Edit(Book newBook);
        public int Delete(int id);
        public BorrowViewModel GetBorrowerdBook(Book book);
    }
    public class BookService(IUnitOfWork unitOfWork, IMapper mapper) : IBookService
    {
        public IEnumerable<Book> GetAll() => unitOfWork.Books.GetAll();

        public Book GetByID<ID>(ID id) where ID : struct => unitOfWork.Books.GetById(id);
        public IEnumerable<Book> GetAllWithRelatedNavigationProperites(string inludededNavigationProperties, bool asNoTracking = false) =>
            unitOfWork.Books.GetWithLoadingRelated(includeProperties: inludededNavigationProperties, asNoTracking: asNoTracking).ToList();

        public int Add(Book book)
        {
            var data = unitOfWork.Books.Add(book);
            if (data is not null)
            {
                return unitOfWork.Complete();
            }
            return -1;
        }

        public int Edit(Book newBook)
        {
            var data = unitOfWork.Books.Update(newBook);
            if (data is not null)
            { return unitOfWork.Complete(); }
            return -1;
        }

        public int Delete(int id)
        {
            var book = unitOfWork.Books.GetById(id);
            if (book is not null)
            {
                unitOfWork.Books.Delete(book);
                return unitOfWork.Complete();
            }
            return -1;
        }

        public BorrowViewModel GetBorrowerdBook(Book book) =>
     mapper.Map<BorrowViewModel>(book);
    }
}