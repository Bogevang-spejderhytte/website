using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public interface IBookingProvider
  {
    Task<BookingDataModel> GetBookingById(int bookingId);
    Task<IEnumerable<BookingSummary>> FindBookingsInInterval(SearchBookingSummariesQuery query);
  }
}
