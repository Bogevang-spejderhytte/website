using Bogevang.Booking.Domain.Bookings.Models;
using System.Collections.Generic;

namespace Bogevang.Booking.Website.ViewComponents
{
  public class BookingCalendarViewModel
  {
    public List<BookingSummary> Bookings { get; set; }
  }
}
