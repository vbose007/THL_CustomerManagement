using System.Collections.Generic;
using CustomerManagement.API.DTOs;

namespace CustomerManagement.API.Models.Request
{
    public class GetCustomersResponseModel
    {
        public IList<CustomerDto> CustomerDtos { get; set; }

        public string NextPageUrl { get; set; }

        public string PreviousPageUrl { get; set; }
    }
}