using System;

namespace Bogevang.Booking.Domain.Bookings.Models
{
  public class CalendarBookingDay
  {
    public DateTime Date { get; set; }

    public BookingSummary Booking { get; set; }
  }
}
