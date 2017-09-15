using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.API.Models.RequestResponseModels
{
    public class PostCustomerRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}