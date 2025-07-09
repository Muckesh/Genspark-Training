using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IPropertyListingService
    {
        Task<PropertyListing> AddListingAsync(CreatePropertyListingDto listingDto);
        Task<PagedResult<PropertyListingResponseDto>> GetFilteredListingsAsync(PropertyListingQueryParametersDto query);
        Task<PropertyListing> UpdateListingAsync(Guid id, UpdatePropertyListingDto listingDto);

        Task<PropertyListing> DeleteListingAsync(Guid id);
        Task<IEnumerable<PropertyListing>> GetAllListings();
        Task<PropertyListingResponseDto> GetListingByIdAsync(Guid id);

    }
}