using System;

namespace Bogevang.Booking.Domain.Bookings.Queries
{
  public class SearchBookingSummariesQuery
  {
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
  }
}
