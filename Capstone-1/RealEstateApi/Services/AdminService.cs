using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AdminService
    {
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, PropertyListing> _propertyListingRepository;
        private IRepository<Guid, Inquiry> _inquiryRepository;

        public AdminService(IRepository<Guid, User> userRepository, IRepository<Guid, PropertyListing> propertyListingRepository, IRepository<Guid, Inquiry> inquiryRepository)
        {
            _userRepository = userRepository;
            _propertyListingRepository = propertyListingRepository;
            _inquiryRepository = inquiryRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var listings = await _propertyListingRepository.GetAllAsync();
            var inquiries = await _inquiryRepository.GetAllAsync();

            return new DashboardStatsDto
            {
                TotalUsers = users.Count(u => !u.IsDeleted),
                TotalAgents = users.Count(u => u.Role == "Agent" && !u.IsDeleted),
                TotalBuyers = users.Count(u => u.Role == "Buyer" && !u.IsDeleted),
                TotalListings = listings.Count(l => !l.IsDeleted),
                TotalInquiries = inquiries.Count()
            };
        }
    }
}