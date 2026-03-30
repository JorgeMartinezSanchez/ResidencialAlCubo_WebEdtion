using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Strategy;
using rec_be.Models;

namespace rec_be.RoomStrategy
{
    public class SimpleRoomStrategy : BaseRoomStrategy
    {
        public SimpleRoomStrategy(Room room, decimal lateCheckoutRate) 
            : base(room, lateCheckoutRate) { }
        
        public override string GetDescription() => 
            "Habitación económica, ideal para viajeros solitarios";
        
        public override decimal CalculateLateCheckoutFee(int extraHours) =>
            extraHours * (_lateCheckoutRate * 0.8m); // 20% menos
        
        public override bool ValidateGuestCount(int guestCount) =>
            guestCount == 1 && guestCount <= GetMaxCapacity();
    }
}