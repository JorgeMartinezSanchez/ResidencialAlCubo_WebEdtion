using System.ComponentModel.DataAnnotations;

namespace rec_be.DTOs.LateCheckOutDTO
{
    public class LateCheckOutRequestDTO
    {
        [Required]
        public int BookingId { get; init; }
        [Required]
        public int ExtraHours { get; init; }
    }
}