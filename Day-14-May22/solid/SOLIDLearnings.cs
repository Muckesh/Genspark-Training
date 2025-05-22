// S - Single Responsibility Principle SRP
// O - Open / Closed Principle OCP
// L - Liskov Substitution Principle LSP
// I - Interface Segregation Principle ISP
// D - Dependency Inversion Principle DIP

// 1. SRP -> A class should have only one reason to change. It should have only one responsibility or only one job

// Bad example -> The below class have more than one responsibility
// public class Report
// {
//     public string Title { get; set; }
//     public string Content { get; set; }

//     public void PrintReport()
//     {
//         Console.WriteLine($"Printing {Title}");
//     }

//     public void SaveToFile()
//     {
//         Console.WriteLine("Saving to file");
//     }
// }

// Good example SRP -> Each class has one responsibility
public class Report
{
    public string Title { get; set; }
    public string Content { get; set; }
}

public class ReportPrinter
{
    public void Print(Report report)
    {
        Console.WriteLine($"Printing {report.Title}");
    }
}

public class ReportSaver
{
    public void Save(Report report)
    {
        Console.WriteLine($"Saving {report.Title} to file");
    }
}

// 2. OCP -> The classes, functions should be Open for Extensions but Closed for Modifications

// Bad Example -> Everytime we add a new customer type, we have to modify the existing class

// public class DiscountCalculator
// {
//     public double GetDiscount(string customerType)
//     {
//         if (customerType == "Regular")
//         {
//             return 0.1;
//         }
//         else if (customerType == "Premium")
//         {
//             return 0.2;
//         }
//         else
//         {
//             return 0;
//         }
//     }
// }

// Good example OCP -> Here we can add new customer types without modifying DiscountCalculator class

public interface IDiscount
{
    double GetDiscount();
}

public class RegularCustomer : IDiscount
{
    public double GetDiscount() => 0.1;
}

public class PremiumCustomer : IDiscount
{
    public double GetDiscount() => 0.2;
}

public class DiscountCalculator
{
    public double CalculateDiscount(IDiscount customer)
    {
        return customer.GetDiscount();
    }
}

// 3. LSP -> Derived classes must be substitutable for thier base classes without altering the correctness of the program.

// Bad example -> Here Ostrich breaks the method of the Bird class
// public class Bird
// {
//     public virtual void Fly() { }
// }

// public class Ostrich : Bird
// {
//     public override void Fly()
//     {
//         throw new NotImplementedException(); // Ostrich can't fly!
//     }
// }

// Good example LSP -> Now we can substitute IFlyingBird with any flying bird.

public abstract class Bird { }

public interface IFlyingBird
{
    void Fly();
}

public class Sparrow : Bird, IFlyingBird
{
    public void Fly() => Console.WriteLine("Sparrow is flying.");
}

public class Ostrich : Bird
{
    // No fly method. It doesn't violates the base class
}

// 4. ISP -> clients should not be forced to depend on methods they do not use.

// Bad example -> Here JuniorDeveloper class should not be forced to implement ManageTeam.

// public interface IEmployee
// {
//     void Work();
//     void AttendMeeting();
//     void ManageTeam();
// }

// public class JuniorDeveloper : IEmployee
// {
//     public void Work() { }

//     public void AttendMeeting() { }
//     public void ManageTeam() => throw new NotImplementedException();
// }

// Good example ISP -> Now each class implements only the relevant interface.

public interface IWorkable
{
    void Work();
}

public interface ITeamLead
{
    void ManageTeam();
}

public class JuniorDeveloper : IWorkable
{
    public void Work() { }
}

public class TeamLead : IWorkable, ITeamLead
{
    public void Work() { }
    public void ManageTeam() { }
    
}


// 5. DIP -> High level modules should not depend on low level modules. Both should depend on abstractions.

// public class Employee {
//     public string Name { get; set; }
// }

// Bad example -> EmployeeService is tightly coupled with EmployeeRepository concrete class
// public class EmployeeRepository
// {
//     public void Save(Employee emp)
//     {
//         Console.WriteLine($"Employee saved. {emp}");
//     }
// }

// public class EmployeeService
// {
//     private EmployeeRepository repo = new EmployeeRepository();

//     public void AddEmployee(Employee emp)
//     {
//         repo.Save(emp);
//     }
// }

// Good example DIP -> Now EmployeeService class depends on abstraction not a concrete class

// public interface IEmployeeRepository
// {
//     void Save(Employee emp);
// }

// public class EmployeeRepository : IEmployeeRepository
// {
//     public void Save(Employee emp)
//     {
//         Console.WriteLine($"Employee saved. {emp}");
//     }
// }

// public class EmployeeService
// {
//     private readonly IEmployeeRepository _repo;
//     public EmployeeService(IEmployeeRepository repo)
//     {
//         _repo = repo;
//     }

//     public void AddEmployee(Employee emp)
//     {
//         _repo.Save(emp);
//     }
// }