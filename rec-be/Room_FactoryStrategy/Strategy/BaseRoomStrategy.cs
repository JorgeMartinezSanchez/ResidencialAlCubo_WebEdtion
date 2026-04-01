using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.Interfaces.Strategy;
using rec_be.Models;

namespace rec_be.RoomStrategy
{
    public abstract class BaseRoomStrategy : IRoomStrategy
    {
        protected readonly Room _room;
        protected readonly decimal _lateCheckoutRate;
        
        protected BaseRoomStrategy(Room room, decimal lateCheckoutRate)
        {
            _room = room;
            _lateCheckoutRate = lateCheckoutRate;
        }
        
        public string GetRoomNumber() => _room.RoomNumber;
        public bool IsOccupied() => _room.Occupied;
        public int GetRoomId() => _room.Id;
        public string GetRoomTypeName() => _room.RoomType?.TypeName ?? "desconocido";
        public decimal GetBasePrice() => _room.RoomType?.Price ?? 0;
        public int GetMaxCapacity() => _room.RoomType?.Capacity ?? 1;
        public Room GetRoom() => _room;
        
        public abstract string GetDescription();
        public abstract decimal CalculateLateCheckoutFee(int extraHours);
        public abstract bool ValidateGuestCount(int guestCount);

        // Discount
        public abstract decimal ApplyDiscountOnFridays(DateTime date, decimal bookingTotal); 
    }
}