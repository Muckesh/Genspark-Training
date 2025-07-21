namespace VideoPortalAPi.Models.DTOs
{
    public class FileUploadDto
    {
        public IFormFile Video { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}