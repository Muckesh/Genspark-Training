namespace RealEstateApi.Models.DTOs
{
    public class AddPropertyImageDto
    {
        public Guid PropertyListingId { get; set; }
        public IFormFile ImageFile { get; set; } = null!;
    }
}