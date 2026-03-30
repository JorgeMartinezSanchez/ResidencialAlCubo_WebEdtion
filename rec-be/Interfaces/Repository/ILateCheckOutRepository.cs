using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface ILateCheckOutRepository
    {
        Task<List<LateCheckOut>> GetLateCheckOutsFromBookingId(int BookingId);
        Task<LateCheckOut> CreateLateCheckout(LateCheckOut NewLateCheckout);
    }
}