#nullable disable
using System.Net;
using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var response = await _client.GetAsync("https://nominatim.openstreetmap.org/status.php?format=json");
            var result = await response.Content.ReadFromJsonAsync<object>();
            return result;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var url = $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1";

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "aspnet-user-agent");

            using var response = await _client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeoDtoResponse[]>();
                return result?.FirstOrDefault();
            }

            return null;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var location = await GetGeoLocation(geoDto);
            var hotels = repository.GetHotels();

            var geoDtoHotelResponses = new List<GeoDtoHotelResponse>();

            foreach (var hotel in hotels)
            {
                var hotelLocation = await GetGeoLocation
                (
                    new GeoDto
                    {
                        Address = hotel.Address,
                        City = hotel.CityName,
                        State = hotel.State
                    }
                );

                var hotelDistance = CalculateDistance
                (
                    location?.lat!,
                    location?.lon!,
                    hotelLocation?.lat!,
                    hotelLocation?.lon!
                );

                geoDtoHotelResponses
                    .Add(new GeoDtoHotelResponse
                {
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityName = hotel.CityName,
                    State = hotel.State,
                    Distance = hotelDistance
                });
            }

            geoDtoHotelResponses
                .Sort((x, y) => x.Distance
                    .CompareTo(y.Distance));

            return geoDtoHotelResponses;
        }



        public int CalculateDistance(string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny)
        {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.', ','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.', ','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.', ','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.', ','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return int.Parse(Math.Round(distance, 0).ToString());
        }

        public double radiano(double degree)
        {
            return degree * Math.PI / 180;
        }

    }
}