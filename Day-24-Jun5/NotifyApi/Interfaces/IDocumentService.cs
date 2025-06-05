public interface IDocumentService
{
    Task<string> PostFile(DocumentUploadDto fileItem, string userName);
    public Task<DocumentGetDto> DownloadFileById(int id);
}