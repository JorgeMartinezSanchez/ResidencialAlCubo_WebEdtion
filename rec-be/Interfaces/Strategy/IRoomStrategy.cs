using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Models;

namespace rec_be.Interfaces.Strategy
{
    public interface IRoomStrategy
    {
        // Datos básicos de la habitación
        string GetRoomNumber();
        bool IsOccupied();
        int GetRoomId();
        
        // Datos del tipo de habitación (vienen de BD)
        string GetRoomTypeName();
        decimal GetBasePrice();
        int GetMaxCapacity();
        
        // Comportamiento variable según el tipo
        string GetDescription();
        decimal CalculateLateCheckoutFee(int extraHours);
        bool ValidateGuestCount(int guestCount);
        
        // Método para obtener el Room original si se necesita
        Room GetRoom();
    }
}