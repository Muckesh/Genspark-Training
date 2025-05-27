
public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected List<T> _items = new List<T>();
    protected abstract K GenerateId();
    public abstract ICollection<T> GetAll();
    public abstract T GetById(K id);
    public abstract T Update(T item);
    public T Add(T item)
    {
        var id = GenerateId();
        var property = typeof(T).GetProperty("Id");
        if (property != null)
        {
            property.SetValue(item, id);
        }
        if (_items.Contains(item))
        {
            throw new Exception("Appointment already exists.");
        }
        _items.Add(item);
        return item;
    }

    public T Delete(K id)
    {
        var item = GetById(id);
        if (item == null)
        {
            throw new KeyNotFoundException("Appointment not found.");
        }
        _items.Remove(item);
        return item;
    }
    
}