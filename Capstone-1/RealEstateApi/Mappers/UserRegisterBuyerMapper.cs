using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Mappers
{
    public class UserRegisterBuyerMapper
    {
        public User MapUserBuyer( RegisterBuyerDto buyerDto, string hashedPassword, string refreshToken)
        {
            User user = new User();
            user.Name = buyerDto.Name;
            user.Email = buyerDto.Email;
            user.PasswordHash = hashedPassword;
            user.Role = "Buyer";
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            return user;
        }

        public Buyer BuyerUserMapper(RegisterBuyerDto buyerDto, User user)
        {
            Buyer buyer = new Buyer();
            buyer.Id = user.Id;
            buyer.Phone = buyerDto.Phone;
            buyer.Budget = buyerDto.Budget;
            buyer.PreferredLocation = buyerDto.PreferredLocation;

            return buyer;
        }
    }
}