using System.ComponentModel.DataAnnotations;

public class Hashtag
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Nav props
    public ICollection<TweetHashtag>? TweetHashtags { get; set; }
}