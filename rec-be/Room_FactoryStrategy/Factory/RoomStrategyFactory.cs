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
                throw new ArgumentException("La habitación no tiene un tipo válido");
            
            return room.RoomType.TypeName switch
            {
                "Simple" => new SimpleRoomStrategy(room, lateCheckoutRate),
                "Suite" => new SuiteRoomStrategy(room, lateCheckoutRate),
                "Doble Matrimonial" => new MatrimonialDoubleRoomStrategy(room, lateCheckoutRate),
                "Doble Individual" => new IndividualDoubleBedRoomStrategy(room, lateCheckoutRate),
                "Grupal 3" => new GroupalForThreePeopleRoomStrategy(room, lateCheckoutRate),
                "Grupal 4" => new GroupalForFourPeopleRoomStrategy(room, lateCheckoutRate),
                _ => throw new ArgumentException(
                    $"Tipo de habitación no soportado: {room.RoomType.TypeName}")
            };
        }
    }
}