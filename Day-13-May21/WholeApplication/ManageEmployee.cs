using System.Collections.Specialized;
using WholeApplication.Interfaces;
using WholeApplication.Models;

namespace WholeApplication
{
    public class ManageEmployee
    {
        private readonly IEmployeeService _employeeService;

        public ManageEmployee(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                int choice = 0;
                while (!int.TryParse(Console.ReadLine(), out choice) || (choice < 1 && choice > 2))
                {
                    Console.WriteLine("Invalid entry. Please enter a valid choice.");
                }
                switch (choice)
                {
                    case 1:
                        AddEmployee();
                        break;
                    case 2:
                        SearchEmployees();
                        break;
                    default:
                        exit = true;
                        break;
                }
            }
        }

        public void PrintMenu()
        {
            Console.WriteLine("------------------------ Employee Management Menu ---------------------------------");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Search Employee");
            Console.Write("Enter your choice : ");
        }
        public void AddEmployee()
        {
            Employee employee = new Employee();
            employee.TakeEmployeeDetailsFromUser();
            int id = _employeeService.AddEmployee(employee);
            if (id != -1)
            {
                Console.WriteLine($"Employee added with ID: {id}");
            }
            else
            {
                Console.WriteLine("Failed to add employee.");
            }
        }

        public void SearchEmployees()
        {
            SearchModel searchModel = new SearchModel();
            Console.Write("Enter ID to search (leave blank to skip) : ");
            int id;
            if (int.TryParse(Console.ReadLine(), out id))
            {
                searchModel.Id = id;
            }
            else
            {
                Console.WriteLine("Skipping...");
            }

            Console.Write("Enter Name to search (leave blank to skip) : ");
            string nameInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nameInput))
            {
                searchModel.Name = nameInput;
            }
            else
            {
                Console.WriteLine("Skipping...");
            }

            Console.Write("Enter Age range (min-max, e.g. 25-40) : ");
            string ageInput = Console.ReadLine();
            if (TryParseRange(ageInput, out Range<int> ageRange))
            {
                searchModel.Age = ageRange;
            }
            else
            {
                Console.WriteLine("Skipping...");
            }

            Console.Write("Enter Salary range (min-max, e.g. 50000-100000) : ");
            string salaryInput = Console.ReadLine();
            if (TryParseRange(salaryInput, out Range<double> salaryRange))
            {
                searchModel.Salary = salaryRange;
            }
            else
            {
                Console.WriteLine("Skipping...");
            }

            var results = _employeeService.SearchEmployee(searchModel);
            if (results==null||results.Count == 0)
            {
                Console.WriteLine("No employees found.");
            }else
            {
                Console.WriteLine("\n--------------------- Search Results -----------------------------");
                PrintEmployees(results);
            }
        }

        private bool TryParseRange<T>(string input, out Range<T> range) where T : struct, IComparable
        {
            range = null;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var parts = input.Split('-');
            if (parts.Length == 2 &&
                TryParse<T>(parts[0], out T min) &&
                TryParse<T>(parts[1], out T max))
            {
                range = new Range<T> { MinVal = min, MaxVal = max };
                return true;
            }
            return false;
        }

        private bool TryParse<T>(string input, out T value) where T : struct
        {
            value = default;
            try
            {
                value = (T)Convert.ChangeType(input, typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SearchEmployee()
        {
            var searchMenu = PrintSearchMenu();
            var employees = _employeeService.SearchEmployee(searchMenu);
            Console.WriteLine("The search options you have selected : ");
            Console.WriteLine(searchMenu);

            if (employees == null)
            {
                Console.WriteLine("No employees for the search.");
            }
            PrintEmployees(employees);
        }

        private void PrintEmployees(List<Employee>? employees)
        {
            foreach (var employee in employees)
            {
                Console.WriteLine(employee);
            }
        }

        private SearchModel PrintSearchMenu()
        {
            Console.WriteLine("Please select the search option : ");
            SearchModel searchModel = new SearchModel();
            Console.WriteLine("Search by ID? Type 1 for yes Type 2 no");
            int idOption = 0;
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if(idOption == 1)
            {
                Console.WriteLine("Please enter the employee ID");
                int id;
                while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
                {
                    Console.WriteLine("Invalid entry for ID. Please enter a valid employee ID");
                }
                searchModel.Id = id;
                idOption = 0;
                return searchModel;
            }
            Console.WriteLine("Search by Name. ? Type 1 for yes Type 2 no");
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if (idOption == 1)
            {
                Console.WriteLine("Please enter the employee Name");
                string name = Console.ReadLine() ?? "";
                searchModel.Name = name;
                idOption = 0;
            }
            Console.WriteLine("3. Search by Age. Please enter 1 for yes and 2 for no");
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if(idOption == 1)
            {
                searchModel.Age = new Range<int>();
                int age;
                Console.WriteLine("Please enter the min employee Age");
                while (!int.TryParse(Console.ReadLine(), out age) || age <= 18)
                {
                    Console.WriteLine("Invalid entry for min age. Please enter a valid employee age");
                }
                searchModel.Age.MinVal = age;
                Console.WriteLine("Please enter the max employee Age");
                while (!int.TryParse(Console.ReadLine(), out age) || age <= 18)
                {
                    Console.WriteLine("Invalid entry for max age. Please enter a valid employee age");
                }
                searchModel.Age.MaxVal = age;
                idOption = 0;
            }

            Console.WriteLine("4. Search by Salary. Please enter 1 for yes and 2 for no");
            while (!int.TryParse(Console.ReadLine(), out idOption) || (idOption != 1 && idOption != 2))
            {
                Console.WriteLine("Invalid entry. Please enter 1 for yes and 2 for no");
            }
            if(idOption == 1)
            {
                searchModel.Salary = new Range<double>();
                double salary;
                Console.WriteLine("Please enter the min employee Salary");
                while (!double.TryParse(Console.ReadLine(), out salary) || salary <= 0)
                {
                    Console.WriteLine("Invalid entry for min salary. Please enter a valid employee salary");
                }
                searchModel.Salary.MinVal = salary;
                Console.WriteLine("Please enter the max employee Salary");
                while (!double.TryParse(Console.ReadLine(), out salary) || salary <= 0)
                {
                    Console.WriteLine("Invalid entry for max salary. Please enter a valid employee salary");
                }
                searchModel.Salary.MaxVal = salary;
                idOption = 0;
            }
            return searchModel;
        }
    }
}