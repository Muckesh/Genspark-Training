using System.Threading.Tasks;
using VideoPortalAPi.Contexts;
using VideoPortalAPi.Interfaces;

namespace VideoPortalAPi.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly VideoPortalDbContext _videoPortalDbContext;

        public Repository(VideoPortalDbContext videoPortalDbContext)
        {
            _videoPortalDbContext = videoPortalDbContext;
        }
        public abstract Task<ICollection<T>> GetAll();

        public abstract Task<T> GetById(K id);

        public async Task<T> Add(T item)
        {
            _videoPortalDbContext.Add(item);
            await _videoPortalDbContext.SaveChangesAsync();

            return item;
        }

        public async Task<T> Delete(K id)
        {
            var item = await GetById(id);
            if (item == null)
                throw new KeyNotFoundException("Item not found");
            _videoPortalDbContext.Remove(item);
            await _videoPortalDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Update(K key, T item)
        {
            var oldItem = await GetById(key);
            if (item == null)
                throw new KeyNotFoundException("Item not found");
            _videoPortalDbContext.Entry(oldItem).CurrentValues.SetValues(item);
            await _videoPortalDbContext.SaveChangesAsync();
            return item;
        }
    }
}