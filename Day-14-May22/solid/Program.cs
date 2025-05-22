

class Program
{
    static void Main()
    {
        IRepository<Employee> repo = new EmployeeRepository();
        ISalaryCalculator calculator = new SalaryCalculator();
        IReportGenerator reportGen = new ReportGenerator();

        EmployeeService empService = new(repo, calculator, reportGen);

        var john = new PermanentEmployee { Name = "John Doe" };
        var jane = new ContractEmployee { Name = "Jane Smith" };

        empService.AddEmployee(john);
        empService.AddEmployee(jane);

        empService.GenerateReport();
    }
}
