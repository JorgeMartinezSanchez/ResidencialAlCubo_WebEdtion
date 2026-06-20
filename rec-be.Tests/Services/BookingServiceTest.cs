using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using rec_be.DTOs.BookingDTOs;
using rec_be.Interfaces.Factory;
using rec_be.Interfaces.Repository;
using rec_be.Interfaces.Strategy;
using rec_be.Models;
using rec_be.Services;
using rec_be.RoomStrategy;
using Xunit;
using rec_be.Interfaces.Services;
using System.Data.Common;

namespace rec_be.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository>    _mockBookingRepo;
        private readonly Mock<IRoomRepository>       _mockRoomRepo;
        private readonly Mock<IConfigRepository>     _mockConfigRepo;
        private readonly Mock<ILateCheckOutService>  _mockLateCheckOutService;
        private readonly Mock<IRoomStrategyFactory>  _mockStrategyFactory;

        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockBookingRepo         = new Mock<IBookingRepository>();
            _mockRoomRepo            = new Mock<IRoomRepository>();
            _mockConfigRepo          = new Mock<IConfigRepository>();
            _mockLateCheckOutService = new Mock<ILateCheckOutService>();
            _mockStrategyFactory     = new Mock<IRoomStrategyFactory>();

            _service = new BookingService(
                _mockBookingRepo.Object,
                _mockRoomRepo.Object,
                _mockConfigRepo.Object,
                _mockLateCheckOutService.Object,
                _mockStrategyFactory.Object);
        }

        [Fact]
        public async Task CreateBooking_WithSimpleRoomType_AssignsCorrectStrategy()
        {
            decimal lateCheckoutRate = 10.00m;

            var mockStrategy = new Mock<IRoomStrategy>();
            mockStrategy.Setup(s => s.GetMaxCapacity()).Returns(1);
            mockStrategy.Setup(s => s.ValidateGuestCount(It.IsAny<int>())).Returns(true);

            RoomType roomType = new RoomType
            {
                Id = 1,
                TypeName = "Simple",
                Price = 100.00m,
                Capacity = 1
            };

            Room sampleRoom = new Room
            {
                Id = 1,
                RoomTypeId = roomType.Id,
                RoomNumber = "001",
                Occupied = false,
                RoomType = roomType
            };

            BookingRequestDTO bookingRequestDTO = new BookingRequestDTO
            {
                RoomId = sampleRoom.Id,
                StartDate = new DateOnly(2026, 7, 1),
                EndDate = new DateOnly(2026, 7, 11),
                GuestIds = new List<int> { 1 }
            };

            _mockRoomRepo.Setup(r => r.GetRoomWithTypeById(sampleRoom.Id))
                .ReturnsAsync(sampleRoom);

            _mockConfigRepo.Setup(c => c.GetConfigByKey("LateCheckOutHourlyRate"))
                .ReturnsAsync(new KeyValuePair<string, string> ("LateCheckOutHourlyRate", lateCheckoutRate.ToString()));

            _mockStrategyFactory.Setup(sf => sf.CreateStrategy(It.IsAny<Room>(), It.IsAny<decimal>()))
                .Returns(mockStrategy.Object);

            _mockBookingRepo.Setup(r => r.GetAllBookings())
                .ReturnsAsync(new List<Booking>());

            _mockBookingRepo.Setup(r => r.CreateBooking(It.IsAny<Booking>()))
                .ReturnsAsync((Booking b) => b);

            var result = await _service.CreateBooking(bookingRequestDTO, bookingRequestDTO.GuestIds);

            Assert.NotNull(result);
            Assert.Equal("Simple", result.RoomTypeName);
            Assert.Equal("pending", result.Status);
            _mockStrategyFactory.Verify(sf => sf.CreateStrategy(It.IsAny<Room>(), It.IsAny<decimal>()), Times.Once);
        }


        [Fact]
        public async Task CheckIn_WithCancelledBooking_ThrowsException()
        {

            var booking = new Booking
            {
                Id = 1,
                RoomId = 1,
                Status = "cancelled",
                StartDate = new DateOnly(2026, 7, 1),
                EndDate = new DateOnly(2026, 7, 11)
            };

            var room = new Room
            {
                Id = 1,
                RoomNumber = "001",
                Occupied = false,
                RoomType = new RoomType { Id = 1, TypeName = "Simple", Price = 100.00m, Capacity = 1 }
            };

            _mockBookingRepo.Setup(r => r.GetBooking(1))
                .ReturnsAsync(booking);
            _mockRoomRepo.Setup(r => r.GetRoomWithTypeById(1))
                .ReturnsAsync(room);


            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.CheckIn(1));

            Assert.Contains("cancelled", exception.Message);
        }
    }
}