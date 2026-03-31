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
        // El cargo varía según el tipo de habitación (Strategy pattern).
        public async Task<LateCheckOutResponseDTO> CreateLateCheckOut(LateCheckOutRequestDTO newLateCheckOut)
        {
            // Necesitamos la habitación para seleccionar la estrategia correcta
            var booking = await _bookingRepo.GetBooking(newLateCheckOut.BookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            // Leer la tarifa base desde configuración
            var rateKvp = await _configRepo.GetConfigByKey("LateCheckOutHourlyRate");
            decimal rate = decimal.Parse(rateKvp.Value);

            // Strategy pattern: cada tipo de habitación aplica su propio multiplicador
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

        // ── Calcular el cargo total acumulado de un booking ───────────
        public async Task<decimal> CalculateTotalCharge(int BookingId)
        {
            var rawBooking   = await _bookingRepo.GetBooking(BookingId);
            var lcosInOneRoom = await _lateCheckOutRepo.GetLateCheckOutsFromBookingId(rawBooking.Id);

            if (lcosInOneRoom == null || lcosInOneRoom.Count == 0)
                return 0.0m;
            rawBooking.Total = lcosInOneRoom.Sum(lco => lco.Charge);
            await _bookingRepo.ChangeBookingStatus(rawBooking);
            // Los cargos ya fueron calculados con Strategy al momento de crearlos,
            // así que simplemente los sumamos.
            return lcosInOneRoom.Sum(lco => lco.Charge);
        }
    }
}