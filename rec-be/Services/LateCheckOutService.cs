using rec_be.DTOs.BookingDTOs;
using rec_be.DTOs.LateCheckOutDTO;
using rec_be.Interfaces.Factory;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Interfaces.Strategy;
using rec_be.Models;

namespace rec_be.Services
{
    public class LateCheckOutService : ILateCheckOutService
    {
        private readonly ILateCheckOutRepository _lateCheckOutRepo;
        private readonly IBookingRepository      _bookingRepo;
        private readonly IRoomRepository         _roomRepo;
        private readonly IConfigRepository       _configRepo;
        private readonly IRoomStrategyFactory    _strategyFactory;

        public LateCheckOutService(
            ILateCheckOutRepository lateCheckOutRepo,
            IBookingRepository      bookingRepo,
            IRoomRepository         roomRepo,
            IConfigRepository       configRepo,
            IRoomStrategyFactory    strategyFactory)
        {
            _lateCheckOutRepo = lateCheckOutRepo;
            _bookingRepo      = bookingRepo;
            _roomRepo         = roomRepo;
            _configRepo       = configRepo;
            _strategyFactory  = strategyFactory;
        }

        // ── HU-08: Crear registro de Late Check-Out ───────────────────
        public async Task<LateCheckOutResponseDTO> CreateLateCheckOut(LateCheckOutRequestDTO newLateCheckOut)
        {
            var booking = await _bookingRepo.GetBooking(newLateCheckOut.BookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            var rateKvp = await _configRepo.GetConfigByKey("LateCheckOutHourlyRate");
            decimal rate = decimal.Parse(rateKvp.Value);

            IRoomStrategy strategy = _strategyFactory.CreateStrategy(room, rate);
            decimal charge = strategy.CalculateLateCheckoutFee(newLateCheckOut.ExtraHours);

            var newLCO = new LateCheckOut
            {
                BookingId  = newLateCheckOut.BookingId,
                ExtraHours = newLateCheckOut.ExtraHours,
                Charge     = charge
            };

            var result = await _lateCheckOutRepo.CreateLateCheckout(newLCO);

            return new LateCheckOutResponseDTO
            {
                BookingId  = result.BookingId,
                ExtraHours = result.ExtraHours,
                Charge     = result.Charge
            };
        }

        public async Task<decimal> CalculateTotalCharge(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var lcos = await _lateCheckOutRepo.GetLateCheckOutsFromBookingId(booking.Id);

            if (lcos == null || lcos.Count == 0)
                return 0.0m;

            var total = lcos.Sum(lco => lco.Charge);
            booking.Total = total;

            await _bookingRepo.ChangeBookingStatus(booking);
            return total;
        }

        // En LateCheckOutService.cs
        public async Task<List<LateCheckOutResponseDTO>> GetLateCheckOutsByBookingId(int bookingId)
        {
            var lcos = await _lateCheckOutRepo.GetLateCheckOutsFromBookingId(bookingId);
            
            return lcos.Select(lco => new LateCheckOutResponseDTO
            {
                BookingId = lco.BookingId,
                ExtraHours = lco.ExtraHours,
                Charge = lco.Charge
            }).ToList();
        }
    }
}