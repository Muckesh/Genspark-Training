public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[]? FileContent { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int UploadedById { get; set; }
    public Employee? UploadedBy { get; set; }
}