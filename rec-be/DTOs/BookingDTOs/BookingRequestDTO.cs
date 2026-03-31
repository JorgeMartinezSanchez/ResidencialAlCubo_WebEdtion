using System.ComponentModel.DataAnnotations;

namespace rec_be.DTOs.BookingDTOs
{
    public class BookingRequestDTO
    {
        [Required]
        public int RoomId { get; init; }
        [Required] 
        public DateOnly StartDate { get; init; }
        [Required]
        public DateOnly EndDate { get; init; }
        [Required]
        public string Status { get; init; } = "";
        public DateTime CheckInDate { get; init; }
        [Required]
        public decimal Total { get; init; }
    }
}