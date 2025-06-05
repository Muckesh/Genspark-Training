public interface IEmployeeService
{
    // public Task<Employee> GetEmployeeByName(string name);
    public Task<Employee> AddEmployee(EmployeeAddRequestDto employee);
    // public Task<ICollection<Employee>> GetAllEmployees();
}