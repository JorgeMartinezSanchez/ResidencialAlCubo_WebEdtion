using Moq;
using Xunit;
using rec_be.Interfaces.Repository;
using rec_be.Services;
using rec_be.Models;
using rec_be.Interfaces.Factory;
using rec_be.Interfaces.Strategy;
using rec_be.DTOs.LateCheckOutDTO;

namespace rec_be.Tests.Services
{
    public class LateCheckOutServiceTest
    {
        private readonly Mock<ILateCheckOutRepository> _mockLateCheckOutRepo;
        private readonly Mock<IBookingRepository> _mockBookingRepo;
        private readonly Mock<IRoomRepository> _mockRoomRepo;
        private readonly Mock<IConfigRepository> _mockConfigRepo;
        private readonly Mock<IRoomStrategyFactory> _mockStrategyFactory;
        private readonly LateCheckOutService _lateCheckOutService;

        public LateCheckOutServiceTest()
        {
            _mockLateCheckOutRepo = new Mock<ILateCheckOutRepository>();
            _mockBookingRepo = new Mock<IBookingRepository>();
            _mockRoomRepo = new Mock<IRoomRepository>();
            _mockConfigRepo = new Mock<IConfigRepository>();
            _mockStrategyFactory = new Mock<IRoomStrategyFactory>();
            
            _lateCheckOutService = new LateCheckOutService(
                _mockLateCheckOutRepo.Object,
                _mockBookingRepo.Object,
                _mockRoomRepo.Object,
                _mockConfigRepo.Object,
                _mockStrategyFactory.Object
            );
        }

        [Fact]
        public async Task CreateLateCheckOut()
        {
            Booking booking = new Booking
            {
                Id = 1,
                RoomId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                Status = "Active",
                CheckInDate = DateTime.MinValue,
                CheckOutDate = DateTime.MinValue,
                Total = 200.00m,
                CreationDate = DateTime.Now
            };
            
            _mockBookingRepo.Setup(b => b.GetBooking(It.IsAny<int>()))
                .ReturnsAsync(booking);
            
            Room room = new Room 
            { 
                Id = 1, 
                RoomNumber = "101",
                RoomType = new RoomType { Price = 100.00m } 
            };
            _mockRoomRepo.Setup(r => r.GetRoomWithTypeById(It.IsAny<int>()))
                .ReturnsAsync(room);

            _mockConfigRepo.Setup(c => c.GetConfigByKey("LateCheckOutHourlyRate"))
                .ReturnsAsync(new KeyValuePair<string, string>("LateCheckOutHourlyRate", "10.00"));

            var strategyMock = new Mock<IRoomStrategy>();
            strategyMock.Setup(s => s.CalculateLateCheckoutFee(It.IsAny<int>()))
                .Returns(30.00m);
            _mockStrategyFactory.Setup(f => f.CreateStrategy(It.IsAny<Room>(), It.IsAny<decimal>()))
                .Returns(strategyMock.Object);

            _mockLateCheckOutRepo.Setup(r => r.CreateLateCheckout(It.IsAny<LateCheckOut>()))
                .ReturnsAsync((LateCheckOut lco) => lco);

            LateCheckOutRequestDTO newlateCheckOut = new LateCheckOutRequestDTO
            {
                BookingId = 1,
                ExtraHours = 3
            };
                
            var result = await _lateCheckOutService.CreateLateCheckOut(newlateCheckOut);

            Assert.NotNull(result);
            Assert.Equal(1, result.BookingId);
            Assert.Equal(3, result.ExtraHours);
            Assert.Equal(30.00m, result.Charge);
        }

        [Fact]
        public async Task LateCheckOutWithoutPreviousCheckIn()
        {
            Booking booking = new Booking
            {
                Id = 1,
                RoomId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                Status = "Active",
                Total = 200.00m,
                CreationDate = DateTime.Now
            };
            
            _mockBookingRepo.Setup(b => b.GetBooking(It.IsAny<int>()))
                .ReturnsAsync(booking);
            
            Room room = new Room 
            { 
                Id = 1, 
                RoomNumber = "101",
                RoomType = new RoomType { Price = 100.00m } 
            };
            _mockRoomRepo.Setup(r => r.GetRoomWithTypeById(It.IsAny<int>()))
                .ReturnsAsync(room);

            _mockConfigRepo.Setup(c => c.GetConfigByKey("LateCheckOutHourlyRate"))
                .ReturnsAsync(new KeyValuePair<string, string>("LateCheckOutHourlyRate", "10.00"));

            var strategyMock = new Mock<IRoomStrategy>();
            strategyMock.Setup(s => s.CalculateLateCheckoutFee(It.IsAny<int>()))
                .Returns(30.00m);
            _mockStrategyFactory.Setup(f => f.CreateStrategy(It.IsAny<Room>(), It.IsAny<decimal>()))
                .Returns(strategyMock.Object);

            _mockLateCheckOutRepo.Setup(r => r.CreateLateCheckout(It.IsAny<LateCheckOut>()))
                .ReturnsAsync((LateCheckOut lco) => lco);

            LateCheckOutRequestDTO newlateCheckOut = new LateCheckOutRequestDTO
            {
                BookingId = 1,
                ExtraHours = 3
            };
                
            var result = await _lateCheckOutService.CreateLateCheckOut(newlateCheckOut);

            Assert.NotNull(result);
            Assert.Equal(1, result.BookingId);
            Assert.Equal(3, result.ExtraHours);
            Assert.Equal(30.00m, result.Charge);
        } 
    }
}