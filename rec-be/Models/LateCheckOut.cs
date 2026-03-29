using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rec_be.Models
{
    public class LateCheckOut
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int ExtraHours { get; set; }
        public decimal Charge { get; set; }

        // Navegación
        public Booking Booking { get; set; } = null!;
    }
}