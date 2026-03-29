using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IRoomRepository
    {
        Task<Room> GetRoomByRoomNumber(string RoomNumber);
        Task SetRoomOccupation(bool Occupation);
        Task<List<Room>> GetAllAvailableRooms();
        Task<List<Room>> GetAllAvailableRooms(string RoomType);
    }
}