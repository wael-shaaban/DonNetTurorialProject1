namespace DonNetTurorialProject1.Data.Services
{
    public interface IGenericService<T> where T : class
    {
        public IEnumerable<T> GetAll();
        public T GetByID<ID>(ID id) where ID : struct;
    }
}
