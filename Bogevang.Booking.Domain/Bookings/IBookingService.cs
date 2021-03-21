using Bogevang.Booking.Domain.Bookings.CustomEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public interface IBookingService
  {
    Task<List<BookingDataModel>> FindBookingsInInterval(DateTime start, DateTime end);
  }
}
