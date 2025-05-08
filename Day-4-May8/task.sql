use northwind;

select * from Orders;
select * from Customers;
select * from Employees;

-- 1) List all orders with the customer name and the employee who handled the order. (Join Orders, Customers, and Employees)
select o.orderID as OrderId,
	c.CompanyName as CompanyName,
	c.ContactName as CustomerName, 
	concat(e.FirstName,' ',e.LastName) as EmployeeName 
	from Orders o 
	join Customers c on o.CustomerID = c.CustomerID 
	join Employees e on o.EmployeeID = e.EmployeeID
	order by o.OrderID;

-- 2) Get a list of products along with their category and supplier name. (Join Products, Categories, and Suppliers)
select * from Products;
select * from Categories;
select * from Suppliers;

select p.ProductId, p.ProductName, c.CategoryName, s.CompanyName as SupplierName from Products p
join Categories c on p.CategoryID = c.CategoryID join Suppliers s
on p.SupplierID = s.SupplierID
order by p.ProductID;

-- 3) Show all orders and the products included in each order with quantity and unit price. (Join Orders, Order Details, Products)
select * from Orders;
select * from [Order Details];
select * from Products;

select o.OrderID, p.ProductName, od.Quantity, od.UnitPrice
from Orders o join [Order Details] od on o.OrderID = od.OrderID
join Products p on od.ProductID = p.ProductID
order by o.OrderID;

-- 4) List employees who report to other employees (manager-subordinate relationship). (Self join on Employees)
select * from Employees;

select CONCAT(e.FirstName,' ',e.LastName) as Employee, CONCAT(m.FirstName,' ',m.LastName) as Manager from Employees e 
join Employees m on e.ReportsTo = m.EmployeeID order by m.EmployeeID;

-- 5) Display each customer and their total order count. (Join Customers and Orders, then GROUP BY)
select * from Customers;
select * from Orders;

select c.CompanyName as Customer, COUNT(o.OrderID) as TotalOrders from Customers c left join 
Orders o on c.CustomerID = o.CustomerID group by c.CompanyName;

-- 6) Find the average unit price of products per category. Use AVG() with GROUP BY

select * from Products;
select * from Categories;

select c.CategoryName as Category, AVG(p.UnitPrice) as Avg_Unit_Price from Categories c join Products p 
on p.CategoryID = c.CategoryID group by c.CategoryName order by Avg_Unit_Price desc;

-- 7) List customers where the contact title starts with 'Owner'. Use LIKE or LEFT(ContactTitle, 5)
select * from Customers;

select CustomerID, CompanyName, ContactName, ContactTitle from Customers where ContactTitle like 'Owner%';

select CustomerID, CompanyName, ContactName, ContactTitle from Customers where LEFT(ContactTitle,5) = 'Owner';

-- 8) Show the top 5 most expensive products.Use ORDER BY UnitPrice DESC and TOP 5
select TOP 5 ProductID, ProductName, UnitPrice from Products order by UnitPrice desc;

-- 9) Return the total sales amount (quantity × unit price) per order. Use SUM(OrderDetails.Quantity * OrderDetails.UnitPrice) and GROUP BY
select * from [Order Details];
select OrderId, SUM(Quantity*UnitPrice) as TotalSalesAmount from [Order Details] group by OrderID;

-- 10) Create a stored procedure that returns all orders for a given customer ID. Input: @CustomerID

select * from Customers;
select * From Orders;

create or alter proc proc_GetOrdersByCustomerId (@CustomerId nvarchar(20))
as
begin
	select * from Orders where CustomerID = @CustomerId order by OrderDate desc;
end
proc_GetOrdersByCustomerId 'ALFKI'

-- 11) Write a stored procedure that inserts a new product. Inputs: ProductName, SupplierID, CategoryID, UnitPrice, etc.
select * from Products;

create proc proc_InsertProduct(
	@ProductName NVARCHAR(40),
    @SupplierID INT,
    @CategoryID INT,
    @QuantityPerUnit NVARCHAR(20),
    @UnitPrice MONEY,
    @UnitsInStock SMALLINT,
    @UnitsOnOrder SMALLINT,
    @ReorderLevel SMALLINT,
    @Discontinued BIT
)
as
begin
	insert into Products (
		ProductName,
        SupplierID,
        CategoryID,
        QuantityPerUnit,
        UnitPrice,
        UnitsInStock,
        UnitsOnOrder,
        ReorderLevel,
        Discontinued
	) values (
		@ProductName,
        @SupplierID,
        @CategoryID,
        @QuantityPerUnit,
        @UnitPrice,
        @UnitsInStock,
        @UnitsOnOrder,
        @ReorderLevel,
        @Discontinued
	)
end

EXEC proc_InsertProduct 
    @ProductName = 'New Chai',
    @SupplierID = 1,
    @CategoryID = 1,
    @QuantityPerUnit = '10 boxes x 20 bags',
    @UnitPrice = 18.00,
    @UnitsInStock = 50,
    @UnitsOnOrder = 0,
    @ReorderLevel = 10,
    @Discontinued = 0;

select * from Products;

-- 12) Create a stored procedure that returns total sales per employee. Join Orders, Order Details, and Employees

select * from Orders;
select * from [Order Details];
select * from Employees;

create proc proc_TotalSalesPerEmployee
as
begin
	select 
	e.EmployeeId,
	concat(e.FirstName,' ',e.LastName) as EmployeeName,
	SUM(od.Quantity * od.UnitPrice) as TotalSales
	from Employees e join Orders o on e.EmployeeID = o.EmployeeID
	join [Order Details] od on o.OrderID = od.OrderID 
	group by e.EmployeeID, e.FirstName, e.LastName
	order by TotalSales desc;
end

EXEC proc_TotalSalesPerEmployee;

-- 13) Use a CTE to rank products by unit price within each category. Use ROW_NUMBER() or RANK() with PARTITION BY CategoryID
with ProductRankings as (
select ProductID,
        ProductName,
        CategoryID,
        UnitPrice,
		ROW_NUMBER() over (partition by CategoryId order by UnitPrice desc) as PriceRank
		from Products
)
select * from ProductRankings order by CategoryID, PriceRank;

with ProductRankings as (
select ProductID,
        ProductName,
        CategoryID,
        UnitPrice,
		Rank() over (partition by CategoryId order by UnitPrice desc) as PriceRank
		from Products
)
select * from ProductRankings order by CategoryID, PriceRank;

-- 14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.
with ProductRevenue as (
    select 
        p.ProductID,
        p.ProductName,
        SUM(od.Quantity * od.UnitPrice) as TotalRevenue
    from [Order Details] od
    join Products p on od.ProductID = p.ProductID
    group by p.ProductID, p.ProductName
)
select * from ProductRevenue
where TotalRevenue > 10000
order by TotalRevenue desc;

-- 15) Use a CTE with recursion to display employee hierarchy. Start from top-level employee (ReportsTo IS NULL) and drill down

with EmployeeHierarchy as (
    -- this is an anchor member: top-level employees (no manager)
    select 
        EmployeeID,
        FirstName + ' ' + LastName as EmployeeName,
        ReportsTo,
        0 as level
    from Employees
    where ReportsTo IS NULL

    union ALL
    -- this is a recursive member: employees reporting to someone
    select 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName,
        e.ReportsTo,
        eh.level + 1
    from Employees e
    join EmployeeHierarchy eh on e.ReportsTo = eh.EmployeeID
)
select * 
from EmployeeHierarchy
order by level, EmployeeName;
