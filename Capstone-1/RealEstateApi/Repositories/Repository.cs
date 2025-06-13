using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;

namespace RealEstateApi.Repositories
{
    public abstract class Repository<K,T> : IRepository<K,T> where T:class
    {
        protected readonly RealEstateDbContext _realEstateDbContext;

        public Repository(RealEstateDbContext realEstateDbContext)
        {
            _realEstateDbContext = realEstateDbContext;            
        }


        public abstract Task<IEnumerable<T>> GetAllAsync();

        public abstract Task<T> GetByIdAsync(K id);

        // public abstract Task<K> DeleteAsync(K key);


        public async Task<T> AddAsync(T item)
        {
            _realEstateDbContext.Add(item);
            await _realEstateDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<K> DeleteAsync(K key)
        {
            var item = await GetByIdAsync(key);
            if (item == null)
            {
                throw new NotFoundException("Item not found");
            }

            _realEstateDbContext.Remove(item);
            await _realEstateDbContext.SaveChangesAsync();
            return key;
        }

        public async Task<T> UpdateAsync(K key, T item)
        {
            var old_item = await GetByIdAsync(key) ?? throw new KeyNotFoundException("Key not found.");
            _realEstateDbContext.Entry(old_item).CurrentValues.SetValues(item);
            await _realEstateDbContext.SaveChangesAsync();
            return item;

        }

    }
}