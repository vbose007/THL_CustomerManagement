using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using CustomerManagement.Data.Config;
using CustomerManagement.Data.CustomExceptions;
using CustomerManagement.Data.Helpers;
using CustomerManagement.Data.Interfaces;
using CustomerManagement.Data.Models;

namespace CustomerManagement.Data
{
    public class CustomerRepository : IRepository<Customer, CustomerModel>
    {
        private CustomerDbContext db;
        public CustomerRepository()
        {
            new RepositoryMapperConfig().ConfigureMappings();

            db = new CustomerDbContext();
            
        }

        public ICollection<CustomerModel> GetAll()
        {
            return Mapper.Map<List<CustomerModel>>(db.Customers.ToList());
        }

        public ICollection<CustomerModel> Get(Expression<Func<Customer, bool>> where = null, Expression<Func<Customer, object>> orderBy = null, bool orderAsc = true, int pageSize = 10, int pageNumber = 1)
        {
            var skipItemCount = pageSize*(pageNumber -1);

            var pagedCustomers = GetPagedCustomersAsQueriable(@where, orderBy, orderAsc, pageSize, skipItemCount);
            return Mapper.Map<IQueryable<CustomerModel>>(pagedCustomers).ToList();
        }

        private IQueryable<Customer> GetPagedCustomersAsQueriable(Expression<Func<Customer, bool>> @where = null, Expression<Func<Customer, object>> orderBy = null, bool orderAsc =true, int pageSize =10, int pageNumber=1)
        {
            var skipItemCount = pageSize * (pageNumber - 1);

            IQueryable<Customer> selectedCustomers = null;

            selectedCustomers = @where != null ? db.Customers.Where(@where) : db.Customers;

            if (@orderBy != null)
            {
                selectedCustomers = orderAsc
                    ? selectedCustomers.OrderBy(@orderBy)
                    : selectedCustomers.OrderByDescending(@orderBy);
            }

            var pagedCustomers = selectedCustomers.Skip(skipItemCount).Take(pageSize);
            return pagedCustomers;
        }

        public CustomerModel Get(object key)
        {
            if (key is int && (int)key >0)
            {
                var customer = db.Customers.SingleOrDefault(x => x.Id == (int) key);
                return Mapper.Map<CustomerModel>(customer);
            }

            if (key is string && EmailValidator.IsValid(key.ToString()))
            {
                var customer = db.Customers.SingleOrDefault(x => x.EmailAddress .Equals(key.ToString(), StringComparison.OrdinalIgnoreCase));
                return Mapper.Map<CustomerModel>(customer);

            }

            return null;
        }

        
        public int Count(Expression<Func<Customer, bool>> @where = null)
        {
            if (where != null)
            {
                return db.Customers.Count(where);
            }

            return db.Customers.Count();
        }

        public CustomerModel Add(CustomerModel model)
        {
            if (model == null) return null;

            var customer = model.ToModel();

            var existingCustomerWithSameEmail = Get(customer.EmailAddress);

            if (existingCustomerWithSameEmail != null)
            {
                throw new AccountWithSameEmailExistsException($"An account with {customer.EmailAddress} already exists");
            }

            db.Customers.Add(customer);
            db.SaveChanges();
            return (CustomerModel) model.FromModel(customer) ;
        }

        public int Update(CustomerModel model)
        {
            if (model == null || model.Id==0) return 0;

            var customer = model.ToModel();

            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
            return 1;
        }

        public int Delete(object key)
        {
            var customer = Get(key)?.ToModel();

            if (customer != null)
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
                return 1;
            }

            return 0;
        }

        public int Delete(Expression<Func<Customer, bool>> @where)
        {
            var customersToDelete = GetPagedCustomersAsQueriable(@where: @where);
            int count = 0;

            customersToDelete.ToList().ForEach(x =>
            {
                try
                {
                    db.Customers.Remove(x);
                    count++;
                }
                catch (Exception e)
                {
                    //TODO Log error
                }
            });

            return count;
        }
    }
}