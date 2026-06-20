using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Models;

namespace rec_be.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<GuestResponseDTO> AddGuest(GuestRequestDTO guest)
        {
            var newGuest = MapToModel(guest);

            if (await _guestRepository.GuestExists(newGuest))
                throw new Exception($"GUEST SERVICE ERROR: A guest with ID card {newGuest.IdCard} already exists.");

            var created = await _guestRepository.CreateSingleGuest(newGuest);
            return MapToDTO(created);
        }

        public async Task<List<GuestResponseDTO>> AddGuestList(List<GuestRequestDTO> guests)
        {
            var results = new List<GuestResponseDTO>();

            foreach (var g in guests)
            {
                var newGuest = MapToModel(g);
                var result   = await _guestRepository.GuestExists(newGuest)
                    ? await _guestRepository.GetGuest(newGuest)
                    : await _guestRepository.CreateSingleGuest(newGuest);

                results.Add(MapToDTO(result));
            }

            return results;
        }

        public async Task<List<GuestResponseDTO>> GetGuestsFromBookingId(int bookingId)
        {
            var guests = await _guestRepository.GetGuestsByBookingId(bookingId);
            if (guests == null || guests.Count == 0)
                throw new Exception("GUEST SERVICE ERROR: No guests found for this booking.");

            return guests.Select(MapToDTO).ToList();
        }

        public async Task<List<GuestResponseDTO>> GetAllTheGuests()
        {
            var result = await _guestRepository.GetAllGuests();
            List<GuestResponseDTO> resultDtos = new List<GuestResponseDTO>();

            foreach(var guest in result)
            {
                resultDtos.Add(MapToDTO(guest));
            }

            return resultDtos;
        }

        private static Guest MapToModel(GuestRequestDTO g) => new Guest
        {
            FirstName = g.FirstName,
            SecondName = g.SecondName,
            LastName = g.LastName,
            IdCard = g.IDCard,
            PhoneNumber = g.PhoneNumber,
            Email = g.Email
        };

        private static GuestResponseDTO MapToDTO(Guest g) => new GuestResponseDTO
        {
            Id = g.Id,
            FirstName = g.FirstName,
            SecondName = g.SecondName,
            LastName = g.LastName,
            IDCard = g.IdCard,
            PhoneNumber = g.PhoneNumber,
            Email = g.Email
        };
    }
}