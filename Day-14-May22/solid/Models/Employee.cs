// LSP
public abstract class Employee : IEmployee
{
    public string Name { get; set; }
    public abstract double GetSalary();
}