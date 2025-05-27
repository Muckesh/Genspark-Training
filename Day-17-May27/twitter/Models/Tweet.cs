using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Tweet
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;


    [ForeignKey("UserId")]
    public User? User { get; set; }

    // nav props
    public ICollection<Like>? Likes { get; set; }
    public ICollection<TweetHashtag>? TweetHashtags { get; set; }
}