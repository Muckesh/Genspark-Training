using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IContactUsService
    {
        Task<ContactResponseDto> CreateContact(ContactRequestDto contact);
        Task<IEnumerable<ContactUs>> GetAllContacts();
        Task<ContactResponseDto> GetContactById(int id);
        Task<ContactResponseDto> UpdateContact(int id, ContactRequestDto updateDto);
        Task<ContactResponseDto> DeleteContact(int id);
        Task<CaptchaResponseDto> VerifyTokenAsync(string token);
    }
}