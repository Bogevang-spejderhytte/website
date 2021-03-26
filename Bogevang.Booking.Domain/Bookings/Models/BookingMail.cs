using Cofoundry.Core.Mail;

namespace Bogevang.Booking.Domain.Bookings.Models
{
  public class BookingMail
  {
    public string Description { get; set; }
    public MailAddress To { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
  }
}
