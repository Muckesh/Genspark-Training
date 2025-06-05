public interface IFileService
{
    Task<Document> SaveFile(IFormFile file, string uploadedByEmail);
    Task<Document?> GetFile(Guid id);
}
