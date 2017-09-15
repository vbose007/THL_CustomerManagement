namespace CustomerManagement.API.Models.RequestResponseModels
{
    public class GetCustomersRequestModel
    {
        public string SortBy { get; set; }
        public bool SortAsc { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}