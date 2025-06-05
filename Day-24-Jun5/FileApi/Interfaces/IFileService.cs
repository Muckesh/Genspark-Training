public interface IFileService
{
    Task<FileModel> SaveFile(IFormFile file);
    FileModel? GetFile(Guid id);
}