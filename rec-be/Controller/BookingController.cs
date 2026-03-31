using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rec_be.DTOs.BookingDTOs;
using rec_be.Interfaces.Services;
using rec_be.Models;

namespace rec_be.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private IBookingService bookingService;

        public BookingController(IBookingService _bookingService)
        {
            bookingService = _bookingService;
        }
        
        [HttpPost("Create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDTO bookingRequest)
        {
            try
            {
                // Validate that at least one guest is selected
                if (bookingRequest.GuestIds == null || !bookingRequest.GuestIds.Any())
                {
                    return BadRequest("At least one guest must be selected for the booking.");
                }
                
                var booking = await bookingService.CreateBooking(bookingRequest, bookingRequest.GuestIds);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await bookingService.GetAllBookings();
            return Ok(bookings);
        }
    }
}