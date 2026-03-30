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
    public class PostgreSQLRoomRepository : IRoomRepository
    {
        protected RACPostgreSQLDbContext dbContext;

        public PostgreSQLRoomRepository(RACPostgreSQLDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<List<Room>> GetAllRooms()
        {
            return await dbContext.Rooms.ToListAsync();
        }
        public async Task<Room> GetRoomByRoomNumber(string RoomNumber)
        {
            var room = await dbContext.Rooms.FindAsync(RoomNumber);
            if(room == null)
            {
                throw new Exception($"The {RoomNumber} Room was not found or doesn't exist.");
            } else
            {
                return room;
            }
        }
        public async Task SetRoomOccupation(int RoomId, bool Occupation)
        {
            var selectedRoom = await dbContext.Rooms.FindAsync(RoomId);
            if (selectedRoom == null)
            {
                throw new Exception("");
            } else
            {
                selectedRoom.Occupied = Occupation;
                dbContext.Rooms.Update(selectedRoom);
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<Room>> GetAllAvailableRooms()
        {
            var avialbleRooms = await dbContext.Rooms.Where(rooms => !rooms.Occupied).ToListAsync();
            if(avialbleRooms == null)
            {
                throw new Exception("Oops! Seems like all rooms are occupied.");
            } else
            {
                return avialbleRooms;
            }
        }
        public async Task<List<Room>> GetAllAvailableRooms(string RoomType)
        {
            var availableRooms = await dbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.RoomType.TypeName == RoomType && r.Occupied == false)
                .ToListAsync();

            if (!availableRooms.Any())
            {
                throw new Exception("Oops! Seems like all rooms are occupied.");
            }

            return availableRooms;
        }
    }
}