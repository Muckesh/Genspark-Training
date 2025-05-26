using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solution
{
    public class Solution
    {

        public static Dictionary<string, int> AverageAgeForEachCompany(List<Employee> employees)
        {
            
            var avgAgesByCompany = employees.GroupBy(e => e.Company).Select(
                group => new {
                    Company = group.Key,
                    AverageAge = group.Average(e => e.Age)
                }
            ).OrderBy(e=>e.Company);
            
            return avgAgesByCompany.ToDictionary(
                kvp => kvp.Company,
                kvp => Convert.ToInt32(Math.Round(kvp.AverageAge))
            );
            
            
        }
        
        public static Dictionary<string, int> CountOfEmployeesForEachCompany(List<Employee> employees)
        {
            var countOfEmpByCompany = employees.GroupBy(e=>e.Company).Select(
                group => new {
                    Company = group.Key,
                    Count = group.Count()
                }
            ).OrderBy(e=>e.Company);
            
            return countOfEmpByCompany.ToDictionary(
                kvp => kvp.Company,
                kvp => kvp.Count
            );
        }
        
        public static Dictionary<string, Employee> OldestAgeForEachCompany(List<Employee> employees)
        {
            var oldAgeByCompany = employees.GroupBy(e=>e.Company).Select(
                group => new {
                    Company = group.Key,
                    Employee = group.OrderByDescending(e=>e.Age).First()
                }
            ).OrderBy(e=> e.Company);
            
            return oldAgeByCompany.ToDictionary(
                kvp => kvp.Company,
                kvp => kvp.Employee
            );
        }

        public static void Main()
        {   
            int countOfEmployees = int.Parse(Console.ReadLine());
            
            var employees = new List<Employee>();
            
            for (int i = 0; i < countOfEmployees; i++)
            {
                string str = Console.ReadLine();
                string[] strArr = str.Split(' ');
                employees.Add(new Employee { 
                    FirstName = strArr[0], 
                    LastName = strArr[1], 
                    Company = strArr[2], 
                    Age = int.Parse(strArr[3]) 
                    });
            }
            
            foreach (var emp in AverageAgeForEachCompany(employees))
            {
                Console.WriteLine($"The average age for company {emp.Key} is {emp.Value}");
            }
            
            foreach (var emp in CountOfEmployeesForEachCompany(employees))
            {
                Console.WriteLine($"The count of employees for company {emp.Key} is {emp.Value}");
            }
            
            foreach (var emp in OldestAgeForEachCompany(employees))
            {
                Console.WriteLine($"The oldest employee of company {emp.Key} is {emp.Value.FirstName} {emp.Value.LastName} having age {emp.Value.Age}");
            }
        }
    }
    
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Company { get; set; }
    }
}   