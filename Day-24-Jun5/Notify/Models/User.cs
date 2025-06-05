using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public string UserName { get; set; } = string.Empty;
    public byte[]? Password { get; set; }

    public byte[]? HashKey { get; set; }
    public string Role { get; set; } = string.Empty;
    public Employee? Employee { get; set; }

}