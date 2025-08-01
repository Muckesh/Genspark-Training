namespace Ecommerce.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> AddAsync(T item);
        Task<T> UpdateAsync(K key, T item);
        Task<T> DeleteAsync(K key);
        Task<T> GetByIdAsync(K key);
        Task<ICollection<T>> GetAllAsync();
    }
}