using System;

namespace Bogevang.Booking.Domain.Bookings.CustomEntities
{
  public class BookingLogEntry
  {
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public string Username { get; set; }
    public int UserId { get; set; }
  }
}
