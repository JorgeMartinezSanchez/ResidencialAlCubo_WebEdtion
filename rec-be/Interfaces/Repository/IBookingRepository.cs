using rec_be.DTOs.BookingDTOs;
using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBooking(BookingRequestDTO NewBooking);
        Task<List<Booking>> GetAllBookings();
        Task<Booking> GetBooking(BookingRequestDTO BookingRequest);
        Task<Booking> ChangeBookingStatus(BookingRequestDTO NewBookingState);
        Task<LateCheckOut> GetLateCheckOut(int BookingId);
    }
}