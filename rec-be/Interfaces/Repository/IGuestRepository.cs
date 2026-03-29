using rec_be.DTOs.GuestDTOs;
using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IGuestRepository
    {
        Task<List<Guest>> CreateGuests(List<GuestRequestDTO> guests);
        Task<Guest> CreateSingleGuest(GuestRequestDTO guest);
        Task<List<Guest>> GetAllCurrentGuests();
        Task<List<Guest>> GetAllGuests();
        Task<List<Guest>> GetGuestsByRoomId(int RoomId);
        Task DeleteGuestFromRoom(int GuestId, int RoomId);
        Task DeleteGuestFromDatabase(int GuestId);
    }
}