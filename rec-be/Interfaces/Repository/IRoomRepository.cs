using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllRooms();
        Task<Room> GetRoomByRoomNumber(string RoomNumber);
        Task SetRoomOccupation(int RoomId, bool Occupation);
        Task<List<Room>> GetAllAvailableRooms();
        Task<List<Room>> GetAllAvailableRooms(string RoomType);
    }
}