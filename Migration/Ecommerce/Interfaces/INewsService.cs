using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface INewsService
    {
        Task<NewsResponseDto> CreateNews(NewsRequestDto news);
        Task<IEnumerable<NewsResponseDto>> GetAllNews();
        Task<NewsResponseDto> GetNewsById(int id);
        Task<NewsResponseDto> UpdateNews(int id, NewsUpdateRequestDto updateDto);
        Task<NewsResponseDto> DeleteNews(int id);
        Task<byte[]> ExportContentToCSVAsync();
        Task<byte[]> ExportContentToExcelAsync();
    }
}