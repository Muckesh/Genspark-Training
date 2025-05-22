// SRP, DIP
public class EmployeeRepository : IRepository<Employee>
{
    private readonly List<Employee> employees = new();

    public void Save(Employee emp) => employees.Add(emp);
    public List<Employee> GetAll() => employees;
}