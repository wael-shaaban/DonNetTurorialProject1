using DonNetTurorialProject1.Data.Repositories;
using DonNetTurorialProject1.Models.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DonNetTurorialProject1.Data.Services
{
    public interface IBorrowBookService : IGenericService<BorrowRecord>
    {
        public int Add(BorrowRecord borrowBook);
        public BorrowRecord GetBorrowRecord(BorrowViewModel borrowViewModel);
        public BorrowViewModel GetBorrowViewModel(BorrowRecord record);
        public BorrowRecord GetBorrowRecordWithRelated(int borrowdRecordId,string relatedEntityName);
        public int Update(BorrowRecord borrowRecord);  
    }
    public class BorrowBookService(IUnitOfWork unitOfWork, IMapper mapper) : IBorrowBookService
    {
        public int Add(BorrowRecord borrowBook)
        {
            var data = unitOfWork.BorrowBooks.Add(borrowBook);
            var success = unitOfWork.Complete();
            if (data is not null)
                return unitOfWork.Complete();
            return -1;
        }

        public IEnumerable<BorrowRecord> GetAll() =>
            unitOfWork.BorrowBooks.GetAll();

        public BorrowRecord GetBorrowRecord(BorrowViewModel borrowViewModel)=>mapper.Map<BorrowRecord>(borrowViewModel);
       
        public BorrowRecord GetBorrowRecordWithRelated(int borrowdRecordId, string relatedEntityName)=>
          unitOfWork.BorrowBooks.GetWithLoadingRelated(relatedEntityName).FirstOrDefault(b=>b.BorrowRecordId == borrowdRecordId);

        public BorrowViewModel GetBorrowViewModel(BorrowRecord record)=>mapper.Map<BorrowViewModel>(record);

        public BorrowRecord GetByID<ID>(ID id) where ID : struct =>
          unitOfWork.BorrowBooks.GetById(id);

        public int Update(BorrowRecord borrowRecord)
        {
            var data = unitOfWork.BorrowBooks.Update(borrowRecord);
            if (data is not null)
            { return unitOfWork.Complete(); }
            return -1;
        }
    }
}