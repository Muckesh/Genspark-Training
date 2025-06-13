namespace RealEstateApi.Models.DTOs
{
    public class UserQueryDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        public string? SortBy { get; set; }
        public bool IsDescending { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}