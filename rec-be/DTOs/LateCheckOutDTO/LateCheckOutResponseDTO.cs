using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rec_be.DTOs.LateCheckOutDTO
{
    public class LateCheckOutResponseDTO
    {
        public int BookingId { get; init; }
        public int ExtraHours { get; init; }
        public decimal Charge { get; init; }
    }
}