using AutoMapper;
using CustomerManagement.API.DTOs;
using CustomerManagement.API.Models.RequestResponseModels;
using CustomerManagement.Data.Config;
using CustomerManagement.Data.Models;

namespace CustomerManagement.API.Config
{
    public class ApiMapperConfig
    {
        public static void ConfigureMappings()
        {

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new CustomerRepositoryMapProfile());
                cfg.AddProfile(new CustomerApiMapProfile());
            });
        }
    }

    public class CustomerApiMapProfile : Profile
    {
        public CustomerApiMapProfile()
        {
            CreateMap<CustomerModel, CustomerDto>().ReverseMap();
            CreateMap<PostCustomerRequestModel, CustomerDto>()
            .ForMember(dest => dest.Id, op => op.Ignore());

            CreateMap<CustomerDto, PostCustomerRequestModel>()
                .ForMember(dest => dest.Name, op => op.MapFrom(s => s.Name))
                .ForMember(dest => dest.EmailAddress, op => op.MapFrom(s => s.EmailAddress));
        }
    }
}
