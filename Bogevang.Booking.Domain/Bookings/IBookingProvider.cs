using Bogevang.Booking.Domain.Bookings.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public interface IBookingProvider
  {
    Task<List<BookingSummary>> FindBookingsInInterval(DateTime? start, DateTime? end);
  }
}
