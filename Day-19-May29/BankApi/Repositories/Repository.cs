using BankApi.Contexts;
using BankApi.Interfaces;

namespace BankApi.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly BankDbContext _bankDbContext;

        public Repository(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }

        public async Task<T> Add(T item)
        {
            _bankDbContext.Add(item);
            await _bankDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _bankDbContext.Entry(myItem).CurrentValues.SetValues(item);
                await _bankDbContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found.");
        }

        public abstract Task<T> Get(K key);

        public abstract Task<IEnumerable<T>> GetAll();
    }
}