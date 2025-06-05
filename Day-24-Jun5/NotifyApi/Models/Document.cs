public class Document
{
    public int Id { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public byte[]? DocumentContent { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public int UploadedById { get; set; }

    public Employee? UploadedBy { get; set; }
}