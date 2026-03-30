using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Strategy;
using rec_be.Models;

namespace rec_be.RoomStrategy
{
    public class GroupalForThreePeopleRoomStrategy : BaseRoomStrategy
    {
        public GroupalForThreePeopleRoomStrategy(Room room, decimal lateCheckoutRate) 
            : base(room, lateCheckoutRate) { }
        
        public override string GetDescription() => 
            "Habitación grupal para 3 personas, con camas individuales";
        
        public override decimal CalculateLateCheckoutFee(int extraHours) =>
            extraHours * (_lateCheckoutRate * 1.1m); // 10% más
        
        public override bool ValidateGuestCount(int guestCount) =>
            guestCount >= 1 && guestCount <= GetMaxCapacity();
    }
}