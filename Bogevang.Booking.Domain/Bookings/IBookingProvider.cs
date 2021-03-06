﻿using Bogevang.Booking.Domain.Bookings.CustomEntities;
using Bogevang.Booking.Domain.Bookings.Models;
using Bogevang.Booking.Domain.Bookings.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public interface IBookingProvider
  {
    Task<BookingDataModel> GetBookingById(int bookingId);
    Task<BookingSummary> GetBookingSummaryById(int bookingId);
    Task<IList<BookingSummary>> FindBookingsInInterval(SearchBookingSummariesQuery query);
    Task<IList<KeyValuePair<int,BookingDataModel>>> FindBookingDataInInterval(SearchBookingSummariesQuery query);
  }
}
