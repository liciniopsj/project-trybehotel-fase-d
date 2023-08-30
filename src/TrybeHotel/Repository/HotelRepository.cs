using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class HotelRepository : IHotelRepository
    {
        protected readonly ITrybeHotelContext _context;
        public HotelRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Desenvolva o endpoint GET /hotel
        public IEnumerable<HotelDto> GetHotels()
        {
            var hotelsWithCity = _context.Hotels
                .Include(hotel => hotel.City)
                .Select(hotel => new
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityId = hotel.CityId,
                    CityName = hotel.City != null ? hotel.City.Name : null,
                    state = hotel.City != null ? hotel.City.State: null,
                }).ToList();

            var hotelsDto = hotelsWithCity.Select(hotel => new HotelDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                CityId = hotel.CityId,
                CityName = hotel.CityName,
                State = hotel.state,
            });

            return hotelsDto.ToList();
        }

        // 6. Desenvolva o endpoint POST /hotel
        public HotelDto AddHotel(Hotel hotel)
        {
            var city = _context.Cities.Find(hotel.CityId);

            var newHotel = new Hotel
            {
                Name = hotel.Name,
                Address = hotel.Address,
                CityId = hotel.CityId
            };

            _context.Hotels.Add(newHotel);
            _context.SaveChanges();

            return new HotelDto
            {
                HotelId = newHotel.HotelId,
                Name = newHotel.Name,
                Address = newHotel.Address,
                CityId = newHotel.CityId,
                CityName = city?.Name,
                State = city?.State,
            };
        }
    }
}