// SRP
public class ReportGenerator : IReportGenerator
{
    public void Generate(List<IEmployee> employees)
    {
        Console.WriteLine("\nEmployee Report : ");
        foreach (var emp in employees)
        {
            Console.WriteLine($"Name: {emp.Name}, Salary: {emp.GetSalary()}");
        }
    }
}