﻿using System.Data;

public class Program
{
    List<Employee> employees = new List<Employee>()
        {
            new Employee(101,30,"John Doe",50000),
            new Employee(102,23,"Jane Smith",60000),
            new Employee(103,34,"Sam Brown",70000),
            new Employee(104,34,"Alex Brown",70000),
            new Employee(105,39,"Alice Brown",70000)
        };
    public delegate void MyDelegate<T>(T n1, T n2);
    // public delegate void MyFDelegate(float n1, float n2);

    public void Add(int num1, int num2)
    {
        int sum = num1 + num2;
        Console.WriteLine($"The sum of {num1} and {num2} is {sum}");
    }

    public void Product(int num1, int num2)
    {
        int prod = num1 * num2;
        Console.WriteLine($"The product of {num1} and {num2} is {prod}");
    }

    Program()
    {
        
        // MyDelegate<int> del = new MyDelegate<int>(Add);
        // Action -> Predefined delegate with no return type
        Action<int, int> del = Add;
        del += Product;

        // anonymous function
        del += delegate (int n1, int n2)
        {
            Console.WriteLine($"The difference of {n1} and {n2} is {n1 - n2}");
        };

        // lambda
        del += (int n1,int n2) =>Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
        del(10, 20);
    }

    void FindEmployee()
    {
        int empId = 102;
        Predicate<Employee> predicate = e => e.Id == empId;
        Employee? emp = employees.Find(predicate);
        Console.WriteLine($"\nEmployee found :\n{emp}" ?? "No such employee.");
    }

    void SortEmployee()
    {
        Console.WriteLine("-------------------------- Sorted Employees ------------------------");
        var sortedEmployees = employees.OrderBy(e => e.Name);
        foreach (var employee in sortedEmployees)
        {
            Console.WriteLine(employee);
        }
    }
    static void Main(string[] args)
    {
        // new Program(); // anonymous 

        // Program program = new();
        // program.FindEmployee();
        // program.SortEmployee();

        // string str1 = "Studen";
        // Console.WriteLine(str1.StringValidationCheck());

        // LINQ 101
        // 1
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

        // var lowNumbers = from number in numbers where number < 5 select number;

        // var lowNumbers = numbers.Where(number => number < 5);
        var lowNumbers = numbers.Where(number => number < 5).Select(number => number);

        Console.WriteLine("Numbers < 5 :");
        foreach (var lowNumber in lowNumbers)
        {
            Console.WriteLine(lowNumber);
        }

        // Filter elements on a property
        List<Product> products = GetProductList();

        // var soldOutProducts = from product in products where product.UnitsInStock == 0 select product;

        var soldOutProducts = products.Where(product => product.UnitsInStock == 0).Select(product => product);
        Console.WriteLine("Sold out products:");
        foreach (var soldOutProduct in soldOutProducts)
        {
            Console.WriteLine($"{soldOutProduct.ProductName} is sold out!");
        }
        Console.ReadKey();

        List<Product> GetProductList()
        {
            List<Product> productsList = new List<Product>();
            productsList.Add(new Product { ProductName = "Product 1", UnitsInStock = 3, UnitPrice = 3.00m });
            productsList.Add(new Product { ProductName = "Product 2", UnitsInStock = 0, UnitPrice = 4.00m });
            productsList.Add(new Product { ProductName = "Product 3", UnitsInStock = 5, UnitPrice = 6.00m });
            productsList.Add(new Product { ProductName = "Product 4", UnitsInStock = 1, UnitPrice = 1.00m });
            productsList.Add(new Product { ProductName = "Product 5", UnitsInStock = 0, UnitPrice = 2.00m });
            productsList.Add(new Product { ProductName = "Product 6", UnitsInStock = 2, UnitPrice = 1.00m });
            productsList.Add(new Product { ProductName = "Product 7", UnitsInStock = 0, UnitPrice = 7.00m });
            productsList.Add(new Product { ProductName = "Product 8", UnitsInStock = 3, UnitPrice = 8.00m });


            return productsList;
        }

        // filter elements on multiple properties

        List<Product> products2 = GetProductList();

        // var expensiveInStockProducts = from product in products2
        //                       where product.UnitsInStock > 0 && product.UnitPrice > 3.00M
        //                       select product;

        var expensiveInStockProducts = products2.Where(product => (product.UnitsInStock > 0 && product.UnitPrice > 3.00M)).Select(product => product);

        Console.WriteLine("In-stock products that cost more than 3.00:");
        foreach (var expensiveInStockProduct in expensiveInStockProducts)
        {
            Console.WriteLine($"{expensiveInStockProduct.ProductName} is in stock and costs more than 3.00.");
        }
        Console.ReadKey();

        // Examine a Sequence Property of Output Elements

        List<Customer> customers = GetCustomerList();

        //var waCustomers = from customer in customers
        //                  where customer.Region == "WA"
        //                  select customer;

        var waCustomers = customers.Where(customer => customer.Region == "WA").Select(customer => customer);

        Console.WriteLine("Customers from Washington and their orders:");
        foreach (var wacustomer in waCustomers)
        {
            Console.WriteLine($"Customer {wacustomer.CustomerID}: {wacustomer.CompanyName}");
            foreach (var wacustomerOrder in wacustomer.Orders)
            {
                Console.WriteLine($"  Order {wacustomerOrder.OrderID}: {wacustomerOrder.OrderDate}");
            }
        }
        Console.ReadKey();

        List<Customer> GetCustomerList()
        {
            List<Customer> customerList = new List<Customer>()
            {
                new Customer() { CustomerID = 1, CompanyName="Nintendo", Region="WA",
                    Orders = new List<Order>() { new Order() { OrderID = 1, OrderDate = DateTime.Now },
                                                new Order() { OrderID = 2, OrderDate = DateTime.Now },
                                                new Order() { OrderID = 3, OrderDate = DateTime.Today},
                                                new Order() { OrderID = 4, OrderDate = DateTime.Today}
                                                }
                            },
                new Customer() { CustomerID = 2, CompanyName="Sony", Region="CA",
                    Orders = new List<Order>() { new Order() { OrderID = 5, OrderDate = DateTime.Now },
                                                new Order() { OrderID = 6, OrderDate = DateTime.Now },
                                                new Order() { OrderID = 7, OrderDate = DateTime.Today},
                                                new Order() { OrderID = 8, OrderDate = DateTime.Today}
                                                }
                                },
                new Customer() { CustomerID = 3, CompanyName="Sega", Region="WA",
                    Orders = new List<Order>() { new Order() { OrderID = 9, OrderDate = DateTime.Now },
                                                new Order() { OrderID = 10, OrderDate = DateTime.Now },
                                                new Order() { OrderID = 11, OrderDate = DateTime.Today},
                                                new Order() { OrderID = 12, OrderDate = DateTime.Today}
                                                }
                            }

            };
            return customerList;
        }
        
        // Filter elements based on position

        string[] digits = { "zero", "one", "two", "three", "four", "five", "six", 
            "seven", "eight", "nine" };

        var shortDigits = digits.Where((digit, index) => digit.Length < index);

        Console.WriteLine("Short digits:");
        foreach (var shortDigit in shortDigits)
        {
            Console.WriteLine($"The word {shortDigit} is shorter than its value.");
        }
        Console.ReadKey();

    }
}