using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Email { get; set; }=string.Empty;
        public string Phone { get; set; }=string.Empty;
        public string Content { get; set; }=string.Empty;

    }
}