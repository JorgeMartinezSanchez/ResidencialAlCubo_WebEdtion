// 📁 Services/BookingService.cs
using rec_be.DTOs.BookingDTOs;
using rec_be.DTOs.LateCheckOutDTO;
using rec_be.Interfaces.Factory;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Services;
using rec_be.Interfaces.States;
using rec_be.Interfaces.Strategy;
using rec_be.Models;
using rec_be.RoomStrategy;
using rec_be.States.Booking_;

namespace rec_be.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository      _bookingRepo;
        private readonly IRoomRepository         _roomRepo;
        private readonly IConfigRepository       _configRepo;
        private readonly ILateCheckOutService    _lateCheckOutService;   // ← reemplaza repo + factory

        public BookingService(
            IBookingRepository   bookingRepo,
            IRoomRepository      roomRepo,
            IConfigRepository    configRepo,
            ILateCheckOutService lateCheckOutService)
        {
            _bookingRepo         = bookingRepo;
            _roomRepo            = roomRepo;
            _configRepo          = configRepo;
            _lateCheckOutService = lateCheckOutService;
        }

        // ── State pattern: resuelve el estado correcto en runtime ─────
        private IBookingState ResolveState(string status) => status switch
        {
            "Pending"   => new PendingState(),
            "Active"    => new ActiveState(),
            "Cancelled" => new CancelledState(),
            "Finished"  => new FinishedState(),
            _ => throw new Exception($"BOOKING SERVICE ERROR: Unknown booking status '{status}'.")
        };

        // ── DTO mapper ────────────────────────────────────────────────
        private static BookingResponseDTO MapToDTO(Booking booking, Room room) =>
            new BookingResponseDTO
            {
                Id           = booking.Id,
                RoomNumber   = room.RoomNumber,
                RoomTypeName = room.RoomType?.TypeName ?? "",
                StartDate    = booking.StartDate,
                EndDate      = booking.EndDate,
                Status       = booking.Status,
                Total        = booking.Total
            };

        // ── HU-02: Crear reserva ──────────────────────────────────────
        public async Task<BookingResponseDTO> CreateBooking(BookingRequestDTO bookingRequest)
        {
            if (!ValidateDate(bookingRequest.StartDate, bookingRequest.EndDate))
                throw new Exception("BOOKING SERVICE ERROR: End date must be after start date.");

            var room = await _roomRepo.GetRoomWithTypeById(bookingRequest.RoomId);

            if (room.Occupied)
                throw new Exception("BOOKING SERVICE ERROR: Room is currently occupied.");

            var allBookings = await _bookingRepo.GetAllBookings();
            bool overlaps = allBookings.Any(b =>
                b.RoomId == bookingRequest.RoomId &&
                b.Status != "Cancelled" &&
                b.Status != "Finished"  &&
                b.StartDate < bookingRequest.EndDate &&
                b.EndDate   > bookingRequest.StartDate);

            if (overlaps)
                throw new Exception("BOOKING SERVICE ERROR: Room is already reserved for that date range.");

            var newBooking = new Booking
            {
                RoomId      = bookingRequest.RoomId,
                StartDate   = bookingRequest.StartDate,
                EndDate     = bookingRequest.EndDate,
                Status      = "Pending",
                CheckInDate = default,
                Total       = bookingRequest.Total
            };

            var created = await _bookingRepo.CreateBooking(newBooking);
            return MapToDTO(created, room);
        }

        // ── HU-03: Listar reservas activas y futuras ──────────────────
        public async Task<List<BookingResponseDTO>> GetAllBookings()
        {
            var bookings = await _bookingRepo.GetAllBookings();
            var rooms    = await _roomRepo.GetAllRooms();
            var roomMap  = rooms.ToDictionary(r => r.Id);

            return bookings
                .Where(b => b.Status == "Pending" || b.Status == "Active")
                .OrderBy(b => b.StartDate)
                .Select(b =>
                {
                    roomMap.TryGetValue(b.RoomId, out var room);
                    return MapToDTO(b, room ?? new Room());
                })
                .ToList();
        }

        // ── HU-04: Check In ───────────────────────────────────────────
        public async Task<BookingResponseDTO> CheckIn(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            IBookingState state = ResolveState(booking.Status);
            state.CheckIn(booking);                  // Status → "Active"
            booking.CheckInDate = DateTime.UtcNow;

            room.Occupied = true;
            await _roomRepo.SetRoomOccupation(room);
            await _bookingRepo.ChangeBookingStatus(booking);

            return MapToDTO(booking, room);
        }

        // ── HU-08: Check Out con Late Check-Out opcional ──────────────
        public async Task<BookingResponseDTO> CheckOut(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            IBookingState state = ResolveState(booking.Status);
            state.CheckOut(booking);                 // Status → "Finished"

            // Verificar si aplica Late Check-Out
            var limitKvp  = await _configRepo.GetConfigByKey("checkout_limit_hour");
            int limitHour = int.Parse(limitKvp.Value);
            var now       = DateTime.UtcNow;

            if (now.Hour >= limitHour)
            {
                int extraHours = now.Hour - limitHour + 1;

                // Delegamos al LateCheckOutService — él resuelve el Strategy internamente
                await _lateCheckOutService.CreateLateCheckOut(new LateCheckOutRequestDTO
                {
                    BookingId  = booking.Id,
                    ExtraHours = extraHours
                });
            }

            room.Occupied = false;
            await _roomRepo.SetRoomOccupation(room);
            await _bookingRepo.ChangeBookingStatus(booking);

            return MapToDTO(booking, room);
        }

        // ── Cancelar reserva ──────────────────────────────────────────
        public async Task<BookingResponseDTO> Cancel(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            IBookingState state = ResolveState(booking.Status);
            state.Cancel(booking);                   // Status → "Cancelled"

            if (room.Occupied)
            {
                room.Occupied = false;
                await _roomRepo.SetRoomOccupation(room);
            }

            await _bookingRepo.ChangeBookingStatus(booking);
            return MapToDTO(booking, room);
        }

        // ── HU-05: Validación de capacidad (llamado desde Controller) ─
        public bool ValidateGuestAmount(int amount, IRoomStrategy room)
            => room.ValidateGuestCount(amount);

        // ── Validación de fechas ──────────────────────────────────────
        public bool ValidateDate(DateOnly startDate, DateOnly endDate)
            => endDate > startDate;
    }
}