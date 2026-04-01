using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rec_be.DTOs.LateCheckOutDTO;
using rec_be.Interfaces.Services;

namespace rec_be.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class LateCheckoutController : ControllerBase
    {
        private readonly ILateCheckOutService lateCheckOutService;

        public LateCheckoutController(ILateCheckOutService _lateCheckOutService)
        {
            lateCheckOutService = _lateCheckOutService;
        }

        [HttpPost("new/latecheckout")]
        public async Task<IActionResult> NewLateCheckout([FromBody] LateCheckOutRequestDTO newLateCheckOut)
        {
            var latecheckout = await lateCheckOutService.CreateLateCheckOut(newLateCheckOut);
            return Ok(latecheckout);
        }

        [HttpGet("Totalcost/{BookingId}")]
        public async Task<IActionResult> GetTotalOfLateCheckoutFromABooking(int BookingId)
        {
            var total = await lateCheckOutService.CalculateTotalCharge(BookingId);
            return Ok(total);
        }
    }
}