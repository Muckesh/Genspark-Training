namespace VideoPortalAPi.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> Add(T item);
        Task<T> Delete(K id);
        Task<T> GetById(K id);
        Task<ICollection<T>> GetAll();
        Task<T> Update(K key,T item);
    }
}