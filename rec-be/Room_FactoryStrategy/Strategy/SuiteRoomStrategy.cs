using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Strategy;
using rec_be.Models;

namespace rec_be.RoomStrategy
{
    public class SuiteRoomStrategy : BaseRoomStrategy
    {
        public SuiteRoomStrategy(Room room, decimal lateCheckoutRate) 
            : base(room, lateCheckoutRate) { }
        
        public override string GetDescription() => 
            "Luxurious Suite room with jacuzzi.";
        
        public override decimal CalculateLateCheckoutFee(int extraHours) =>
            extraHours * (_lateCheckoutRate * 1.5m);
        
        public override bool ValidateGuestCount(int guestCount) =>
            guestCount >= 1 && guestCount <= GetMaxCapacity();

        public override decimal ApplyDiscountOnFridays(DateTime date, decimal bookingTotal) { return 0.0m; }
    }
}