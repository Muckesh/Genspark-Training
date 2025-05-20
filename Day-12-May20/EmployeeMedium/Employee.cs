class Employee : IComparable<Employee>
{
    public int Id { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    public double Salary { get; set; }

    public Employee() { }

    public Employee(int id, int age, string name, double salary)
    {
        Id = id;
        Age = age;
        Name = name;
        Salary = salary;
    }

    public void TakeEmployeeDetailsFromUser()
    {
        Console.Write("Enter ID: ");
        Id = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Name: ");
        Name = Console.ReadLine();

        Console.Write("Enter Age: ");
        Age = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Salary: ");
        Salary = Convert.ToDouble(Console.ReadLine());
    }

    // Medium : Q2
    public int CompareTo(Employee? other)
    {
        if (other == null) return 1;
        return other.Salary.CompareTo(this.Salary); // Descending order
    }

    public override string ToString()
    {
        return $"ID: {Id}\nName: {Name}\nAge: {Age}\nSalary: {Salary}";
    }
}