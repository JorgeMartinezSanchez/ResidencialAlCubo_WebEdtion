using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.BookingDTOs;
using rec_be.DTOs.GuestDTOs;
using rec_be.DTOs.LateCheckOutDTO;
using rec_be.Interfaces.Strategy;

namespace rec_be.Interfaces.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDTO> CreateBooking(BookingRequestDTO bookingRequest);
        Task<bool> ValidateGuestAmount(List<GuestRequestDTO> Guests);
        Task<BookingResponseDTO> CheckIn(int bookingId);
        Task<BookingResponseDTO> CheckOut(int bookingId);
        Task<BookingResponseDTO> Cancel(int bookingId);
    }
}