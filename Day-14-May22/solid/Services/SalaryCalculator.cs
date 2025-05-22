// OCP
public class SalaryCalculator : ISalaryCalculator
{
    public double CalculateSalary(IEmployee employee) => employee.GetSalary();
}