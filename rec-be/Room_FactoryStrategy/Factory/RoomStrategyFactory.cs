using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.Interfaces.Factory;
using rec_be.Interfaces.Strategy;
using rec_be.Models;
using rec_be.RoomStrategy;

namespace rec_be.Room_FactoryStrategy.Factory
{
    public class RoomStrategyFactory : IRoomStrategyFactory
    {
        public IRoomStrategy CreateStrategy(Room room, decimal lateCheckoutRate)
        {
            if (room?.RoomType == null)
                throw new ArgumentException("Room doesnt have a valid type.");
            
            return room.RoomType.TypeName switch
            {
                "Simple" => new SimpleRoomStrategy(room, lateCheckoutRate),
                "Suite" => new SuiteRoomStrategy(room, lateCheckoutRate),
                "Matrimonial Double" => new MatrimonialDoubleRoomStrategy(room, lateCheckoutRate),
                "Individual Bed Double" => new IndividualDoubleBedRoomStrategy(room, lateCheckoutRate),
                "Groupal for 3 people" => new GroupalForThreePeopleRoomStrategy(room, lateCheckoutRate),
                "Groupal for 4 people" => new GroupalForFourPeopleRoomStrategy(room, lateCheckoutRate),
                _ => throw new ArgumentException(
                    $"Room type not supported: {room.RoomType.TypeName}")
            };
        }
    }
}