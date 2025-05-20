using System;
using System.Collections.Generic;
using System.Linq;

class EmployeeManager
{
    private Dictionary<int, Employee> employeeDictionary = new();

    // Medium : Q1 
    public void AddEmployees()
    {
        Console.Write("Enter number of employees: ");
        int count = Convert.ToInt32(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Employee emp = new();
            emp.TakeEmployeeDetailsFromUser();

            if (employeeDictionary.ContainsKey(emp.Id))
            {
                Console.WriteLine("ID already exists. Please enter a unique ID.");
                i--;
            }
            else
            {
                employeeDictionary.Add(emp.Id, emp);

            }
        }
    }

    public void PrintEmployeeById()
    {
        Console.Write("\nEnter am employee ID to retrieve details : ");
        int searchId = Convert.ToInt32(Console.ReadLine());

        if (employeeDictionary.TryGetValue(searchId, out Employee foundEmployee))
        {
            Console.WriteLine("\nEmployee Found : ");
            Console.WriteLine(foundEmployee);
        }
        else
        {
            Console.WriteLine("Employee with the given ID was not found.");
        }
    }

    // Medium : Q2
    public void DisplaySortedBySalary()
    {
        List<Employee> list = employeeDictionary.Values.ToList();
        list.Sort();

        Console.WriteLine("\n--- Sorted by Salary ---");
        foreach (var emp in list)
        {
            Console.WriteLine(emp);
            Console.WriteLine("---------------------");
        }
    }

    public void SearchById()
    {
        Console.Write("Enter employee ID to search: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var emp = employeeDictionary.Values.Where(e => e.Id == id).FirstOrDefault();
        Console.WriteLine(emp != null ? emp.ToString() : "Employee not found.");
    }

    // Medium : : Q3
    public void SearchByName()
    {
        Console.Write("Enter employee name to search: ");
        string name = Console.ReadLine();

        var matches = employeeDictionary.Values
            .Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Any())
        {
            foreach (var emp in matches)
            {
                Console.WriteLine(emp);
                Console.WriteLine("---------------------");
            }
        }
        else
        {
            Console.WriteLine("No employees found with that name.");
        }
    }

    // Medium : : Q4
    public void FindElderEmployees()
    {
        Console.Write("Enter reference employee ID: ");
        int id = Convert.ToInt32(Console.ReadLine());

        var refEmp = employeeDictionary.Values.FirstOrDefault(e => e.Id == id);
        if (refEmp == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        var elders = employeeDictionary.Values
            .Where(e => e.Age > refEmp.Age)
            .ToList();

        if (elders.Any())
        {
            Console.WriteLine($"\nEmployees elder than {refEmp.Name} (Age {refEmp.Age}):");
            foreach (var emp in elders)
            {
                Console.WriteLine(emp);
                Console.WriteLine("---------------------");
            }
        }
        else
        {
            Console.WriteLine("No elder employees found.");
        }
    }
}



class Program
{
    static void Main(string[] args)
    {
        EmployeeManager manager = new();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n================ Employee Manager Menu ================");
            Console.WriteLine("1. Add Employees");
            Console.WriteLine("2. Print Employee By ID");
            Console.WriteLine("3. Display All Employees Sorted By Salary");
            Console.WriteLine("4. Search Employee By ID (LINQ)");
            Console.WriteLine("5. Search Employees By Name");
            Console.WriteLine("6. Find Elder Employees (than given ID)");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    manager.AddEmployees();
                    break;
                case "2":
                    manager.PrintEmployeeById();
                    break;
                case "3":
                    manager.DisplaySortedBySalary();
                    break;
                case "4":
                    manager.SearchById();
                    break;
                case "5":
                    manager.SearchByName();
                    break;
                case "6":
                    manager.FindElderEmployees();
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("Exiting the application...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number from 0 to 6.");
                    break;
            }
        }
    }
}
