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
        public async Task<Booking> CreateBooking(Booking NewBooking)
        {
            await dbContext.Bookings.AddAsync(NewBooking);
            await dbContext.SaveChangesAsync();

            return NewBooking;
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
        public async Task<Booking> ChangeBookingStatus(Booking NewBookingState)
        {
            dbContext.Bookings.Update(NewBookingState);
            await dbContext.SaveChangesAsync();
            return NewBookingState;
        }
    }
}