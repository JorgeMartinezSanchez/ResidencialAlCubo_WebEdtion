using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Strategy;

namespace rec_be.Interfaces.Services
{
    public interface IBookingService
    {
        Task ValidateGuestAmount(List<GuestRequestDTO> Guests);
        Task<decimal> CalculateLateCheckout(IRoomStrategy Room);
    }
}