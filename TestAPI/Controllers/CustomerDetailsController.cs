
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Data;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDetailsController : ControllerBase
    {
        private readonly CustomerContext _context;

        public CustomerDetailsController(CustomerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get List of customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<CustomerDetail>>> GetList()
        {
            return await _context.CustomerDetail.ToListAsync();
        }

        /// <summary>
        /// Get one customer detail using Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<CustomerDetail>> GetById(int id)
        {
            var customerDetail = await _context.CustomerDetail.FindAsync(id);

            if (customerDetail == null)
            {
                return NotFound();
            }

            return customerDetail;
        }

        /// <summary>
        /// Add new customer
        /// </summary>
        /// <param name="customerDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public async Task<ActionResult<CustomerDetail>> Add(CustomerDetail customerDetail)
        {
            _context.CustomerDetail.Add(customerDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = customerDetail.Id }, customerDetail);
        }

        /// <summary>
        /// Update customer  detail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerDetail"></param>
        /// <returns></returns>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, CustomerDetail customerDetail)
        {
            if (id != customerDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(customerDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<CustomerDetail>> Delete(int id)
        {
            var customerDetail = await _context.CustomerDetail.FindAsync(id);
            if (customerDetail == null)
            {
                return NotFound();
            }

            _context.CustomerDetail.Remove(customerDetail);
            await _context.SaveChangesAsync();

            return customerDetail;
        }

       /// <summary>
       /// Search customer by first name or last name
       /// </summary>
       /// <param name="searchText"></param>
       /// <returns></returns>
        [HttpGet("SearchByName/{SearchText}")]
        public async Task<ActionResult<IEnumerable<CustomerDetail>>> SearchByName(string searchText)
        {
            var customerDetail = await _context.CustomerDetail.Where(x => searchText.ToLower().Contains(x.FirstName.ToLower()) || searchText.ToLower().Contains(x.LastName.ToLower())).ToListAsync(); 

            if (customerDetail.Count() == 0)
            {
                return NotFound();
            }

            return customerDetail;
        }

        /// <summary>
        /// Check if the customer detail is exists or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool CustomerDetailExists(int id)
        {
            return _context.CustomerDetail.Any(e => e.Id == id);
        }
    }
}
