using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.RoomDTOs;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Models;

namespace rec_be.Services
{
    public class RoomService : IRoomService
    {
        protected IRoomRepository roomRepository;

        public RoomService(IRoomRepository _roomRepository)
        {
            roomRepository = _roomRepository;
        }

        public async Task<List<RoomResponseDTO>> GetAllRooms()
        {
            var roomResult = await roomRepository.GetAllRooms();
            List<RoomType> roomTypes = await roomRepository.GetAllRoomTypes();
            if(roomResult == null) throw new Exception("ROOM SERVICE ERROR: Fuck it we ball.");
            List<RoomResponseDTO> resultDto = new List<RoomResponseDTO>();
            foreach(Room r in roomResult)
            {
                foreach(RoomType t in roomTypes)
                {
                    if(r.RoomTypeId == t.Id)
                    {
                        resultDto.Add(new RoomResponseDTO
                        {
                           Id = r.Id,
                           RoomNumber = r.RoomNumber,
                           RoomType = t.TypeName,
                           Capacity = t.Capacity,
                           Price = t.Price,
                           Occupied = r.Occupied
                        });
                    }
                }
            }
            return resultDto;
        }

        public
    }
}