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
            "Groupal room for 3 people.";
        
        public override decimal CalculateLateCheckoutFee(int extraHours) =>
            extraHours * (_lateCheckoutRate * 1.1m);
        
        public override bool ValidateGuestCount(int guestCount) =>
            guestCount >= 1 && guestCount <= GetMaxCapacity();
        public override decimal ApplyDiscountOnFridays(DateTime date, decimal bookingTotal)
        {
            if(date.DayOfWeek == DayOfWeek.Friday)
            {
                return (80 * bookingTotal) / 100;
            } else
            {
                return 0.0m;
            }
        }
    }
}