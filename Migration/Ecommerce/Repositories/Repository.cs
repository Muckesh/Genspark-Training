using Ecommerce.Contexts;
using Ecommerce.Interfaces;

namespace Ecommerce.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly EcommerceDbContext _ecommerceDbContext;
        public Repository(EcommerceDbContext ecommerceDbContext)
        {
            _ecommerceDbContext = ecommerceDbContext;   
        }
        public abstract Task<ICollection<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(K key);
        public async Task<T> AddAsync(T item)
        {
            _ecommerceDbContext.Add(item);
            await _ecommerceDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> DeleteAsync(K key)
        {
            var item = await GetByIdAsync(key);
            if (item == null)
            {
                throw new KeyNotFoundException("Item not found");
            }

            _ecommerceDbContext.Remove(item);
            await _ecommerceDbContext.SaveChangesAsync();
            return item;

        }

        public async Task<T> UpdateAsync(K key, T item)
        {
            var old_item = await GetByIdAsync(key) ?? throw new KeyNotFoundException("Key not found.");
            _ecommerceDbContext.Entry(old_item).CurrentValues.SetValues(item);
            await _ecommerceDbContext.SaveChangesAsync();
            return item;
        }
    }
}