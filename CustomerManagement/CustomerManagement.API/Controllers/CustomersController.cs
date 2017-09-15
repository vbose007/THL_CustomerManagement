using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CustomerManagement.API.DTOs;
using CustomerManagement.API.Interfaces;
using CustomerManagement.API.Models.Request;
using CustomerManagement.API.Services;
using CustomerManagement.Data;
using CustomerManagement.Data.CustomExceptions;

namespace CustomerManagement.API.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService = new CustomersCustomerService(new CustomerRepository());

        // GET: api/Customers
        public GetCustomersResponseModel GetCustomers([FromUri] GetCustomersRequestModel query)
        {
            return _customerService.GetAll(query);
        }

        // GET: api/Customers/5
        [ResponseType(typeof (CustomerDto))]
        public IHttpActionResult GetCustomer(int id)
        {
            var customerDto = _customerService.GetById(id);
            if (customerDto == null)
            {
                return NotFound();
            }

            return Ok(customerDto);
        }

        // PUT: api/Customers/5
        [ResponseType(typeof (void))]
        public IHttpActionResult PutCustomer(int id, CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.Id)
            {
                return BadRequest();
            }

            if (!CustomerExists(id))
            {
                return NotFound();
            }


            try
            {
                _customerService.Update(customer);

                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        // POST: api/Customers
        [ResponseType(typeof (CustomerDto))]
        public IHttpActionResult PostCustomer(CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _customerService.Create(customer);

                return CreatedAtRoute("DefaultApi", new {id = customer.Id}, customer);
            }
            catch (AccountWithSameEmailExistsException ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message));
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof (CustomerDto))]
        public IHttpActionResult DeleteCustomerDto(int id)
        {
            var customerDto = _customerService.GetById(id);

            if (customerDto == null)
            {
                return NotFound();
            }

            try
            {
                if (_customerService.Delete(customerDto))
                {
                    return Ok(customerDto);
                }
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message));
            }

            return StatusCode(HttpStatusCode.InternalServerError);
        }

        private bool CustomerExists(int id)
        {
            return _customerService.GetById(id) != null;
        }
    }
}