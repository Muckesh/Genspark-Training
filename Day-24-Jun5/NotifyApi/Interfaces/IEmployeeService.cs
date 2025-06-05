public interface IEmployeeService
{
    Task<Employee> RegisterEmployee(EmployeeAddRequestDto employee);
}
