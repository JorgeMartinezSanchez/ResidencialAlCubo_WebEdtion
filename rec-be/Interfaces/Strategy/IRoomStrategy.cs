using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;

namespace rec_be.Interfaces.Strategy
{
    public interface IRoomStrategy
    {
        string GetRoomNumber();
        bool IsOccupied();
        decimal GetPrice();
    }
}