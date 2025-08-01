using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using Newtonsoft.Json;

namespace Ecommerce.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly IRepository<int, ContactUs> _contactUsRepository;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string GOOGLE_VERIFY_URL = "https://www.google.com/recaptcha/api/siteverify";

        public ContactUsService(IRepository<int, ContactUs> contactUsRepository, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _contactUsRepository = contactUsRepository;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ContactResponseDto> CreateContact(ContactRequestDto contact)
        {
            // var contacts = await _contactUsRepository.GetAllAsync();
            // var existing = contacts.SingleOrDefault(c => string.Equals(c.ColorName, newColor.ColorName, StringComparison.OrdinalIgnoreCase));
            // if (existing != null)
            //     throw new Exception("Color Already Exists.");
            ContactUs contactUs = new ContactUs
            {
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Content = contact.Content
            };
            contactUs = await _contactUsRepository.AddAsync(contactUs);
            return new ContactResponseDto
            {
                Id = contactUs.Id,
                Name = contactUs.Name,
                Email = contactUs.Email,
                Phone = contactUs.Phone,
                Content = contactUs.Content
            };
        }

        public async Task<ContactResponseDto> DeleteContact(int id)
        {
            var contact = await _contactUsRepository.DeleteAsync(id);
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Content = contact.Content
            };
        }

        public async Task<IEnumerable<ContactUs>> GetAllContacts()
        {
            var contacts = await _contactUsRepository.GetAllAsync();
            return contacts;
        }


        public async Task<ContactResponseDto> GetContactById(int id)
        {
            var contact = await _contactUsRepository.GetByIdAsync(id);
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Content = contact.Content
            };
        }

        public async Task<ContactResponseDto> UpdateContact(int id, ContactRequestDto updateDto)
        {
            var contact = await _contactUsRepository.GetByIdAsync(id);
            if (contact == null)
                throw new KeyNotFoundException("Contact not found.");
            contact.Name = updateDto.Name;
            contact.Email = updateDto.Email;
            contact.Phone = updateDto.Phone;
            contact.Content = contact.Content;

            contact = await _contactUsRepository.UpdateAsync(id, contact);
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Content = contact.Content
            };
        }
        
        public async Task<CaptchaResponseDto> VerifyTokenAsync(string token)
        {
            var secret = _config["reCaptcha:SecretKey"];
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{GOOGLE_VERIFY_URL}?secret={secret}&response={token}");

            return JsonConvert.DeserializeObject<CaptchaResponseDto>(response) ?? new CaptchaResponseDto { Success = false };
        }
    }
}