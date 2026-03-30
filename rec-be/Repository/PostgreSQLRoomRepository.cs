using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rec_be.Data;
using rec_be.DTOs.RoomDTOs;
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
            var room = await dbContext.Rooms.FirstOrDefaultAsync(room => room.RoomNumber == RoomNumber);
            if(room == null)
            {
                throw new Exception($"The {RoomNumber} Room was not found or doesn't exist.");
            } else
            {
                return room;
            }
        }
        public async Task<List<Room>> GetAllRoomsFromRoomType(string RoomType)
        {
            return await dbContext.Rooms
                        .Include(r => r.RoomType)
                        .Where(r => r.RoomType.TypeName == RoomType)
                        .ToListAsync();
        }
        public async Task SetRoomOccupation(Room TargetRoom)
        {
            dbContext.Rooms.Update(TargetRoom);
            await dbContext.SaveChangesAsync();
        }
        public async Task<Room> GetRoomById(int RoomId)
        {
            var result = await dbContext.Rooms.FindAsync(RoomId);
            if(result == null) throw new Exception($"ROOM REPOSITORY ERROR: No room with {RoomId} id was found in room table.");
            return result;
        }
    }
}