using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rec_be.DTOs.GuestDTOs;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Models;

namespace rec_be.Services
{
    public class GuestService : IGuestService
    {
        protected IGuestRepository guestRepository;

        public GuestService(IGuestRepository _guestRepository)
        {
            guestRepository = _guestRepository;
        }

        public async Task<GuestResponseDTO> AddGuest(GuestResponseDTO guest)
        {
            Guest newGuest = new Guest
            {
                FirstName = guest.FirstName,
                SecondName = guest.SecondName,
                LastName = guest.LastName,
                IdCard = guest.IDCard,
                PhoneNumber = guest.PhoneNumber,
                Email = guest.Email
            };
            bool exists = await guestRepository.GuestExists(newGuest);
            if (exists) throw new Exception($"GUEST SERVICE ERROR: Guest named {newGuest.FirstName} {newGuest.LastName} already exists");

            Guest fetchedNewGuest = await guestRepository.CreateSingleGuest(newGuest);
            return new GuestResponseDTO
            {
                Id = fetchedNewGuest.Id,
                FirstName = fetchedNewGuest.FirstName,
                SecondName = fetchedNewGuest.SecondName,
                LastName = fetchedNewGuest.LastName,
                IDCard = fetchedNewGuest.IdCard,
                PhoneNumber = fetchedNewGuest.PhoneNumber,
                Email = fetchedNewGuest.Email
            };   
        }

        public async Task<List<GuestResponseDTO>> AddGuestList(List<GuestRequestDTO> guests)
        {
            List<GuestResponseDTO> guestsResponse = new List<GuestResponseDTO>();
            foreach (GuestRequestDTO g in guests)
            {
                Guest newGuest = new Guest
                {
                    FirstName = g.FirstName,
                    SecondName = g.SecondName,
                    LastName = g.LastName,
                    IdCard = g.IDCard,
                    PhoneNumber = g.PhoneNumber,
                    Email = g.Email
                };
                bool exists = await guestRepository.GuestExists(newGuest);
                if (!exists)
                {
                    Guest fetchedNewGuest = await guestRepository.CreateSingleGuest(newGuest);
                    guestsResponse.Add(new GuestResponseDTO
                    {
                        Id = fetchedNewGuest.Id,
                        FirstName = fetchedNewGuest.FirstName,
                        SecondName = fetchedNewGuest.SecondName,
                        LastName = fetchedNewGuest.LastName,
                        IDCard = fetchedNewGuest.IdCard,
                        PhoneNumber = fetchedNewGuest.PhoneNumber,
                        Email = fetchedNewGuest.Email
                    });
                } else
                {
                    Guest fetchedNewGuest = await guestRepository.GetGuest(newGuest);
                    guestsResponse.Add(new GuestResponseDTO
                    {
                        Id = fetchedNewGuest.Id,
                        FirstName = fetchedNewGuest.FirstName,
                        SecondName = fetchedNewGuest.SecondName,
                        LastName = fetchedNewGuest.LastName,
                        IDCard = fetchedNewGuest.IdCard,
                        PhoneNumber = fetchedNewGuest.PhoneNumber,
                        Email = fetchedNewGuest.Email
                    });
                }
            }
            return guestsResponse;
        }
        public async Task<List<GuestResponseDTO>> GetGuestsFromBookingId(int BookingId)
        {
            var result = await guestRepository.GetGuestsByBookingId(BookingId);
            if (result == null) throw new Exception("GUEST SERVICE ERROR: Guests in this booking are empty.");
            List<GuestResponseDTO> resultDto = new List<GuestResponseDTO>();
            foreach (Guest g in result)
            {
                resultDto.Add(new GuestResponseDTO
                {
                    FirstName = g.FirstName,
                    SecondName = g.SecondName,
                    LastName = g.LastName,
                    IDCard = g.IdCard,
                    PhoneNumber = g.PhoneNumber,
                    Email = g.Email
                });
            }
            return resultDto;
        }
    }
}