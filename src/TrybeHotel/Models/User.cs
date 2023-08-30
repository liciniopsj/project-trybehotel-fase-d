namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// 1. Implemente as models da aplicação
public class User
{
  public int UserId { get; set; }
  public string Name { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;
  public string UserType { get; set; } = null!;
  public ICollection<Booking>? Bookings { get; set; }
}