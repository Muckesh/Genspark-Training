// DIP
public class EmployeeService
{
    private readonly IRepository<Employee> _repository;
    private readonly ISalaryCalculator _salaryCalculator;
    private readonly IReportGenerator _reportGenerator;
    public EmployeeService(IRepository<Employee> repository, ISalaryCalculator salaryCalculator, IReportGenerator reportGenerator)
    {
        _repository = repository;
        _salaryCalculator = salaryCalculator;
        _reportGenerator = reportGenerator;
    }

    public void AddEmployee(Employee employee)
    {
        _repository.Save(employee);
        Console.WriteLine($"{employee.Name} added with salary: {_salaryCalculator.CalculateSalary(employee)}");
    }

    public void GenerateReport()
    {
        var employees = _repository.GetAll().Cast<IEmployee>().ToList();
        _reportGenerator.Generate(employees);
    }
}