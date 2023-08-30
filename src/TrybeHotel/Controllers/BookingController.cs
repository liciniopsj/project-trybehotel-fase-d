using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TrybeHotel.Dto;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]

    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;
        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var NewBooking = _repository.Add(bookingInsert, userEmail);
                return Created(string.Empty, NewBooking);
            }
            catch (Exception err)
            {
                return BadRequest(new { message = err.Message });
            }
        }


        [HttpGet("{Bookingid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Policy = "Client")]
        public IActionResult GetBooking(int Bookingid)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var booking = _repository.GetBooking(Bookingid, userEmail);
                return Ok(booking);
            }
            catch (Exception err)
            {
                return Unauthorized(new { message = err.Message });
            }
        }
    }
}