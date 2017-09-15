using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CustomerManagement.Data.Models;

namespace CustomerManagement.Data.Config
{
    public class RepositoryMapperConfig
    {
        public void ConfigureMappings()
        {

            Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<Customer, CustomerModel>().ReverseMap();
                cfg.AddProfile(new CustomerRepositoryMapProfile());
            });
            
        }
    }

    public class CustomerRepositoryMapProfile : Profile
    {
        public CustomerRepositoryMapProfile()
        {
            CreateMap<Customer, CustomerModel>().ReverseMap();
        }
    }
}
