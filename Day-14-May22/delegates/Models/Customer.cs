public class Customer
{
    public int CustomerID { get; set; }
    public string CompanyName { get; set; }
    public string Region { get; set; }
    public List<Order> Orders { get; set; }
}