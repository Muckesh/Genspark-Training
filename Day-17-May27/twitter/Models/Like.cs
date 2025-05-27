using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Like
{
    [Key]
    public int Id { get; set; }
    public int TweetId { get; set; }
    public int UserId { get; set; }
    public DateTime LikedAt { get; set; } = DateTime.Now;

    // Foreign keys
    [ForeignKey("TweetId")]
    public Tweet? Tweet { get; set; }
    [ForeignKey("UserId")]
    public User? User { get; set; }
}