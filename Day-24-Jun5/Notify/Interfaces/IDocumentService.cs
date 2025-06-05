public interface IDocumentService
{
    Task<string> UploadDocumentAsync(DocumentUploadRequestDto file, string uploader);
    Task<DocumentResponseDto> GetDocumentsAsync(int id);
}
