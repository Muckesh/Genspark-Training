using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserFollow
{
    [Key]
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public int FollowingId { get; set; }
    public DateTime FollowedAt { get; set; } = DateTime.Now;

    // Nav props
    [ForeignKey("FollowerId")]
    public User? Follower { get; set; }

    [ForeignKey("FollowingId")]
    public User? Following { get; set; }
}