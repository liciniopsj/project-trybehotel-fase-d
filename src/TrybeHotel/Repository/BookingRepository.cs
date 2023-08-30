#nullable disable
using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = _context.Rooms
                .Include(room => room.Hotel)
                .ThenInclude(hotel => hotel.City)
                .FirstOrDefault(room => room.RoomId == booking.RoomId);

            if (room == null || booking.GuestQuant > room.Capacity)
            {
                throw new Exception("Guest quantity over room capacity");
            }

            var newBooking = new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = room
            };

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            return new BookingResponse
            {
                BookingId = newBooking.BookingId,
                CheckIn = newBooking.CheckIn,
                CheckOut = newBooking.CheckOut,
                GuestQuant = newBooking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.Hotel.HotelId,
                        Name = room.Hotel.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City.Name,
                        State = room.Hotel.City.State
                    }
                }
            };


        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            var booking = _context.Bookings
                .Include(booking => booking.Room)
                .ThenInclude(room => room.Hotel)
                .ThenInclude(hotel => hotel.City)
                .FirstOrDefault(booking => booking.BookingId == bookingId);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == email);

            if (booking == null || user == null || user.UserId != booking.UserId)
            {
                throw new Exception("Booking not found");
            }

            var room = GetRoomById(booking.RoomId);

            return new BookingResponse
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.Hotel.HotelId,
                        Name = room.Hotel.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City.Name,
                        State = room.Hotel.City.State
                    }
                },
            };
        }

        public Room GetRoomById(int roomId)
        {
            return _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
        }

    }

}