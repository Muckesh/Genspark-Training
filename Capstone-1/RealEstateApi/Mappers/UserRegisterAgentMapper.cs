using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Mappers
{
    public class UserRegisterAgentMapper
    {
        public User MapUserAgent(RegisterAgentDto agentDto, string hashedPassword, string refreshToken)
        {
            User user = new User();
            user.Name = agentDto.Name;
            user.Email = agentDto.Email;
            user.PasswordHash = hashedPassword;
            user.Role = "Agent";
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            return user;
        }

        public Agent AgentUserMapper(RegisterAgentDto agentDto, User user)
        {
            Agent agent = new Agent();
            agent.Id = user.Id;
            agent.AgencyName = agentDto.AgencyName;
            agent.LicenseNumber = agentDto.LicenseNumber;
            agent.Phone = agentDto.Phone;

            return agent;
        }
    }
}