#nullable disable
using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 6. Desenvolva o endpoint GET /room/:hotelId
        public IEnumerable<RoomDto> GetRooms(int hotelId)
        {
            return _context.Rooms
                .Where(room => room.HotelId == hotelId)
                .Select(room => new RoomDto
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
                }).ToList();
        }


        // 8. Desenvolva o endpoint POST /room
        public RoomDto AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();

            var hotelInfo = _context.Hotels
                .Where(hotel => hotel.HotelId == room.HotelId)
                .Select(hotel => new HotelDto
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityId = hotel.CityId,
                    CityName = _context.Cities
                        .Where(city => city.CityId == hotel.CityId)
                        .Select(city => city.Name)
                        .FirstOrDefault(),
                    State = _context.Cities
                        .Where(city => city.CityId == hotel.CityId)
                        .Select(city => city.State)
                        .FirstOrDefault(),
                }).FirstOrDefault();

            return new RoomDto
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Image = room.Image,
                Hotel = hotelInfo
            };
        }

        // 8. Desenvolva o endpoint DELETE /room/:roomId
        public void DeleteRoom(int RoomId)
        {
            var targetRoom = _context.Rooms.Find(RoomId);
            if (targetRoom != null) 
            {
                _context.Rooms.Remove(targetRoom);
                _context.SaveChanges();
            }
        }
    }
}