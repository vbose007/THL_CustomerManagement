using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using CustomerManagement.API.CustomExceptions;
using CustomerManagement.API.DTOs;
using CustomerManagement.API.Interfaces;
using CustomerManagement.API.Models.RequestResponseModels;
using CustomerManagement.API.Services;
using CustomerManagement.Data;
using CustomerManagement.Data.CustomExceptions;

namespace CustomerManagement.API.Controllers
{
    /***Only authenticated users can use the API ***/
    [Authorize]
    [RoutePrefix("api/Customers")]
    public class CustomersController : ApiController
    {
        private ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        // GET: api/Customers
        public GetCustomersResponseModel GetCustomers([FromUri] GetCustomersRequestModel query)
        {
            return _customerService.GetAll(query);
        }

        // GET: api/Customers/5
        [ResponseType(typeof (CustomerDto))]
        [Route("{id:int}")]
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
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult PutCustomer(int id, CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The data posted is invalid."));
                return BadRequest(ModelState);
            }

            if (id ==0 ||  id>0 && customer.Id> 0 && id!= customer.Id)
            {
                return BadRequest();
            }

            if (!CustomerExists(id))
            {
                return NotFound();
            }

            customer.Id = id;

            try
            {
                _customerService.Update(customer);

                return StatusCode(HttpStatusCode.OK);
            }
            catch (AuthorizationException ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, ex.Message));
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        // POST: api/Customers
        [ResponseType(typeof (CustomerDto))]
        [HttpPost]
        public IHttpActionResult PostCustomer([FromBody]PostCustomerRequestModel postCustomer)
        {
            if (!ModelState.IsValid || postCustomer ==null)
            {
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The data posted is invalid."));
                return BadRequest(ModelState);

            }

            try
            {
                var customerDto = Mapper.Map<CustomerDto>(postCustomer);
                customerDto =_customerService.Create(customerDto);

                postCustomer = Mapper.Map<PostCustomerRequestModel>(customerDto);

                return CreatedAtRoute("DefaultApi", new {id = customerDto.Id}, customerDto);
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
        [HttpDelete]
        [Route("{id:int}")]
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