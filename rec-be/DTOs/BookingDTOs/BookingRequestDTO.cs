namespace rec_be.DTOs.BookingDTOs
{
    public class BookingRequestDTO
    {
        public int RoomId { get; init; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public string Status { get; init; } = "";
        public decimal Total { get; init; }
    }
}