// Client → Proxy → RealSubject (Actual Object)

// - Both Proxy and RealSubject implement a common interface

class Program
{
    static void Main(string[] args)
    {
        var users = new List<User>
        {
            new User("Alice",Role.Admin),
            new User("John",Role.User),
            new User("Bob",Role.Guest)
        };

        foreach (var user in users)
        {
            Console.WriteLine($"\nUser : {user.UserName} | Role : {user.UserRole}");
            IFile proxyFile = new ProxyFile(user);
            proxyFile.Read();
        }
    }
}