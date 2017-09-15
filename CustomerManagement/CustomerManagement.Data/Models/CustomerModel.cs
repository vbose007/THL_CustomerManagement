using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CustomerManagement.Data.Interfaces;

namespace CustomerManagement.Data.Models
{
    public class CustomerModel : IDomainModel<Customer>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public string CreatedBy { get; set; }

        public Customer ToModel()
        {
           return Mapper.Map<Customer>(this);
        }

        public IDomainModel<Customer> FromModel(Customer model)
        {
            return Mapper.Map<CustomerModel>(model);
        }

        public Customer UpdateModel(Customer model)
        {
            var updatedModel = this.ToModel();
            
            //Ensuring not overwrite actual Entity's Id value
            updatedModel.Id = model.Id;

            return updatedModel;
        }

        public object GetKey()
        {
            return this.Id;
        }

    }
}
