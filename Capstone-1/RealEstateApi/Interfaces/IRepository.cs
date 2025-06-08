namespace RealEstateApi.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(K id);
        Task<T> AddAsync(T item);
        Task<T> UpdateAsync(K key, T item);
        Task<K> DeleteAsync(K key);
    }
}