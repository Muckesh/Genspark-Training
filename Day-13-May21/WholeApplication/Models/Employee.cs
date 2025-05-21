namespace WholeApplication.Models
{
    public class Employee : IComparable<Employee>, IEquatable<Employee>
    {
        // int id, age; 
        // string name; 
        // double salary; 
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }

        public Employee()
        {
            Name = string.Empty;
        }

        public Employee(int id, int age, string name, double salary)
        {
            // this.id = id; 
            // this.age = age; 
            // this.name = name; 
            // this.salary = salary; 

            Id = id;
            Name = name;
            Age = age;
            Salary = salary;
        } 
 
        public void TakeEmployeeDetailsFromUser() 
        { 
            Console.WriteLine("Please enter the employee ID");
            int id;
            while (!int.TryParse(Console.ReadLine(),out id))
            {
                Console.WriteLine("Invalid entry for ID. Please enter a valid ID.");
            }
            Id = id;
            Console.WriteLine("Please enter the employee name"); 
            string name = Console.ReadLine()??""; // Null colascing
            Name = name; 
            Console.WriteLine("Please enter the employee age"); 
            int age;
            while (!int.TryParse(Console.ReadLine(),out age))
            {
                Console.WriteLine("Invalid entry for age. Please enter a valid age.");
            }
            Age = age;
            Console.WriteLine("Please enter the employee salary"); 
            double salary;
            while (!double.TryParse(Console.ReadLine(),out salary))
            {
                Console.WriteLine("Invalid entry for salary. Please enter a valid salary.");
            }
            Salary = salary; 
        } 
 
        public override string ToString() 
        { 
            return "Employee ID : " + Id + "\nName : " + Name + "\nAge : " + Age + 
"\nSalary : " + Salary; 
        } 
 
        // public int Id { get => id; set => id = value; } 
        // public int Age { get => age; set => age = value; } 
        // public string Name { get => name; set => name = value; } 
        // public double Salary { get => salary; set => salary = value; }
        // 

        public int CompareTo(Employee? other)
        {
            return  this.Id.CompareTo(other?.Id);
        }

        public bool Equals(Employee? other)
        {
            return this.Id == other?.Id;
        }
    }
}