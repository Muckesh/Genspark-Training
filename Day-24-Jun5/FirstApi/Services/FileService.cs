
public class FileService : IFileService
{
    private static readonly Dictionary<Guid, FileModel> _fileStore = new();
    private readonly string _uploadDir;

    public FileService(IWebHostEnvironment env)
    {
        var projectRoot = env.ContentRootPath;
        _uploadDir = Path.Combine(projectRoot, "Uploads");
        if (!Directory.Exists(_uploadDir))
        {
            Directory.CreateDirectory(_uploadDir);
        }
    }
    public FileModel? GetFile(Guid id)
    {
        if (!_fileStore.TryGetValue(id, out var fileMeta))
            return null;

        var fileExtension = Path.GetExtension(fileMeta.FileName);
        var filePath = Path.Combine(_uploadDir, $"{id}{fileExtension}");

        if (!File.Exists(filePath))
            return null;

        fileMeta.Data = File.ReadAllBytes(filePath); // Load when needed
        return fileMeta;
    }

    public async Task<FileModel> SaveFile(IFormFile file)
    {
        var id = Guid.NewGuid();
        var fileExtension = Path.GetExtension(file.FileName);
        var storedFileName = $"{id}{fileExtension}";
        var filePath = Path.Combine(_uploadDir, storedFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var uploadedFile = new FileModel
        {
            Id = id,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Data = Array.Empty<byte>() // Not used when storing physically
        };

        _fileStore[id] = uploadedFile;
        return uploadedFile;
    }
}