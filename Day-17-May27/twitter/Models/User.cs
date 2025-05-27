using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Nav prop
    public ICollection<Tweet>? Tweets { get; set; }
    public ICollection<Like>? Likes { get; set; }
    public ICollection<User>? Followers { get; set; }
    public ICollection<User>? Following { get; set; }

}