// SRP, DIP
public interface IRepository<T>
{
    void Save(T item);
    List<T> GetAll();
}