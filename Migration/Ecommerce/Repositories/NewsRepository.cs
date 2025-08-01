using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class NewsRepository : Repository<int, News>
    {
        public NewsRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {

        }

        public override async Task<ICollection<News>> GetAllAsync()
        {
            var newsList = await _ecommerceDbContext.News.ToListAsync();
            return newsList;
        }

        public override async Task<News> GetByIdAsync(int key)
        {
            var news = await _ecommerceDbContext.News.SingleOrDefaultAsync(n => n.NewsId == key);
            return news ?? throw new KeyNotFoundException("News not found.");
        }
    }
}