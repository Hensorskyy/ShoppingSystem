using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Data;
using ShoppingSystem.Models;
using ShoppingSystem.Services;

namespace ShoppingSystem.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomers _customers;

        public CustomerController(ICustomers customers)
        {
            _customers = customers;
        }

        // GET: api/Customers
        /// <summary>
        /// Gets all customer from database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            return Ok(await _customers.GetAllAsync());
        }

        // GET: api/Customers/5
        /// <summary>
        /// Get a customer by passed id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            try
            {
                var customer = await _customers.GetByIdAsync(id);
                return customer;
            }
            catch(Exception)
            {
                return NotFound();
            }

            
        }

        // PUT: api/Customer/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Updates customer by id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }
            try
            {
                await _customers.EditAsync(customer);
            }
            catch
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Adds new customer to database.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /AddCustomer
        ///     {
        ///        "FirstName": "Name",
        ///        "LastName": "SurName",
        ///        "Address": "Lviv",
        ///        "Discount": "10"
        ///     }
        ///
        /// </remarks>
        /// <returns>A newly created customer</returns>
        /// <response code="201">Returns the newly created customer</response>
        /// <response code="400">If the item is null</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(Customer customer)
        {
            await _customers.AddAsync(customer);

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Products/5
        /// <summary>
        /// Removes a customer from database.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            Customer customer;
            try
            {
                customer = await _customers.GetByIdAsync(id);
                await _customers.DeleteAsync(id);
            }
            catch
            {
                return BadRequest();
            }


            return customer;
        }

    }
}
