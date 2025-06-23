public interface IFileProcessingService
{
    public Task<FileUploadReturnDto> ProcessData(CsvUploadDto csvUploadDto);
}