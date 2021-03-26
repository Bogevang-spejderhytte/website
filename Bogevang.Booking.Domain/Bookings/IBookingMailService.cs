using Bogevang.Booking.Domain.Bookings.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bogevang.Booking.Domain.Bookings
{
  public interface IBookingMailService
  {
    Task<BookingMail> CreateBookingMail(int bookingId, string templateName);
  }
}
