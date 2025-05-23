
public enum Role
{
    Admin,
    User,
    Guest
}
public class User
{
    public string UserName { get; set; }
    public Role UserRole { get; set; }

    public User(string username, Role role)
    {
        UserName = username;
        UserRole = role;
    }
}

