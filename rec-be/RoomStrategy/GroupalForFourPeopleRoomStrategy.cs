using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Strategy;
using rec_be.Models;

namespace rec_be.RoomStrategy
{
    public class GroupalForFourPeopleRoomStrategy : IRoomStrategy
    {
        private readonly Room room;
        private readonly decimal lateCheckoutRate;
        public GroupalForFourPeopleRoomStrategy(Room _room, decimal _lateCheckoutRate)
        {
            room = _room;
            lateCheckoutRate = _lateCheckoutRate;
        }
 
        public string GetRoomNumber() => room.RoomNumber;
 
        public bool IsOccupied() => room.Occupied;
 
        public decimal GetPrice() => room.RoomType.Price;
        public decimal CalculateLateCheckOutCharge() =>
            room.RoomType.Price * lateCheckoutRate;
 
        public bool ValidateGuests(List<GuestRequestDTO> guests) =>
            guests.Count > 0 && guests.Count <= room.RoomType.Capacity;
 
        public bool ValidatePayment(decimal amount) =>
            amount >= room.RoomType.Price;
    }
}