using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CustomerManagement.API.DTOs;
using CustomerManagement.Data.Models;

namespace CustomerManagement.API.Config
{
    public class ApiMapperConfig
    {
        public static void ConfigureMappings()
        {

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CustomerModel, CustomerDto>().ReverseMap();
            });
            
        }
    }
}
