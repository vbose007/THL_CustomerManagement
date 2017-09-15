using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.API.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }


    }
}