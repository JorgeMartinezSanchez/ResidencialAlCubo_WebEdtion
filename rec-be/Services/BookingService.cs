// 📁 Services/BookingService.cs
using Microsoft.EntityFrameworkCore;
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
        private readonly IRoomStrategyFactory    _strategyFactory;

        public BookingService(
            IBookingRepository   bookingRepo,
            IRoomRepository      roomRepo,
            IConfigRepository    configRepo,
            ILateCheckOutService lateCheckOutService,
            IRoomStrategyFactory strategyFactory)
        {
            _bookingRepo         = bookingRepo;
            _roomRepo            = roomRepo;
            _configRepo          = configRepo;
            _lateCheckOutService = lateCheckOutService;
            _strategyFactory     = strategyFactory;
        }

        // ── State pattern: resuelve el estado correcto en runtime ─────
        private IBookingState ResolveState(string status) => status switch
        {
            "pending"   => new PendingState(),
            "active"    => new ActiveState(),
            "cancelled" => new CancelledState(),
            "finished"  => new FinishedState(),
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

        public async Task<BookingResponseDTO> CreateBooking(BookingRequestDTO bookingRequest, List<int> guestIds)
        {
            try
            {
                // CA-2: Validate dates
                if (!ValidateDate(bookingRequest.StartDate, bookingRequest.EndDate))
                    throw new Exception("BOOKING SERVICE ERROR: End date must be after start date.");

                // Log the room ID being requested
                Console.WriteLine($"Attempting to get room with ID: {bookingRequest.RoomId}");
                
                var room = await _roomRepo.GetRoomWithTypeById(bookingRequest.RoomId);

                Console.WriteLine($"Room found: ID={room.Id}, Number={room.RoomNumber}, Type={room.RoomType?.TypeName}");

                // CA-1: Check if room exists and is not occupied
                if (room.Occupied)
                    throw new Exception("BOOKING SERVICE ERROR: Room is currently occupied.");

                // CA-4: Validate guest count against room capacity
                var guestCount = guestIds?.Count ?? 0;
                var rateKvp = await _configRepo.GetConfigByKey("LateCheckOutHourlyRate");
                decimal rate = decimal.Parse(rateKvp.Value);
                var strategy = _strategyFactory.CreateStrategy(room, rate);
                
                if (!ValidateGuestAmount(guestCount, strategy))
                    throw new Exception($"BOOKING SERVICE ERROR: Room capacity is {strategy.GetMaxCapacity()} guests, but you're trying to book for {guestCount} guests.");

                // Prevent overlapping reservations
                var allBookings = await _bookingRepo.GetAllBookings();
                bool overlaps = allBookings.Any(b =>
                    b.RoomId == bookingRequest.RoomId &&
                    b.Status != "cancelled" &&
                    b.Status != "finished" &&
                    b.StartDate < bookingRequest.EndDate &&
                    b.EndDate > bookingRequest.StartDate);

                if (overlaps)
                    throw new Exception("BOOKING SERVICE ERROR: Room is already reserved for that date range.");

                var total = room.RoomType!.Price * (bookingRequest.EndDate.DayNumber - bookingRequest.StartDate.DayNumber);
                // Create the booking
                var newBooking = new Booking
                {
                    RoomId = bookingRequest.RoomId,
                    StartDate = bookingRequest.StartDate,
                    EndDate = bookingRequest.EndDate,
                    Status = "pending",
                    CheckInDate = default,
                    CheckOutDate = default,
                    Total = total
                };

                // Log before saving
                Console.WriteLine($"Creating booking: RoomId={newBooking.RoomId}, StartDate={newBooking.StartDate}, EndDate={newBooking.EndDate}, Total={newBooking.Total}");
                
                var created = await _bookingRepo.CreateBooking(newBooking);

                Console.WriteLine($"Booking created with ID: {created.Id}");

                if (guestIds != null && guestIds.Any())
                {
                    foreach (var guestId in guestIds)
                    {
                        Console.WriteLine($"Checking guest ID: {guestId}");
                    }
                    
                    await _bookingRepo.AssignGuestsToBooking(created.Id, guestIds);
                }
                
                return MapToDTO(created, room);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // HU-03
        public async Task<List<BookingResponseDTO>> GetAllBookings()
        {
            var bookings = await _bookingRepo.GetAllBookings();
            var rooms    = await _roomRepo.GetAllRooms();
            var roomMap  = rooms.ToDictionary(r => r.Id);

            return bookings
                .Where(b => b.Status == "pending" || b.Status == "active")
                .OrderBy(b => b.StartDate)
                .Select(b =>
                {
                    roomMap.TryGetValue(b.RoomId, out var room);
                    return MapToDTO(b, room ?? new Room());
                })
                .ToList();
        }

        // HU-04 
        public async Task<BookingResponseDTO> CheckIn(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            IBookingState state = ResolveState(booking.Status);
            state.CheckIn(booking);
            booking.CheckInDate = DateTime.UtcNow;

            room.Occupied = true;
            await _roomRepo.SetRoomOccupation(room);
            await _bookingRepo.ChangeBookingStatus(booking);

            return MapToDTO(booking, room);
        }

        //  HU-08: Check Out con optional late Check-Out
        public async Task<BookingResponseDTO> CheckOut(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var room = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            IBookingState state = ResolveState(booking.Status);
            state.CheckOut(booking);
            booking.CheckOutDate = DateTime.UtcNow;
            
            decimal lateCheckOutCharge = 0;

            // Verify if late checkout applies to this
            var limitKvp = await _configRepo.GetConfigByKey("checkout_limit_hour");
            int limitHour = int.Parse(limitKvp.Value);
            var now = DateTime.Now;

            if (now.Hour >= limitHour)
            {
                int extraHours = now.Hour - limitHour + 1;
                
                // Create late check-out and get the charge
                var lateCheckOut = await _lateCheckOutService.CreateLateCheckOut(new LateCheckOutRequestDTO
                {
                    BookingId = booking.Id,
                    ExtraHours = extraHours
                });
                
                lateCheckOutCharge = lateCheckOut.Charge;
            }

            // Add the charge of the late checkout al total de la reserva
            booking.Total += lateCheckOutCharge;

            room.Occupied = false;
            await _roomRepo.SetRoomOccupation(room);
            await _bookingRepo.ChangeBookingStatus(booking);

            return MapToDTO(booking, room);
        }

        public async Task<BookingResponseDTO> Cancel(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var room    = await _roomRepo.GetRoomWithTypeById(booking.RoomId);

            IBookingState state = ResolveState(booking.Status);
            state.Cancel(booking);

            if (room.Occupied)
            {
                room.Occupied = false;
                await _roomRepo.SetRoomOccupation(room);
            }

            await _bookingRepo.ChangeBookingStatus(booking);
            return MapToDTO(booking, room);
        }

        // HU-05
        public bool ValidateGuestAmount(int amount, IRoomStrategy room)
            => room.ValidateGuestCount(amount);

        // date validation
        public bool ValidateDate(DateOnly startDate, DateOnly endDate)
            => endDate > startDate;

        public async Task<decimal> RecalculateTotalWithLateCheckOuts(int bookingId)
        {
            var booking = await _bookingRepo.GetBooking(bookingId);
            var lateCheckOuts = await _lateCheckOutService.GetLateCheckOutsByBookingId(bookingId);
            
            decimal lateCheckOutTotal = lateCheckOuts.Sum(lco => lco.Charge);
            booking.Total += lateCheckOutTotal;
            
            await _bookingRepo.ChangeBookingStatus(booking);
            return booking.Total;
        }
    }
}