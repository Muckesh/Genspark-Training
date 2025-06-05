
using Microsoft.EntityFrameworkCore;

public class EmployeeRepository : Repository<int, Employee>
{
    public EmployeeRepository(NotifyDbContext context) : base(context)
    {
        
    }
    public override async Task<Employee> Get(int key)
    {
        var employee = await _context.Employees.SingleOrDefaultAsync(d => d.Id == key);
        if (employee != null)
        {
            return employee;
        }
        throw new Exception("Employee not found with the given ID.");
    }

    public override async Task<IEnumerable<Employee>> GetAll()
    {
        var employees = await _context.Employees.ToListAsync();
        if (employees.Count() == 0)
        {
            throw new Exception("No employees in the database.");
        }
        return employees;

    }
}