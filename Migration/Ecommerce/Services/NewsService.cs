using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using RealEstateApi.Exceptions;


namespace Ecommerce.Services
{
    public class NewsService : INewsService
    {
        private readonly IRepository<int, News> _newsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NewsService(IRepository<int, News> newsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _newsRepository = newsRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<NewsResponseDto> CreateNews(NewsRequestDto news)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/news");

            var newsList = await _newsRepository.GetAllAsync();
            var existing = newsList.SingleOrDefault(n => string.Equals(n.Title, news.Title, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new Exception("News already exists.");
            var originalFileName = Path.GetFileName(news.Image.FileName);
            var extension = Path.GetExtension(originalFileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (!allowedExtensions.Contains(extension.ToLower()))
                throw new FailedOperationException("Unsupported image format.");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(basePath, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await news.Image.CopyToAsync(stream);
            }

            News newNews = new News
            {
                UserId = news.UserId,
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Image = $"/uploads/news/{uniqueName}",
                Content = news.Content,
                CreatedDate = DateTime.UtcNow,
                // CreatedDate = news.CreatedDate,
                Status = news.Status

            };
            newNews = await _newsRepository.AddAsync(newNews);

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                throw new NotFoundException("HTTP context not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var imageUrl = $"{baseUrl}{newNews.Image}";

            return new NewsResponseDto
            {
                NewsId = newNews.NewsId,
                UserId = newNews.UserId,
                Title = newNews.Title,
                ShortDescription = newNews.ShortDescription,
                Image = imageUrl,
                Content = newNews.Content,
                // CreatedDate = DateTime.UtcNow,
                CreatedDate = newNews.CreatedDate,
                Status = newNews.Status
            };
        }

        public async Task<NewsResponseDto> DeleteNews(int id)
        {
            var news = await _newsRepository.DeleteAsync(id);
            return new NewsResponseDto
            {
                NewsId = news.NewsId,
                UserId = news.UserId,
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Image = news.Image,
                Content = news.Content,
                // CreatedDate = DateTime.UtcNow,
                CreatedDate = news.CreatedDate,
                Status = news.Status
            };
        }

        public async Task<IEnumerable<NewsResponseDto>> GetAllNews()
        {
            var allNews = await _newsRepository.GetAllAsync();
            var newsList = new List<NewsResponseDto>();
            foreach (var news in allNews)
            {
                var request = _httpContextAccessor.HttpContext?.Request;

                if (request == null)
                    throw new NotFoundException("HTTP context not available.");

                var baseUrl = $"{request.Scheme}://{request.Host}";

                var imageUrl = $"{baseUrl}{news.Image}";
                var newsResponse = new NewsResponseDto
                {
                    NewsId = news.NewsId,
                    UserId = news.UserId,
                    Title = news.Title,
                    ShortDescription = news.ShortDescription,
                    // Image = news.Image,
                    Image = imageUrl,
                    Content = news.Content,
                    // CreatedDate = DateTime.UtcNow,
                    CreatedDate = news.CreatedDate,
                    Status = news.Status
                };
                newsList.Add(newsResponse);
            }
            return newsList;

        }

        public async Task<NewsResponseDto> GetNewsById(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                throw new NotFoundException("HTTP context not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var imageUrl = $"{baseUrl}{news.Image}";
            return new NewsResponseDto
            {
                NewsId = news.NewsId,
                UserId = news.UserId,
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Image = imageUrl,
                Content = news.Content,
                // CreatedDate = DateTime.UtcNow,
                CreatedDate = news.CreatedDate,
                Status = news.Status
            };
        }

        public async Task<NewsResponseDto> UpdateNews(int id, NewsUpdateRequestDto updateDto)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/news");

            var news = await _newsRepository.GetByIdAsync(id);
            news.UserId = updateDto.UserId;
            news.Title = updateDto.Title;
            news.ShortDescription = updateDto.ShortDescription;
            news.Content = updateDto.Content;

            if (updateDto.Image!=null)
            {
                var originalFileName = Path.GetFileName(updateDto.Image.FileName);
                var extension = Path.GetExtension(originalFileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (!allowedExtensions.Contains(extension.ToLower()))
                    throw new FailedOperationException("Unsupported image format.");

                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(basePath, uniqueName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.Image.CopyToAsync(stream);
                }

                news.Image = $"/uploads/news/{uniqueName}";
            }
            news.Status = updateDto.Status;
            var updatedNews = await _newsRepository.UpdateAsync(id, news);

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                throw new NotFoundException("HTTP context not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var imageUrl = $"{baseUrl}{updatedNews.Image}";
            return new NewsResponseDto
            {
                NewsId = updatedNews.NewsId,
                UserId = updatedNews.UserId,
                Title = updatedNews.Title,
                ShortDescription = updatedNews.ShortDescription,
                Image = imageUrl,
                Content = updatedNews.Content,
                // CreatedDate = DateTime.UtcNow,
                CreatedDate = updatedNews.CreatedDate,
                Status = updatedNews.Status
            };
        }

        public async Task<byte[]> ExportContentToCSVAsync()
        {
            var newsList = await _newsRepository.GetAllAsync();
            var sb = new StringBuilder();

            // CSV header
            sb.AppendLine("\"NewsId\",\"Title\",\"ShortDescription\",\"CreatedDate\",\"Status\"");

            // CSV rows
            foreach (var news in newsList.OrderBy(x => x.NewsId))
            {
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                    news.NewsId,
                    news.Title?.Replace("\"", "\"\""),
                    news.ShortDescription?.Replace("\"", "\"\""),
                    news.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                    news.Status));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        
        public async Task<byte[]> ExportContentToExcelAsync()
        {
            var newsList = await _newsRepository.GetAllAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("News Listing");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "NewsId";
                worksheet.Cell(currentRow, 2).Value = "Title";
                worksheet.Cell(currentRow, 3).Value = "Short Description";
                worksheet.Cell(currentRow, 4).Value = "Created Date";
                worksheet.Cell(currentRow, 5).Value = "Status";

                // Rows
                foreach (var news in newsList.OrderBy(x => x.NewsId))
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = news.NewsId;
                    worksheet.Cell(currentRow, 2).Value = news.Title;
                    worksheet.Cell(currentRow, 3).Value = news.ShortDescription;
                    worksheet.Cell(currentRow, 4).Value = news.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cell(currentRow, 5).Value = news.Status;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
       
    }
}