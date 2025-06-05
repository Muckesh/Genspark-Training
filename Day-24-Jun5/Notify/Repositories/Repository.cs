

public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly NotifyDbContext _context;
    
    public Repository(NotifyDbContext context)
    {
        _context = context;
    }

    public abstract Task<T> Get(K key);

    public abstract Task<IEnumerable<T>> GetAll();

    public async Task<T> Add(T item)
    {
        _context.Add(item);
        //generate and execute the DML quries for the objects whse state is in ['added','modified','deleted'],
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<T> Update(K key, T item)
    {
        var myItem = await Get(key);
        if (myItem != null)
        {
            _context.Entry(myItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return item;
        }
        throw new Exception("No such item found for updation.");
    }

    public async Task<T> Delete(K key)
    {
        var item = await Get(key);
        if (item != null)
        {
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }
        throw new Exception("No such item found for deleting.");
    }
}