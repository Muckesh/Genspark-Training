using System;
using System.Collections.Generic;
using System.Linq;

// Hard Question

class EmployeeManager
{
    private Dictionary<int, Employee> employeeDictionary = new();

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


    public void PrintAllEmployees()
    {
        if (employeeDictionary.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        Console.WriteLine("\n--- All Employees ---");
        foreach (var emp in employeeDictionary.Values)
        {
            Console.WriteLine(emp);
            Console.WriteLine("---------------------");
        }
    }

    public void PrintEmployeeById()
    {
        Console.Write("Enter Employee ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (employeeDictionary.TryGetValue(id, out Employee emp))
            {
                Console.WriteLine("\nEmployee Details:");
                Console.WriteLine(emp);
            }
            else
            {
                Console.WriteLine("No employee found with the given ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID entered.");
        }
    }

    public void EditEmployee()
    {
        Console.Write("Enter Employee ID to modify: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (employeeDictionary.TryGetValue(id, out Employee emp))
            {
                Console.WriteLine("Leave input blank to keep existing value.");

                Console.Write($"Current Name ({emp.Name}): ");
                string name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name)) emp.Name = name;

                Console.Write($"Current Age ({emp.Age}): ");
                string ageInput = Console.ReadLine();
                if (int.TryParse(ageInput, out int newAge)) emp.Age = newAge;

                Console.Write($"Current Salary ({emp.Salary}): ");
                string salaryInput = Console.ReadLine();
                if (double.TryParse(salaryInput, out double newSalary)) emp.Salary = newSalary;

                Console.WriteLine("Employee details updated.");
            }
            else
            {
                Console.WriteLine("No employee found with the given ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID entered.");
        }
    }

    public void DeleteEmployee()
    {
        Console.Write("Enter Employee ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (employeeDictionary.Remove(id))
            {
                Console.WriteLine("Employee removed successfully.");
            }
            else
            {
                Console.WriteLine("No employee found with the given ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID entered.");
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
            Console.WriteLine("\n=============== Employee Management Menu ===============");
            Console.WriteLine("1. Add Employees");
            Console.WriteLine("2. View All Employees");
            Console.WriteLine("3. View Employee By ID");
            Console.WriteLine("4. Edit Employee Details");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
            Console.WriteLine("========================================================");

            switch (choice)
            {
                case "1":
                    manager.AddEmployees();
                    break;
                case "2":
                    manager.PrintAllEmployees();
                    break;
                case "3":
                    manager.PrintEmployeeById();
                    break;
                case "4":
                    manager.EditEmployee();
                    break;
                case "5":
                    manager.DeleteEmployee();
                    break;
                case "0":
                    exit = true;
                    Console.WriteLine("Exiting application...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
