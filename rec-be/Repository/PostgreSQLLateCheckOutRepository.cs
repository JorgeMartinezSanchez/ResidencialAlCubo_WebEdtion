using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rec_be.Data;
using rec_be.Interfaces.Repository;
using rec_be.Models;

namespace rec_be.Repository
{
    public class PostgreSQLLateCheckOutRepository : ILateCheckOutRepository
    {
        protected RACPostgreSQLDbContext dbContext;

        public PostgreSQLLateCheckOutRepository (RACPostgreSQLDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<LateCheckOut> CreateLateCheckout(LateCheckOut NewLateCheckout)
        {
            await dbContext.LateCheckOuts.AddAsync(NewLateCheckout);
            await dbContext.SaveChangesAsync();
            return NewLateCheckout;
        }

        public async Task<List<LateCheckOut>> GetLateCheckOutsFromBookingId(int BookingId)
        {
            var LateCheckOutList = await dbContext.LateCheckOuts.Where(lco => lco.BookingId == BookingId).ToListAsync();
            if (LateCheckOutList == null)
            {
                throw new Exception("No Late Check-Out was not found in this booking yayyyy!!!1!");
            }
            else
            {
                return LateCheckOutList;
            }
        }
    }
}