using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rec_be.Data;
using rec_be.DTOs.BookingDTOs;
using rec_be.Interfaces.Repository;
using rec_be.Models;

namespace rec_be.Repository
{
    public class PostgreSQLBookingRepository : IBookingRepository
    {
        protected RACPostgreSQLDbContext dbContext;
        public PostgreSQLBookingRepository(RACPostgreSQLDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<Booking> CreateBooking(BookingRequestDTO NewBooking)
        {
            Booking booking = new Booking
            {
                RoomId = NewBooking.RoomId,
                StartDate = NewBooking.StartDate,
                EndDate = NewBooking.EndDate,
                Status = NewBooking.Status,
                Total = NewBooking.Total
            };

            await dbContext.Bookings.AddAsync(booking);
            await dbContext.SaveChangesAsync();

            return booking;
        }
        public async Task<List<Booking>> GetAllBookings()
        {
            return await dbContext.Bookings.ToListAsync();
        }
        public async Task<Booking> GetBooking(int BookingId)
        {
            var booking = await dbContext.Bookings.FirstOrDefaultAsync(booking => booking.Id == BookingId);
            if(booking == null)
            {
                throw new Exception("No booking was found.");
            } else
            {
                return booking;
            }
        }
        public async Task<Booking> ChangeBookingStatus(BookingRequestDTO NewBookingState)
        {
            Booking booking = new Booking
            {
                RoomId = NewBookingState.RoomId,
                StartDate = NewBookingState.StartDate,
                EndDate = NewBookingState.EndDate,
                Status = NewBookingState.Status,
                Total = NewBookingState.Total
            };

            dbContext.Bookings.Update(booking);
            await dbContext.SaveChangesAsync();
            return booking;
        }
        public async Task<LateCheckOut> GetLateCheckOut(int BookingId)
        {
            var lateCheckOut = await dbContext.LateCheckOuts.FirstOrDefaultAsync(booking => booking.Id == BookingId);
            if (lateCheckOut == null)
            {
                throw new Exception("Late Check-Out was not found in this booking yayyyy!!!1!");
            }
            else
            {
                return lateCheckOut;
            }
        }
    }
}