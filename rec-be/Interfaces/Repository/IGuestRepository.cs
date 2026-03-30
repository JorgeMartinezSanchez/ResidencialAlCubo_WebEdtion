using rec_be.DTOs.GuestDTOs;
using rec_be.Models;

namespace rec_be.Interfaces.Repository
{
    public interface IGuestRepository
    {
        Task<List<Guest>> CreateGuests(List<Guest> guests);
        Task<Guest> CreateSingleGuest(Guest guest);
        Task<List<Guest>> GetAllActiveGuests();
        Task<List<Guest>> GetAllPendantGuests();
        Task<List<Guest>> GetAllGuests();
        Task<List<Guest>> GetGuestsByRoomId(int RoomId);
        Task DeleteGuestFromRoom(int GuestId, int BookingId);
        Task DeleteGuestFromDatabase(int GuestId);
    }
}