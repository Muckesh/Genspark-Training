public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly NotifyDbContext _context;

    public Repository(NotifyDbContext context)
    {
        _context = context;
    }
    public abstract Task<T> GetById(K id);

    public abstract Task<IEnumerable<T>> GetAll();
    public virtual async Task<T> Add(T item)
    {

        _context.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<K> Delete(K id)
    {
        var item = await GetById(id);
        if (item == null)
        {
            throw new KeyNotFoundException("Item not found");
        }

        _context.Remove(item);
        await _context.SaveChangesAsync();
        return id;
    }

    public async Task<T> Update(K id, T item)
    {
        var old_item = await GetById(id) ?? throw new KeyNotFoundException("Item not found");
        _context.Entry(old_item).CurrentValues.SetValues(item);
        await _context.SaveChangesAsync();
        return item;
    }

}